using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class ApiCatalogService : IApiCatalogService
{
    private readonly ApplicationDbContext _context;

    public ApiCatalogService(ApplicationDbContext context)
    {
        _context = context;
    }

    // ==========================================
    // TIPOS DE PROPIEDADES
    // ==========================================

    public async Task<List<CatalogItemDto>> GetAllPropertyTypesAsync()
    {
        return await _context.PropertyTypes
            .AsNoTracking()
            .Select(pt => new CatalogItemDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description
            })
            .ToListAsync();
    }

    public async Task<CatalogItemDto?> GetPropertyTypeByIdAsync(int id)
    {
        return await _context.PropertyTypes
            .AsNoTracking()
            .Where(pt => pt.Id == id)
            .Select(pt => new CatalogItemDto
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogItemDto> CreatePropertyTypeAsync(SaveCatalogItemRequest request)
    {
        var entity = new PropertyType
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        _context.PropertyTypes.Add(entity);
        await _context.SaveChangesAsync();

        return new CatalogItemDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<UpdateCatalogItemResponse?> UpdatePropertyTypeAsync(int id, SaveCatalogItemRequest request)
    {
        var entity = await _context.PropertyTypes.FindAsync(id);
        if (entity == null) return null;

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();

        _context.PropertyTypes.Update(entity);
        await _context.SaveChangesAsync();

        return new UpdateCatalogItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<bool> DeletePropertyTypeAsync(int id)
    {
        var entity = await _context.PropertyTypes
            .Include(pt => pt.Properties)
            .FirstOrDefaultAsync(pt => pt.Id == id);

        if (entity == null) return false;

        // Regla de borrado: eliminar propiedades asociadas a este tipo
        if (entity.Properties.Any())
        {
            _context.Properties.RemoveRange(entity.Properties);
        }

        _context.PropertyTypes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ==========================================
    // TIPOS DE VENTAS
    // ==========================================

    public async Task<List<CatalogItemDto>> GetAllSaleTypesAsync()
    {
        return await _context.SaleTypes
            .AsNoTracking()
            .Select(st => new CatalogItemDto
            {
                Id = st.Id,
                Name = st.Name,
                Description = st.Description
            })
            .ToListAsync();
    }

    public async Task<CatalogItemDto?> GetSaleTypeByIdAsync(int id)
    {
        return await _context.SaleTypes
            .AsNoTracking()
            .Where(st => st.Id == id)
            .Select(st => new CatalogItemDto
            {
                Id = st.Id,
                Name = st.Name,
                Description = st.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogItemDto> CreateSaleTypeAsync(SaveCatalogItemRequest request)
    {
        var entity = new SaleType
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        _context.SaleTypes.Add(entity);
        await _context.SaveChangesAsync();

        return new CatalogItemDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<UpdateCatalogItemResponse?> UpdateSaleTypeAsync(int id, SaveCatalogItemRequest request)
    {
        var entity = await _context.SaleTypes.FindAsync(id);
        if (entity == null) return null;

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();

        _context.SaleTypes.Update(entity);
        await _context.SaveChangesAsync();

        return new UpdateCatalogItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<bool> DeleteSaleTypeAsync(int id)
    {
        var entity = await _context.SaleTypes
            .Include(st => st.Properties)
            .FirstOrDefaultAsync(st => st.Id == id);

        if (entity == null) return false;

        // Regla de borrado: eliminar propiedades asociadas a este tipo de venta
        if (entity.Properties.Any())
        {
            _context.Properties.RemoveRange(entity.Properties);
        }

        _context.SaleTypes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // ==========================================
    // MEJORAS
    // ==========================================

    public async Task<List<CatalogItemDto>> GetAllImprovementsAsync()
    {
        return await _context.Improvements
            .AsNoTracking()
            .Select(imp => new CatalogItemDto
            {
                Id = imp.Id,
                Name = imp.Name,
                Description = imp.Description
            })
            .ToListAsync();
    }

    public async Task<CatalogItemDto?> GetImprovementByIdAsync(int id)
    {
        return await _context.Improvements
            .AsNoTracking()
            .Where(imp => imp.Id == id)
            .Select(imp => new CatalogItemDto
            {
                Id = imp.Id,
                Name = imp.Name,
                Description = imp.Description
            })
            .FirstOrDefaultAsync();
    }

    public async Task<CatalogItemDto> CreateImprovementAsync(SaveCatalogItemRequest request)
    {
        var entity = new Improvement
        {
            Name = request.Name.Trim(),
            Description = request.Description.Trim()
        };

        _context.Improvements.Add(entity);
        await _context.SaveChangesAsync();

        return new CatalogItemDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<UpdateCatalogItemResponse?> UpdateImprovementAsync(int id, SaveCatalogItemRequest request)
    {
        var entity = await _context.Improvements.FindAsync(id);
        if (entity == null) return null;

        entity.Name = request.Name.Trim();
        entity.Description = request.Description.Trim();

        _context.Improvements.Update(entity);
        await _context.SaveChangesAsync();

        return new UpdateCatalogItemResponse
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description
        };
    }

    public async Task<bool> DeleteImprovementAsync(int id)
    {
        var entity = await _context.Improvements
            .Include(imp => imp.PropertyImprovements)
            .FirstOrDefaultAsync(imp => imp.Id == id);

        if (entity == null) return false;

        // Regla de borrado: desvincular de las propiedades sin borrar las propiedades
        if (entity.PropertyImprovements.Any())
        {
            _context.PropertyImprovements.RemoveRange(entity.PropertyImprovements);
        }

        _context.Improvements.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}