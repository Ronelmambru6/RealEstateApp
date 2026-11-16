using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAdminService
{
    Task<AdminDashboardViewModel> GetDashboardStatsAsync();
    Task<List<AgentListItemViewModel>> GetAllAgentsAsync();
    Task<bool> ToggleAgentStatusAsync(string agentId);
    Task<bool> DeleteAgentWithCascadeAsync(string agentId);

    Task<List<SystemUserViewModel>> GetSystemUsersByRoleAsync(Roles role);
    Task<EditSystemUserViewModel?> GetSystemUserForEditAsync(string id);
    Task<(bool Succeeded, string? ErrorMessage)> CreateSystemUserAsync(CreateSystemUserViewModel vm, Roles role);
    Task<(bool Succeeded, string? ErrorMessage)> UpdateSystemUserAsync(EditSystemUserViewModel vm, string? currentAdminId);
    Task<(bool Succeeded, string? ErrorMessage)> ToggleSystemUserStatusAsync(string userId, string? currentAdminId);
}