using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class FavoriteService : IFavoriteService
{
    private readonly ApplicationDbContext _context;

    public FavoriteService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<HashSet<int>> GetFavoritePropertyIdsAsync(string clientId)
    {
        var ids = await _context.FavoriteProperties
            .Where(f => f.ClientId == clientId)
            .Select(f => f.PropertyId)
            .ToListAsync();

        return ids.ToHashSet();
    }

    public async Task<bool> ToggleFavoriteAsync(string clientId, int propertyId)
    {
        var existing = await _context.FavoriteProperties
            .FirstOrDefaultAsync(f => f.ClientId == clientId && f.PropertyId == propertyId);

        if (existing is not null)
        {
            _context.FavoriteProperties.Remove(existing);
            await _context.SaveChangesAsync();
            return false;
        }

        // Solo se pueden marcar propiedades que existan y estén disponibles.
        var isAvailable = await _context.Properties
            .AnyAsync(p => p.Id == propertyId && p.Status == PropertyStatus.Available);
        if (!isAvailable)
        {
            return false;
        }

        _context.FavoriteProperties.Add(new FavoriteProperty { ClientId = clientId, PropertyId = propertyId });
        await _context.SaveChangesAsync();
        return true;
    }
}
