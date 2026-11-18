namespace RealEstateApp.Core.Application.ViewModels.Chat;

public class ChatConversationViewModel
{
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string LastMessage { get; set; } = string.Empty;
    public DateTime LastMessageAt { get; set; }
    public bool LastMessageFromAgent { get; set; }
    public int MessageCount { get; set; }
}
