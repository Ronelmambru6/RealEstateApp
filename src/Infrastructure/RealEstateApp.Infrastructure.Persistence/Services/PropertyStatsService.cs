using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class PropertyStatsService : IPropertyStatsService
{
    private readonly ApplicationDbContext _context;

    public PropertyStatsService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<(int available, int sold)> GetPropertyCountsAsync()
    {
        var available = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Available);
        var sold = await _context.Properties.CountAsync(p => p.Status == PropertyStatus.Sold);
        return (available, sold);
    }

    public async Task<Dictionary<string, int>> GetPropertyCountByAgentIdsAsync(IEnumerable<string> agentIds)
    {
        var ids = agentIds.ToList();
        return await _context.Properties
            .Where(p => ids.Contains(p.AgentId))
            .GroupBy(p => p.AgentId)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AgentId, x => x.Count);
    }

    public async Task DeletePropertiesByAgentIdAsync(string agentId)
    {
        var agentProperties = await _context.Properties
            .Where(p => p.AgentId == agentId)
            .ToListAsync();

        if (agentProperties.Count > 0)
        {
            _context.Properties.RemoveRange(agentProperties);
            await _context.SaveChangesAsync();
        }
    }
}