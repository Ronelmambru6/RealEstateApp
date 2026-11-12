using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class Property : BaseEntity
{
    public required string Code { get; set; } // Código único de 6 dígitos
    public decimal Price { get; set; }        // Expresado en DOP
    public double SizeInMeters { get; set; }  // Tamaño en metros
    public int Bedrooms { get; set; }
    public int Bathrooms { get; set; }
    public required string Description { get; set; }
    public PropertyStatus Status { get; set; } = PropertyStatus.Available;

    // Claves foráneas
    public int PropertyTypeId { get; set; }
    public PropertyType? PropertyType { get; set; }

    public int SaleTypeId { get; set; }
    public SaleType? SaleType { get; set; }

    public required string AgentId { get; set; } // Referencia al usuario Agente en Identity

    // Navegación
    public ICollection<PropertyImage> Images { get; set; } = new List<PropertyImage>();
    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
    public ICollection<ChatMessage> ChatMessages { get; set; } = new List<ChatMessage>();
    public ICollection<FavoriteProperty> FavoriteProperties { get; set; } = new List<FavoriteProperty>();
}