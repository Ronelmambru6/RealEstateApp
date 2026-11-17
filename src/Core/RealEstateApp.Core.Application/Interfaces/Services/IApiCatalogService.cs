using RealEstateApp.Core.Application.Dtos.Catalog;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IApiCatalogService
{
    // Tipos de Propiedades
    Task<List<CatalogItemDto>> GetAllPropertyTypesAsync();
    Task<CatalogItemDto?> GetPropertyTypeByIdAsync(int id);
    Task<CatalogItemDto> CreatePropertyTypeAsync(SaveCatalogItemRequest request);
    Task<UpdateCatalogItemResponse?> UpdatePropertyTypeAsync(int id, SaveCatalogItemRequest request);
    Task<bool> DeletePropertyTypeAsync(int id);

    // Tipos de Ventas
    Task<List<CatalogItemDto>> GetAllSaleTypesAsync();
    Task<CatalogItemDto?> GetSaleTypeByIdAsync(int id);
    Task<CatalogItemDto> CreateSaleTypeAsync(SaveCatalogItemRequest request);
    Task<UpdateCatalogItemResponse?> UpdateSaleTypeAsync(int id, SaveCatalogItemRequest request);
    Task<bool> DeleteSaleTypeAsync(int id);

    // Mejoras
    Task<List<CatalogItemDto>> GetAllImprovementsAsync();
    Task<CatalogItemDto?> GetImprovementByIdAsync(int id);
    Task<CatalogItemDto> CreateImprovementAsync(SaveCatalogItemRequest request);
    Task<UpdateCatalogItemResponse?> UpdateImprovementAsync(int id, SaveCatalogItemRequest request);
    Task<bool> DeleteImprovementAsync(int id);
}