namespace RealEstateApp.Core.Application.ViewModels.Offer;

public class ClientPropertyOffersViewModel
{
    public int PropertyId { get; set; }
    public string PropertyCode { get; set; } = string.Empty;
    public double PropertyPrice { get; set; }
    public bool CanCreateOffer { get; set; }
    public string? BlockReason { get; set; }
    public List<OfferViewModel> Offers { get; set; } = new();
    public CreateOfferViewModel NewOffer { get; set; } = new();
}