using Microsoft.EntityFrameworkCore;
using SPMD.Data;
using SPMD.Data.Repositories;
using SPMD.Services;
using QuestPDF.Infrastructure;

// Set QuestPDF License
QuestPDF.Settings.License = LicenseType.Community;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSignalR();

builder.Services.AddDbContext<AppDbContext>(
    options => options.UseSqlServer(
      builder.Configuration.GetConnectionString("DefaultConnection")
    )
  );

// Register Generic Repository and Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<MedicineService>();
builder.Services.AddScoped<PatientService>();
builder.Services.AddScoped<PrescriptionService>();
builder.Services.AddScoped<AuditService>();
builder.Services.AddScoped<DispensingService>();
builder.Services.AddScoped<PdfService>();
builder.Services.AddScoped<IEmailService, EmailService>();

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = System.Text.Encoding.ASCII.GetBytes(jwtSettings["Key"] ?? "default_secret_key_ensure_this_is_changed");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultSignInScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme;
})
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = System.TimeSpan.FromDays(7);
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = System.TimeSpan.Zero
    };
});

var app = builder.Build();

// Seed the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<AppDbContext>();
    SPMD.Data.DbInitializer.Seed(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
  app.UseExceptionHandler("/Error");
  // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
  app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();
app.MapHub<SPMD.Hubs.NotificationHub>("/notificationHub");

app.Run();
