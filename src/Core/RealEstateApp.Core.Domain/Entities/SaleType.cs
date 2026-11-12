using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class SaleType : BaseEntity
{
    public required string Name { get; set; }
    public required string Description { get; set; }

    // Navegación
    public ICollection<Property> Properties { get; set; } = new List<Property>();
}