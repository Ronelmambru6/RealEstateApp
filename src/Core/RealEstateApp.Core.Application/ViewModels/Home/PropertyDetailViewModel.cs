namespace RealEstateApp.Core.Application.ViewModels.Home;

public class PropertyDetailViewModel
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string PropertyTypeName { get; set; }
    public required string SaleTypeName { get; set; }
    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double SquareMeters { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateTime Created { get; set; }

    // Slider de imágenes
    public List<string> ImageUrls { get; set; } = new();

    // Mejoras de la propiedad
    public List<string> Improvements { get; set; } = new();

    // Datos del agente
    public required string AgentId { get; set; }
    public required string AgentFirstName { get; set; }
    public required string AgentLastName { get; set; }
    public required string AgentEmail { get; set; }
    public string? AgentPhoneNumber { get; set; }
    public string? AgentPhotoUrl { get; set; }
}