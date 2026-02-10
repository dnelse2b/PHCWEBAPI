using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using FluentValidation;
using Parameters.Infrastructure;

namespace Parameters.API;

/// <summary>
/// Extension methods para configurar o módulo de Parameters
/// </summary>
public static class DependencyInjection
{
    public static IServiceCollection AddParametersModule(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Application Layer
        services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssembly(typeof(Application.Commands.CreateParameterCommand).Assembly));

        services.AddValidatorsFromAssembly(
            typeof(Application.Validators.CreateParameterCommandValidator).Assembly);

        // Infrastructure Layer
        services.AddParametersInfrastructure(configuration);

        return services;
    }
}
