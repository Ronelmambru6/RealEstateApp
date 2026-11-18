using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class ApiAgentService : IApiAgentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public ApiAgentService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<AgentDto>> GetAllAsync()
    {
        var agents = await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString());

        return agents
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(Map)
            .ToList();
    }

    public async Task<AgentDto?> GetByIdAsync(string id)
    {
        var agent = await FindAgentAsync(id);
        return agent is null ? null : Map(agent);
    }

    public async Task<bool> ChangeStatusAsync(string id, bool isActive)
    {
        var agent = await FindAgentAsync(id);
        if (agent is null) return false;

        agent.IsActive = isActive;
        var result = await _userManager.UpdateAsync(agent);
        return result.Succeeded;
    }

    private async Task<ApplicationUser?> FindAgentAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user is null || !await _userManager.IsInRoleAsync(user, Roles.Agente.ToString()))
        {
            return null;
        }

        return user;
    }

    private static AgentDto Map(ApplicationUser agent) => new()
    {
        Id = agent.Id,
        FirstName = agent.FirstName,
        LastName = agent.LastName,
        Email = agent.Email ?? string.Empty,
        PhoneNumber = agent.PhoneNumber,
        PhotoUrl = agent.PhotoUrl,
        IsActive = agent.IsActive
    };
}
