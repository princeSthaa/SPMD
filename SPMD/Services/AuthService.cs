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
        private readonly IEmailService _emailService;

        public AuthService(UserService userService, IConfiguration configuration, AppDbContext context, IEmailService emailService)
        {
            _userService = userService;
            _configuration = configuration;
            _context = context;
            _emailService = emailService;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto)
        {
            var user = await _userService.GetByUsernameAsync(loginDto.Username);
            if (user == null || !BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                return null;
            }

            if (!user.IsActive)
            {
                // In a real app we might throw a specific exception or return a different result, 
                // but for now we'll just return null if they are not verified/active.
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
                string otpCode = new Random().Next(100000, 999999).ToString();
                var user = new User
                {
                    Username = registerDto.Username,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
                    Email = registerDto.Email,
                    RoleId = targetRole.RoleId,
                    IsActive = false,
                    VerificationCode = otpCode,
                    VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(15)
                };

                await _userService.CreateAsync(user);

                // Create the corresponding profile
                switch ((RoleName)registerDto.RoleId)
                {
                    case RoleName.Doctor:
                        _context.Doctors.Add(new Doctor 
                        { 
                            UserId = user.UserId, 
                            FirstName = registerDto.FirstName, 
                            LastName = registerDto.LastName,
                            BirthDate = registerDto.BirthDate,
                            HospitalEmployeeNumber = registerDto.HospitalEmployeeNumber,
                            Contact = registerDto.PhoneNumber
                        });
                        break;
                    case RoleName.Pharmacist:
                        _context.Pharmacists.Add(new Pharmacist 
                        { 
                            UserId = user.UserId, 
                            FirstName = registerDto.FirstName, 
                            LastName = registerDto.LastName,
                            BirthDate = registerDto.BirthDate,
                            HospitalEmployeeNumber = registerDto.HospitalEmployeeNumber,
                            Phone = registerDto.PhoneNumber
                        });
                        break;
                    case RoleName.Patient:
                        _context.Patients.Add(new Patient 
                        { 
                            UserId = user.UserId, 
                            FirstName = registerDto.FirstName, 
                            LastName = registerDto.LastName,
                            BirthDate = registerDto.BirthDate,
                            HealthId = registerDto.HealthId,
                            Phone = registerDto.PhoneNumber,
                            Email = registerDto.Email,
                            Gender = "U" 
                        });
                        break;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                // Send OTP Email
                await _emailService.SendEmailAsync(
                    user.Email,
                    "Verify your SPMD Account",
                    $"<h2>Welcome to SPMD!</h2><p>Your email verification code is: <strong>{otpCode}</strong></p><p>This code will expire in 15 minutes.</p>"
                );

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
