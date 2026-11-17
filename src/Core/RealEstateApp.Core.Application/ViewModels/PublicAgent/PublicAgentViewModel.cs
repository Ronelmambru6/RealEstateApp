namespace RealEstateApp.Core.Application.ViewModels.PublicAgent;

public class PublicAgentViewModel
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public string? PhotoUrl { get; set; }
}
