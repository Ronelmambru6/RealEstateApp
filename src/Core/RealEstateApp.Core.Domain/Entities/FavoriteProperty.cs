using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Domain.Entities;

public class FavoriteProperty : BaseEntity
{
    public int PropertyId { get; set; }
    public Property? Property { get; set; }

    public required string ClientId { get; set; }
}