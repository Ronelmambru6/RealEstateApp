using RealEstateApp.Core.Domain.Enums;

namespace RealEstateApp.Core.Application.ViewModels.Offer;

public class OfferViewModel
{
    public int Id { get; set; }
    public int PropertyId { get; set; }
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime CreatedAt { get; set; }
    public OfferStatus Status { get; set; }
}