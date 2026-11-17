using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IPropertyManagementService
{
    Task<List<PropertyViewModel>> GetPropertiesByAgentAsync(string agentId);
    Task<SavePropertyViewModel> PrepareSaveViewModelAsync(SavePropertyViewModel? vm = null);
    Task<SavePropertyViewModel?> GetPropertyForEditAsync(int id, string agentId);
    Task<(bool Succeeded, string? ErrorMessage)> CreatePropertyAsync(SavePropertyViewModel vm, string agentId);
    Task<(bool Succeeded, string? ErrorMessage)> UpdatePropertyAsync(SavePropertyViewModel vm, string agentId);
    Task<bool> DeletePropertyAsync(int id, string agentId);
}