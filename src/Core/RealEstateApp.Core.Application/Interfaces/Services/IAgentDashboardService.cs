using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAgentDashboardService
{
    Task<List<AgentPropertyViewModel>> GetPropertiesByAgentIdAsync(string agentId);
    Task<AgentProfileViewModel?> GetAgentProfileByIdAsync(string agentId);
    Task<bool> UpdateAgentProfileAsync(AgentProfileViewModel vm);
}