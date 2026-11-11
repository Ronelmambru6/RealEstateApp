using Microsoft.AspNetCore.Identity;

namespace RealEstateApp.Infrastructure.Identity.Entities;

public class ApplicationUser : IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public string? Cedula { get; set; }
    public string? PhotoUrl { get; set; }
    public bool IsActive { get; set; } = false;
}