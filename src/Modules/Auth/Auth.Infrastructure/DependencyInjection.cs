using Auth.Domain.Interfaces;
using Auth.Infrastructure.Persistence;
using Auth.Infrastructure.Repositories;
using Auth.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Auth.Infrastructure;

/// <summary>
/// Infrastructure layer dependency injection configuration
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddAuthInfrastructure(
        this IServiceCollection services, 
        IConfiguration configuration)
    {
        // Register DbContext
        services.AddDbContext<AuthDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly(typeof(AuthDbContext).Assembly.FullName)));

        // Configure Identity
        services.AddIdentity<IdentityUser, IdentityRole>(options =>
        {
            // 🛡️ SECURITY: Password settings (VULN-002 Fixed)
            options.Password.RequireDigit = true;
            options.Password.RequireLowercase = true;
            options.Password.RequireUppercase = true;
            options.Password.RequireNonAlphanumeric = true;  // ✅ Exige caracteres especiais (!@#$%^&*)
            options.Password.RequiredLength = 12;            // ✅ Aumentado de 6 para 12 caracteres
            options.Password.RequiredUniqueChars = 4;        // ✅ Exige 4 caracteres únicos

            // 🛡️ SECURITY: Lockout settings (anti-brute force)
            options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15); // ✅ Aumentado de 5 para 15 min
            options.Lockout.MaxFailedAccessAttempts = 3;                        // ✅ Reduzido de 5 para 3 tentativas
            options.Lockout.AllowedForNewUsers = true;

            // User settings
            options.User.RequireUniqueEmail = true;
            options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
        })
        .AddEntityFrameworkStores<AuthDbContext>()
        .AddDefaultTokenProviders(); // ✅ Token providers para reset password, email confirmation, etc.

        // ✅ Configure Dual Authentication: JWT Bearer (API) + Cookie (Web UI)
        services.AddAuthentication(options =>
        {
            // Usa um esquema "smart" que escolhe automaticamente entre JWT e Cookie
            options.DefaultScheme = "Smart";
            options.DefaultAuthenticateScheme = "Smart";
            options.DefaultChallengeScheme = "Smart";
        })
        .AddPolicyScheme("Smart", "Smart Auth Scheme", options =>
        {
            // Escolhe JWT Bearer para APIs (requisições com Authorization header)
            // Escolhe Cookie para páginas web (requisições sem Authorization header)
            options.ForwardDefaultSelector = context =>
            {
                // Se tem header Authorization, usa JWT Bearer
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    return JwtBearerDefaults.AuthenticationScheme;
                }
                // Senão, usa Cookie (Admin UI)
                return IdentityConstants.ApplicationScheme;
            };
        })
        .AddJwtBearer(options =>
        {
            options.SaveToken = false;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["JWT:ValidIssuer"],
                ValidAudience = configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? 
                        throw new InvalidOperationException("JWT:Secret not configured")))
            };
        });

        // ✅ Configure Identity Application Cookie (usado pelo Admin UI)
        services.ConfigureApplicationCookie(options =>
        {
            // ✅ CRITICAL: Cookie Authentication Paths para Admin.UI
            options.LoginPath = "/Admin/Account/Login";
            options.LogoutPath = "/Admin/Account/Logout";
            options.AccessDeniedPath = "/Admin/Account/AccessDenied";
            options.ExpireTimeSpan = TimeSpan.FromHours(8);
            options.SlidingExpiration = true;
            options.Cookie.Name = "AdminPanel.Auth";
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });

        // Register repositories
        services.AddScoped<IUserRepository, IdentityUserRepository>();
        services.AddScoped<IRoleRepository, IdentityRoleRepository>();

        // Register services
        services.AddScoped<ITokenService, JwtTokenService>();
        services.AddScoped<IAuthenticationService, IdentityAuthenticationService>();

        return services;
    }
}
