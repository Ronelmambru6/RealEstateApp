namespace RealEstateApp.Core.Application.ViewModels.Home;

public class HomePropertyViewModel
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string PropertyTypeName { get; set; }
    public required string SaleTypeName { get; set; }
    public double Price { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public double Fläche { get; set; } // O metros cuadrados (SquareMeters)
    public string? MainImageUrl { get; set; }
    public DateTime Created { get; set; }
}