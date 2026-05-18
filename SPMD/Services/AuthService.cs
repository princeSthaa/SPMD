using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Models;
using BCrypt.Net;

namespace SPMD.Services
{
    public class AuthService
    {
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public AuthService(UserService userService, IConfiguration configuration, AppDbContext context)
        {
            _userService = userService;
            _configuration = configuration;
            _context = context;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userService.GetByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            var token = GenerateJwtToken(user);
            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Role = user.Role.Name.ToString()
            };
        }

        public async Task<User?> RegisterAsync(RegisterDto registerDto)
        {
            var existingUser = await _userService.GetByUsernameAsync(registerDto.Username);
            if (existingUser != null) return null;

            var targetRole = await _context.Roles.FirstOrDefaultAsync(r => r.Name == (RoleName)registerDto.RoleId);
            if (targetRole == null) return null;

            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = new User
                {
                    Username = registerDto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Email = registerDto.Email,
                    RoleId = targetRole.RoleId,
                    IsActive = true
                };

                await _userService.CreateAsync(user);

                // Create the corresponding profile
                switch ((RoleName)registerDto.RoleId)
                {
                    case RoleName.Doctor:
                        _context.Doctors.Add(new Doctor { UserId = user.UserId, FirstName = user.Username, LastName = "(Pending Profile Setup)" });
                        break;
                    case RoleName.Pharmacist:
                        _context.Pharmacists.Add(new Pharmacist { UserId = user.UserId, FirstName = user.Username, LastName = "(Pending Profile Setup)" });
                        break;
                    case RoleName.Patient:
                        _context.Patients.Add(new Patient { UserId = user.UserId, FirstName = user.Username, LastName = "(Pending Profile Setup)", Gender = "U" });
                        break;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                return user;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                return null;
            }
        }

        private string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var key = Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "a_very_long_and_secure_secret_key_at_least_32_chars");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Name, user.Username),
                    new Claim(ClaimTypes.Role, user.Role.Name.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
