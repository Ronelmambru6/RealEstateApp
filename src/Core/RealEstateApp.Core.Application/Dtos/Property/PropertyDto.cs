namespace RealEstateApp.Core.Application.Dtos.Property;

public class PropertyDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string PropertyType { get; set; } = string.Empty;
    public string SaleType { get; set; } = string.Empty;
    public double Price { get; set; }
    public double SizeInMeters { get; set; }
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AgentId { get; set; } = string.Empty;
    public string AgentName { get; set; } = string.Empty;
    public List<string> Images { get; set; } = new();
    public List<string> Improvements { get; set; } = new();
}
