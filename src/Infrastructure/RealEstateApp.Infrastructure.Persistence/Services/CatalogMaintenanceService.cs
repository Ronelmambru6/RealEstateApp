using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class CatalogMaintenanceService : ICatalogMaintenanceService
{
    private readonly ApplicationDbContext _context;

    public CatalogMaintenanceService(ApplicationDbContext context)
    {
        _context = context;
    }

    // =========================================================================
    // TIPOS DE PROPIEDADES (PropertyTypes)
    // =========================================================================

    public async Task<List<CatalogItemViewModel>> GetAllPropertyTypesAsync()
    {
        return await _context.PropertyTypes
            .AsNoTracking()
            .OrderBy(pt => pt.Name)
            .Select(pt => new CatalogItemViewModel
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                PropertiesCount = pt.Properties.Count
            })
            .ToListAsync();
    }

    public async Task<CatalogItemViewModel?> GetPropertyTypeByIdAsync(int id)
    {
        return await _context.PropertyTypes
            .AsNoTracking()
            .Where(pt => pt.Id == id)
            .Select(pt => new CatalogItemViewModel
            {
                Id = pt.Id,
                Name = pt.Name,
                Description = pt.Description,
                PropertiesCount = pt.Properties.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreatePropertyTypeAsync(SaveCatalogItemViewModel vm)
    {
        var entity = new PropertyType
        {
            Name = vm.Name.Trim(),
            Description = vm.Description.Trim()
        };

        _context.PropertyTypes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdatePropertyTypeAsync(SaveCatalogItemViewModel vm)
    {
        var entity = await _context.PropertyTypes.FindAsync(vm.Id);
        if (entity == null) return false;

        entity.Name = vm.Name.Trim();
        entity.Description = vm.Description.Trim();

        _context.PropertyTypes.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeletePropertyTypeWithCascadeAsync(int id)
    {
        var entity = await _context.PropertyTypes
            .Include(pt => pt.Properties)
            .FirstOrDefaultAsync(pt => pt.Id == id);

        if (entity == null) return false;

        if (entity.Properties.Count > 0)
        {
            _context.Properties.RemoveRange(entity.Properties);
        }

        _context.PropertyTypes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    // =========================================================================
    // TIPOS DE VENTAS (SaleTypes)
    // =========================================================================

    public async Task<List<CatalogItemViewModel>> GetAllSaleTypesAsync()
    {
        return await _context.SaleTypes
            .AsNoTracking()
            .OrderBy(st => st.Name)
            .Select(st => new CatalogItemViewModel
            {
                Id = st.Id,
                Name = st.Name,
                Description = st.Description,
                PropertiesCount = st.Properties.Count
            })
            .ToListAsync();
    }

    public async Task<CatalogItemViewModel?> GetSaleTypeByIdAsync(int id)
    {
        return await _context.SaleTypes
            .AsNoTracking()
            .Where(st => st.Id == id)
            .Select(st => new CatalogItemViewModel
            {
                Id = st.Id,
                Name = st.Name,
                Description = st.Description,
                PropertiesCount = st.Properties.Count
            })
            .FirstOrDefaultAsync();
    }

    public async Task CreateSaleTypeAsync(SaveCatalogItemViewModel vm)
    {
        var entity = new SaleType
        {
            Name = vm.Name.Trim(),
            Description = vm.Description.Trim()
        };

        _context.SaleTypes.Add(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> UpdateSaleTypeAsync(SaveCatalogItemViewModel vm)
    {
        var entity = await _context.SaleTypes.FindAsync(vm.Id);
        if (entity == null) return false;

        entity.Name = vm.Name.Trim();
        entity.Description = vm.Description.Trim();

        _context.SaleTypes.Update(entity);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteSaleTypeWithCascadeAsync(int id)
    {
        var entity = await _context.SaleTypes
            .Include(st => st.Properties)
            .FirstOrDefaultAsync(st => st.Id == id);

        if (entity == null) return false;

        if (entity.Properties.Count > 0)
        {
            _context.Properties.RemoveRange(entity.Properties);
        }

        _context.SaleTypes.Remove(entity);
        await _context.SaveChangesAsync();
        return true;
    }
}