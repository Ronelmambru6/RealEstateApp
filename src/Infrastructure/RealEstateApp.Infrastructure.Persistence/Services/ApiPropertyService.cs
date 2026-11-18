using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class ApiPropertyService : IApiPropertyService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ApiPropertyService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<List<PropertyDto>> GetAllAsync()
    {
        var properties = await BaseQuery().OrderByDescending(p => p.CreatedAt).ToListAsync();
        return await MapAsync(properties);
    }

    public async Task<PropertyDto?> GetByIdAsync(int id)
    {
        var property = await BaseQuery().FirstOrDefaultAsync(p => p.Id == id);
        if (property is null) return null;

        return (await MapAsync(new List<Property> { property })).First();
    }

    public async Task<PropertyDto?> GetByCodeAsync(string code)
    {
        var property = await BaseQuery().FirstOrDefaultAsync(p => p.Code == code);
        if (property is null) return null;

        return (await MapAsync(new List<Property> { property })).First();
    }

    public async Task<List<PropertyDto>> GetByAgentIdAsync(string agentId)
    {
        var properties = await BaseQuery()
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

        return await MapAsync(properties);
    }

    private IQueryable<Property> BaseQuery()
    {
        return _context.Properties
            .AsNoTracking()
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Images)
            .Include(p => p.PropertyImprovements)
                .ThenInclude(pi => pi.Improvement);
    }

    private async Task<List<PropertyDto>> MapAsync(List<Property> properties)
    {
        var agentIds = properties.Select(p => p.AgentId).Distinct().ToList();
        var agents = await _userManager.Users
            .Where(u => agentIds.Contains(u.Id))
            .ToListAsync();
        var agentNames = agents.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        return properties.Select(p => new PropertyDto
        {
            Id = p.Id,
            Code = p.Code,
            PropertyType = p.PropertyType?.Name ?? string.Empty,
            SaleType = p.SaleType?.Name ?? string.Empty,
            Price = p.Price,
            SizeInMeters = p.SizeInMeters,
            Bedrooms = p.Bedrooms,
            Bathrooms = p.Bathrooms,
            Description = p.Description,
            Status = p.Status.ToString(),
            AgentId = p.AgentId,
            AgentName = agentNames.TryGetValue(p.AgentId, out var name) ? name : string.Empty,
            Images = p.Images.Select(i => i.ImageUrl).ToList(),
            Improvements = p.PropertyImprovements
                .Where(pi => pi.Improvement != null)
                .Select(pi => pi.Improvement!.Name)
                .ToList()
        }).ToList();
    }
}
