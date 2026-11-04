using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstateApp.Core.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        // Aquí se registrarán los GenericServices y servicios específicos (Issue #3)
    }
}