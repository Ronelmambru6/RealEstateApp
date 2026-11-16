using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface ICatalogMaintenanceService
{
    // Tipos de Propiedades
    Task<List<CatalogItemViewModel>> GetAllPropertyTypesAsync();
    Task<CatalogItemViewModel?> GetPropertyTypeByIdAsync(int id);
    Task CreatePropertyTypeAsync(SaveCatalogItemViewModel vm);
    Task<bool> UpdatePropertyTypeAsync(SaveCatalogItemViewModel vm);
    Task<bool> DeletePropertyTypeWithCascadeAsync(int id);

    // Tipos de Ventas
    Task<List<CatalogItemViewModel>> GetAllSaleTypesAsync();
    Task<CatalogItemViewModel?> GetSaleTypeByIdAsync(int id);
    Task CreateSaleTypeAsync(SaveCatalogItemViewModel vm);
    Task<bool> UpdateSaleTypeAsync(SaveCatalogItemViewModel vm);
    Task<bool> DeleteSaleTypeWithCascadeAsync(int id);
}