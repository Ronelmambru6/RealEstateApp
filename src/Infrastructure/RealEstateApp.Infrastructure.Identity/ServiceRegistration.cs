using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstateApp.Infrastructure.Identity;

public static class ServiceRegistration
{
    public static void AddIdentityInfrastructureForWebApp(this IServiceCollection services, IConfiguration configuration)
    {
        // Aquí se configurará Identity con Cookies para MVC (Issue #4)
    }

    public static void AddIdentityInfrastructureForWebApi(this IServiceCollection services, IConfiguration configuration)
    {
        // Aquí se configurará Identity con JWT para la API (Issue #4 y #18)
    }
}