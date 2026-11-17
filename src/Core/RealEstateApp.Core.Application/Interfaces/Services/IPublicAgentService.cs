using RealEstateApp.Core.Application.ViewModels.PublicAgent;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPublicAgentService
{
    /// <summary>Agentes activos ordenados alfabéticamente, con búsqueda opcional por nombre o apellido.</summary>
    Task<List<PublicAgentViewModel>> GetActiveAgentsAsync(string? search);

    /// <summary>Devuelve el agente solo si existe y está activo.</summary>
    Task<PublicAgentViewModel?> GetActiveAgentByIdAsync(string agentId);
}
