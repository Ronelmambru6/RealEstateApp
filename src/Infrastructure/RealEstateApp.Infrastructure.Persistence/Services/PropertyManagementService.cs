using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Property;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class PropertyManagementService : IPropertyManagementService
{
    private readonly ApplicationDbContext _context;

    public PropertyManagementService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<PropertyViewModel>> GetPropertiesByAgentAsync(string agentId)
    {
        return await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Images)
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new PropertyViewModel
            {
                Id = p.Id,
                Code = p.Code,
                PropertyTypeName = p.PropertyType!.Name,
                SaleTypeName = p.SaleType!.Name,
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                SizeInMeters = p.SizeInMeters,
                MainImageUrl = p.Images.Select(img => img.ImageUrl).FirstOrDefault(),
                IsAvailable = p.Status == PropertyStatus.Available
            })
            .ToListAsync();
    }

    public async Task<SavePropertyViewModel> PrepareSaveViewModelAsync(SavePropertyViewModel? vm = null)
    {
        var propertyTypes = await _context.PropertyTypes.AsNoTracking().ToListAsync();
        var saleTypes = await _context.SaleTypes.AsNoTracking().ToListAsync();
        var improvements = await _context.Improvements.AsNoTracking().ToListAsync();

        vm ??= new SavePropertyViewModel();

        vm.PropertyTypes = new SelectList(propertyTypes, "Id", "Name", vm.PropertyTypeId);
        vm.SaleTypes = new SelectList(saleTypes, "Id", "Name", vm.SaleTypeId);
        vm.ImprovementsList = improvements.Select(imp => new SelectListItem
        {
            Value = imp.Id.ToString(),
            Text = imp.Name,
            Selected = vm.ImprovementIds.Contains(imp.Id)
        }).ToList();

        return vm;
    }

    public async Task<SavePropertyViewModel?> GetPropertyForEditAsync(int id, string agentId)
    {
        var property = await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements)
            .FirstOrDefaultAsync(p => p.Id == id && p.AgentId == agentId);

        if (property == null) return null;

        var vm = new SavePropertyViewModel
        {
            Id = property.Id,
            Price = property.Price,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            SizeInMeters = property.SizeInMeters,
            Description = property.Description,
            PropertyTypeId = property.PropertyTypeId,
            SaleTypeId = property.SaleTypeId,
            ImprovementIds = property.PropertyImprovements.Select(pi => pi.ImprovementId).ToList(),
            ExistingImageUrls = property.Images.Select(img => img.ImageUrl).ToList()
        };

        return await PrepareSaveViewModelAsync(vm);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> CreatePropertyAsync(SavePropertyViewModel vm, string agentId)
    {
        // 1. Validar cantidad de imágenes (1 a 4 obligatorias al crear)
        if (vm.ImageFiles == null || vm.ImageFiles.Count == 0 || vm.ImageFiles.Count > 4)
            return (false, "Debe cargar entre 1 y 4 imágenes para la propiedad.");

        // 2. Validar existencia de tipos y mejoras
        bool propertyTypeExists = await _context.PropertyTypes.AnyAsync(pt => pt.Id == vm.PropertyTypeId);
        bool saleTypeExists = await _context.SaleTypes.AnyAsync(st => st.Id == vm.SaleTypeId);
        if (!propertyTypeExists || !saleTypeExists)
            return (false, "El tipo de propiedad o el tipo de venta seleccionado no es válido.");

        // 3. Generar código único de 6 dígitos
        string uniqueCode = await GenerateUniquePropertyCodeAsync();

        var property = new Property
        {
            Code = uniqueCode,
            Price = vm.Price,
            Bedrooms = vm.Bedrooms,
            Bathrooms = vm.Bathrooms,
            SizeInMeters = vm.SizeInMeters,
            Description = vm.Description.Trim(),
            PropertyTypeId = vm.PropertyTypeId,
            SaleTypeId = vm.SaleTypeId,
            AgentId = agentId,
            Status = PropertyStatus.Available,
            CreatedAt = DateTime.UtcNow
        };

        // 4. Guardar imágenes en disco
        string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/properties");
        if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

        foreach (var file in vm.ImageFiles)
        {
            if (file.Length > 0)
            {
                string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(fileStream);
                }

                property.Images.Add(new PropertyImage
                {
                    ImageUrl = $"/images/properties/{uniqueFileName}"
                });
            }
        }

        // 5. Agregar mejoras seleccionadas
        if (vm.ImprovementIds != null && vm.ImprovementIds.Count > 0)
        {
            foreach (int impId in vm.ImprovementIds)
            {
                property.PropertyImprovements.Add(new PropertyImprovement
                {
                    ImprovementId = impId
                });
            }
        }

        _context.Properties.Add(property);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> UpdatePropertyAsync(SavePropertyViewModel vm, string agentId)
    {
        var property = await _context.Properties
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements)
            .FirstOrDefaultAsync(p => p.Id == vm.Id && p.AgentId == agentId);

        if (property == null) return (false, "La propiedad no existe o no pertenece al agente.");

        property.Price = vm.Price;
        property.Bedrooms = vm.Bedrooms;
        property.Bathrooms = vm.Bathrooms;
        property.SizeInMeters = vm.SizeInMeters;
        property.Description = vm.Description.Trim();
        property.PropertyTypeId = vm.PropertyTypeId;
        property.SaleTypeId = vm.SaleTypeId;

        // Si se adjuntaron nuevas imágenes (opcional en edición, máx 4 en total o reemplazo)
        if (vm.ImageFiles != null && vm.ImageFiles.Count > 0)
        {
            if (property.Images.Count + vm.ImageFiles.Count > 4)
                return (false, "Una propiedad no puede tener más de 4 imágenes en total.");

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/properties");
            foreach (var file in vm.ImageFiles)
            {
                if (file.Length > 0)
                {
                    string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(fileStream);
                    }

                    property.Images.Add(new PropertyImage
                    {
                        ImageUrl = $"/images/properties/{uniqueFileName}"
                    });
                }
            }
        }

        // Actualizar mejoras
        property.PropertyImprovements.Clear();
        if (vm.ImprovementIds != null)
        {
            foreach (int impId in vm.ImprovementIds)
            {
                property.PropertyImprovements.Add(new PropertyImprovement
                {
                    ImprovementId = impId
                });
            }
        }

        _context.Properties.Update(property);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<bool> DeletePropertyAsync(int id, string agentId)
    {
        var property = await _context.Properties
            .FirstOrDefaultAsync(p => p.Id == id && p.AgentId == agentId);

        if (property == null) return false;

        _context.Properties.Remove(property);
        await _context.SaveChangesAsync();
        return true;
    }

    private async Task<string> GenerateUniquePropertyCodeAsync()
    {
        var random = new Random();
        string code;
        bool exists;

        do
        {
            code = random.Next(100000, 999999).ToString();
            exists = await _context.Properties.AnyAsync(p => p.Code == code);
        } while (exists);

        return code;
    }
}