namespace RealEstateApp.Core.Application.ViewModels.Chat;

public class ChatThreadViewModel
{
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public List<ChatMessageViewModel> Messages { get; set; } = new();
}
