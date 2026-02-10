using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Parameters.Domain.Repositories;
using Parameters.Infrastructure.Persistence;
using Parameters.Infrastructure.Repositories;

namespace Parameters.Infrastructure;

/// <summary>
/// Extension methods para configurar a infraestrutura
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddParametersInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // DbContext
        services.AddDbContext<ParametersDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("ParametersConnection"),
                b => b.MigrationsAssembly(typeof(ParametersDbContext).Assembly.FullName)));

        // Repositories
        services.AddScoped<IE1Repository, E1Repository>();
        services.AddScoped<IE4Repository, E4Repository>();

        return services;
    }
}
