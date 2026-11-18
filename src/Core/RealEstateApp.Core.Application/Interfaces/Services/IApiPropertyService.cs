using RealEstateApp.Core.Application.Dtos.Property;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IApiPropertyService
{
    Task<List<PropertyDto>> GetAllAsync();
    Task<PropertyDto?> GetByIdAsync(int id);
    Task<PropertyDto?> GetByCodeAsync(string code);
    Task<List<PropertyDto>> GetByAgentIdAsync(string agentId);
}
