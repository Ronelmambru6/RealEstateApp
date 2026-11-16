namespace RealEstateApp.Core.Application.ViewModels.Admin;

public class SystemUserViewModel
{
    public required string Id { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string UserName { get; set; }
    public required string Cedula { get; set; }
    public required string Email { get; set; }
    public bool IsActive { get; set; }
    public required string Role { get; set; }
}