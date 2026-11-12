using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class Improvement : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Navegación
    public ICollection<PropertyImprovement> PropertyImprovements { get; set; } = new List<PropertyImprovement>();
}