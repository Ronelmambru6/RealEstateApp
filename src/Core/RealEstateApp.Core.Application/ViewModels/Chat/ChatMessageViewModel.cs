namespace RealEstateApp.Core.Application.ViewModels.Chat;

public class ChatMessageViewModel
{
    public string Content { get; set; } = string.Empty;
    public bool IsFromAgent { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
