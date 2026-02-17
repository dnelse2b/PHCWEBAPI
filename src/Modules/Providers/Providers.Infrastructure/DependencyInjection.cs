using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Providers.Domain.Repositories;
using Providers.Domain.Services;
using Providers.Infrastructure.Persistence;
using Providers.Infrastructure.Repositories;
using Providers.Infrastructure.Services;
using Shared.Infrastructure.Persistence.Interceptors;

namespace Providers.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddProvidersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core Interceptors (do Shared)
        services.AddSingleton<AuditableEntityInterceptorEFCore>();

        // EF Core DbContext (Database First - sem migrations)
        services.AddDbContext<ProvidersDbContextEFCore>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptorEFCore>();

            options.UseSqlServer(configuration.GetConnectionString("DBconnect"))
                   .AddInterceptors(auditInterceptor);
        });

        // Repositories - EF Core implementation
        services.AddScoped<IProviderRepository, ProviderRepositoryEFCore>();
        services.AddScoped<IProviderValueRepository, ProviderValueRepositoryEFCore>();

        // Encryption Service
        var encryptionKey = configuration["Encryption:Key"] ?? "PHC_DEFAULT_ENCRYPTION_KEY_2024_SECURE";
        services.AddSingleton<IEncryptionService>(new EncryptionService(encryptionKey));

        return services;
    }
}
