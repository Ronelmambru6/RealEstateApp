using RealEstateApp.Core.Domain.Common;
using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Domain.Entities;

public class Offer : BaseEntity
{
    public int PropertyId { get; set; }
    public Property? Property { get; set; }

    public required string ClientId { get; set; } // Usuario Cliente en Identity
    public decimal Amount { get; set; }
    public OfferStatus Status { get; set; } = OfferStatus.Pending;
}