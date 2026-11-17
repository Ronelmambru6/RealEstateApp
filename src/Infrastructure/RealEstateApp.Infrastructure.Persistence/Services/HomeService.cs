using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Home;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class HomeService : IHomeService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public HomeService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<HomePropertyViewModel>> GetFilteredPropertiesAsync(FilterPropertyViewModel filter)
    {
        var query = _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Images)
            .Where(p => p.Status == PropertyStatus.Available) // Solo propiedades Disponibles
            .AsQueryable();

        // Filtro por Agente (módulo público de agentes)
        if (!string.IsNullOrWhiteSpace(filter.AgentId))
        {
            query = query.Where(p => p.AgentId == filter.AgentId);
        }

        // Filtro por favoritas de un cliente (Mis Propiedades)
        if (!string.IsNullOrWhiteSpace(filter.FavoriteOfClientId))
        {
            query = query.Where(p => p.FavoriteProperties.Any(f => f.ClientId == filter.FavoriteOfClientId));
        }

        // Búsqueda por código exacto o parcial
        if (!string.IsNullOrWhiteSpace(filter.PropertyCode))
        {
            query = query.Where(p => p.Code.Contains(filter.PropertyCode.Trim()));
        }

        // Filtro por Tipo de Propiedad
        if (filter.PropertyTypeId.HasValue && filter.PropertyTypeId.Value > 0)
        {
            query = query.Where(p => p.PropertyTypeId == filter.PropertyTypeId.Value);
        }

        // Filtro por Tipo de Venta
        if (filter.SaleTypeId.HasValue && filter.SaleTypeId.Value > 0)
        {
            query = query.Where(p => p.SaleTypeId == filter.SaleTypeId.Value);
        }

        // Filtro por Habitaciones
        if (filter.Bedrooms.HasValue && filter.Bedrooms.Value > 0)
        {
            query = query.Where(p => p.Bedrooms >= filter.Bedrooms.Value);
        }

        // Filtro por Baños
        if (filter.Bathrooms.HasValue && filter.Bathrooms.Value > 0)
        {
            query = query.Where(p => p.Bathrooms >= filter.Bathrooms.Value);
        }

        // Filtro por Precio Mínimo
        if (filter.MinPrice.HasValue && filter.MinPrice.Value > 0)
        {
            query = query.Where(p => p.Price >= (double)filter.MinPrice.Value);
        }

        // Filtro por Precio Máximo
        if (filter.MaxPrice.HasValue && filter.MaxPrice.Value > 0)
        {
            query = query.Where(p => p.Price <= (double)filter.MaxPrice.Value);
        }

        var properties = await query
            .OrderByDescending(p => p.CreatedAt) // Más reciente primero
            .Select(p => new HomePropertyViewModel
            {
                Id = p.Id,
                Code = p.Code,
                PropertyTypeName = p.PropertyType!.Name,
                SaleTypeName = p.SaleType!.Name,
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                Fläche = p.SizeInMeters,
                MainImageUrl = p.Images.Select(img => img.ImageUrl).FirstOrDefault(),
                Created = p.CreatedAt
            })
            .ToListAsync();

        return properties;
    }

    public async Task<PropertyDetailViewModel?> GetPropertyDetailByIdAsync(int id)
    {
        var property = await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements)
                .ThenInclude(pi => pi.Improvement)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (property == null) return null;

        // Obtener datos del agente asociado a la propiedad
        var agent = await _userManager.FindByIdAsync(property.AgentId);

        return new PropertyDetailViewModel
        {
            Id = property.Id,
            Code = property.Code,
            PropertyTypeName = property.PropertyType!.Name,
            SaleTypeName = property.SaleType!.Name,
            Price = property.Price,
            Bedrooms = property.Bedrooms,
            Bathrooms = property.Bathrooms,
            SquareMeters = property.SizeInMeters,
            Description = property.Description,
            Created = property.CreatedAt,
            ImageUrls = property.Images.Select(img => img.ImageUrl).ToList(),
            Improvements = property.PropertyImprovements.Select(pi => pi.Improvement!.Name).ToList(),
            AgentId = property.AgentId,
            AgentFirstName = agent?.FirstName ?? "Agente",
            AgentLastName = agent?.LastName ?? "Inmobiliario",
            AgentEmail = agent?.Email ?? string.Empty,
            AgentPhoneNumber = agent?.PhoneNumber,
            AgentPhotoUrl = agent?.PhotoUrl
        };
    }
}