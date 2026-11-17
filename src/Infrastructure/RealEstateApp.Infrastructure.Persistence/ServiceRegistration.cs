using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstateApp.Core.Application.Interfaces.Repositories;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Infrastructure.Persistence.Contexts;
using RealEstateApp.Infrastructure.Persistence.Repositories;
using RealEstateApp.Infrastructure.Persistence.Services;

namespace RealEstateApp.Infrastructure.Persistence;

public static class ServiceRegistration
{
    public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        if (configuration.GetValue<bool>("UseInMemoryDatabase"))
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseInMemoryDatabase("RealEstateDb"));
        }
        else
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    m => m.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));
        }

        // Repositorio genérico (Issue #3)
        services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddTransient<IPropertyStatsService, PropertyStatsService>();

        services.AddTransient<IPropertyManagementService, PropertyManagementService>();
        services.AddTransient<IPropertyStatsService, PropertyStatsService>();
        services.AddTransient<ICatalogMaintenanceService, CatalogMaintenanceService>();
        services.AddTransient<IHomeService, HomeService>();

        services.AddTransient<IAgentDashboardService, AgentDashboardService>();
        services.AddTransient<IPropertyManagementService, PropertyManagementService>();
        services.AddTransient<IOfferService, OfferService>();
    }
    
}