using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.Services;

namespace RealEstateApp.Core.Application;

public static class ServiceRegistration
{
    public static void AddApplicationLayer(this IServiceCollection services)
    {
        services.AddAutoMapper(cfg => cfg.AddMaps(Assembly.GetExecutingAssembly()));
        // Servicio genérico (Issue #3)
        services.AddTransient(typeof(IGenericService<,,>), typeof(GenericService<,,>));

        #region Módulos Dev A (Propiedades, Admin, Ofertas)
        #endregion

        #region Módulos Dev B (Catálogos, Favoritos, Chat)
        #endregion
    }
}