namespace RealEstateApp.Core.Application.ViewModels.Admin;

public class AgentListItemViewModel
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public int PropertiesCount { get; set; }
}