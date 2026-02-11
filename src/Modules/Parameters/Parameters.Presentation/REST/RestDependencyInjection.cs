using Microsoft.Extensions.DependencyInjection;

namespace Parameters.Presentation.REST;

/// <summary>
/// Extension methods para configurar REST API do módulo Parameters
/// </summary>
public static class RestDependencyInjection
{
    public static IServiceCollection AddParametersRestApi(this IServiceCollection services)
    {
        // Controllers são automaticamente descobertos pelo ASP.NET Core
        // Este método existe para possíveis configurações futuras específicas de REST
        
        return services;
    }
}
