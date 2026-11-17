using RealEstateApp.Core.Application.ViewModels.Offer;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IOfferService
{
    // Operaciones para el Cliente
    Task<ClientPropertyOffersViewModel?> GetClientOffersAsync(int propertyId, string clientId);
    Task<(bool Succeeded, string? ErrorMessage)> CreateOfferAsync(CreateOfferViewModel vm, string clientId);

    // Operaciones para el Agente
    Task<List<AgentOfferClientViewModel>> GetClientsWithOffersByPropertyAsync(int propertyId, string agentId);
    Task<List<OfferViewModel>> GetOffersByClientAndPropertyAsync(int propertyId, string clientId, string agentId);
    Task<(bool Succeeded, string? ErrorMessage)> RespondToOfferAsync(int offerId, bool accept, string agentId);
}