using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class ChatMessage : BaseEntity
{
    public int PropertyId { get; set; }
    public Property? Property { get; set; }

    public required string ClientId { get; set; }     // Identifica el hilo del cliente con la propiedad
    public required string SenderId { get; set; }     // Quién envió este mensaje (ClientId o AgentId)
    public bool IsFromAgent { get; set; }             // true = Agente, false = Cliente
    public required string Content { get; set; }
}