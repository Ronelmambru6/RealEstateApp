namespace RealEstateApp.Core.Application.ViewModels.Agent;

public class AgentPropertyViewModel
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string PropertyTypeName { get; set; }
    public required string SaleTypeName { get; set; }
    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double SizeInMeters { get; set; }
    public string? MainImageUrl { get; set; }
    public bool IsAvailable { get; set; } // True = Disponible, False = Vendida
}