using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IChatService
{
    /// <summary>Conversación de un cliente con el agente de una propiedad. Null si la propiedad no existe.</summary>
    Task<ChatThreadViewModel?> GetThreadAsync(int propertyId, string clientId);

    /// <summary>Conversaciones (cliente + propiedad) de las propiedades de un agente, la más reciente primero.</summary>
    Task<List<ChatConversationViewModel>> GetAgentConversationsAsync(string agentId);

    Task<bool> SendClientMessageAsync(string clientId, int propertyId, string content);

    /// <summary>El agente solo puede responder en conversaciones de sus propiedades que ya iniciaron los clientes.</summary>
    Task<bool> SendAgentMessageAsync(string agentId, int propertyId, string clientId, string content);
}
