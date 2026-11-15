namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPropertyStatsService
{
    Task<(int available, int sold)> GetPropertyCountsAsync();
    Task<Dictionary<string, int>> GetPropertyCountByAgentIdsAsync(IEnumerable<string> agentIds);
    Task DeletePropertiesByAgentIdAsync(string agentId);
}