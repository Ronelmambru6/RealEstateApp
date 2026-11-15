using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropertyStatsService _propertyStatsService;

    public AdminService(
        UserManager<ApplicationUser> userManager,
        IPropertyStatsService propertyStatsService)
    {
        _userManager = userManager;
        _propertyStatsService = propertyStatsService;
    }

    public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
    {
        var (availableProps, soldProps) = await _propertyStatsService.GetPropertyCountsAsync();

        var agents = await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString());
        var clients = await _userManager.GetUsersInRoleAsync(Roles.Cliente.ToString());
        var developers = await _userManager.GetUsersInRoleAsync(Roles.Desarrollador.ToString());

        return new AdminDashboardViewModel
        {
            AvailablePropertiesCount = availableProps,
            SoldPropertiesCount = soldProps,
            ActiveAgentsCount = agents.Count(u => u.IsActive),
            InactiveAgentsCount = agents.Count(u => !u.IsActive),
            ActiveClientsCount = clients.Count(u => u.IsActive),
            InactiveClientsCount = clients.Count(u => !u.IsActive),
            ActiveDevelopersCount = developers.Count(u => u.IsActive),
            InactiveDevelopersCount = developers.Count(u => !u.IsActive)
        };
    }

    public async Task<List<AgentListItemViewModel>> GetAllAgentsAsync()
    {
        var agents = await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString());
        var agentIds = agents.Select(a => a.Id);
        var propCounts = await _propertyStatsService.GetPropertyCountByAgentIdsAsync(agentIds);

        return agents
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(a => new AgentListItemViewModel
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email ?? string.Empty,
                PhoneNumber = a.PhoneNumber,
                IsActive = a.IsActive,
                PropertiesCount = propCounts.TryGetValue(a.Id, out var count) ? count : 0
            })
            .ToList();
    }

    public async Task<bool> ToggleAgentStatusAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent == null) return false;

        agent.IsActive = !agent.IsActive;
        var result = await _userManager.UpdateAsync(agent);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAgentWithCascadeAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent == null) return false;

        // 1. Eliminar en cascada todas sus propiedades y dependencias (ofertas, imágenes, mejoras, chats, favoritos)
        await _propertyStatsService.DeletePropertiesByAgentIdAsync(agentId);

        // 2. Eliminar el usuario Agente en Identity
        var result = await _userManager.DeleteAsync(agent);
        return result.Succeeded;
    }
}