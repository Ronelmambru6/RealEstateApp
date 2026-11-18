using RealEstateApp.Core.Application.Dtos.Agent;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IApiAgentService
{
    Task<List<AgentDto>> GetAllAsync();
    Task<AgentDto?> GetByIdAsync(string id);

    /// <summary>Cambia el estado del agente. Devuelve false si el agente no existe.</summary>
    Task<bool> ChangeStatusAsync(string id, bool isActive);
}
