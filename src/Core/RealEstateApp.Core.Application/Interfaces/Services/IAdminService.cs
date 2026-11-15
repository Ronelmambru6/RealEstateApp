using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardViewModel> GetDashboardStatsAsync();
    Task<List<AgentListItemViewModel>> GetAllAgentsAsync();
    Task<bool> ToggleAgentStatusAsync(string agentId);
    Task<bool> DeleteAgentWithCascadeAsync(string agentId);
}