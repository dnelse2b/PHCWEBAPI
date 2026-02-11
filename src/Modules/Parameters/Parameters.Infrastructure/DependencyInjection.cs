using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameters.Domain.Repositories;
using Parameters.Infrastructure.Persistence;
using Parameters.Infrastructure.Persistence.Interceptors;
using Parameters.Infrastructure.Repositories;

namespace Parameters.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddParametersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Interceptors
        services.AddSingleton<AuditableEntityInterceptor>();

        // DbContext
        services.AddDbContext<ParametersDbContext>((sp, options) =>
        {
            var auditInterceptor = sp.GetRequiredService<AuditableEntityInterceptor>();

            options.UseSqlServer(
                configuration.GetConnectionString("ParametersConnection"),
                b => b.MigrationsAssembly(typeof(ParametersDbContext).Assembly.FullName))
            .AddInterceptors(auditInterceptor);
        });

        // Repositories
        services.AddScoped<IPara1Repository, Para1Repository>();

        return services;
    }
}

