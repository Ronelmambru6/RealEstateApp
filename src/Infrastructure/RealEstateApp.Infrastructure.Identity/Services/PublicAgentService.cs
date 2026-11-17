using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.PublicAgent;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class PublicAgentService : IPublicAgentService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public PublicAgentService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<List<PublicAgentViewModel>> GetActiveAgentsAsync(string? search)
    {
        var agents = (await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString()))
            .Where(a => a.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            agents = agents.Where(a =>
                a.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                a.LastName.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                $"{a.FirstName} {a.LastName}".Contains(term, StringComparison.OrdinalIgnoreCase));
        }

        return agents
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(Map)
            .ToList();
    }

    public async Task<PublicAgentViewModel?> GetActiveAgentByIdAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent is null || !agent.IsActive || !await _userManager.IsInRoleAsync(agent, Roles.Agente.ToString()))
        {
            return null;
        }

        return Map(agent);
    }

    private static PublicAgentViewModel Map(ApplicationUser agent) => new()
    {
        Id = agent.Id,
        FirstName = agent.FirstName,
        LastName = agent.LastName,
        Email = agent.Email ?? string.Empty,
        PhoneNumber = agent.PhoneNumber,
        PhotoUrl = agent.PhotoUrl
    };
}
