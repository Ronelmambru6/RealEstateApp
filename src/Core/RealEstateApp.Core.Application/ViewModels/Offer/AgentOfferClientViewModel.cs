namespace RealEstateApp.Core.Application.ViewModels.Offer;

public class AgentOfferClientViewModel
{
    public string ClientId { get; set; } = string.Empty;
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public int TotalOffersCount { get; set; }
    public bool HasPendingOffer { get; set; }
    public decimal LatestOfferAmount { get; set; }
    public DateTime LatestOfferDate { get; set; }
}