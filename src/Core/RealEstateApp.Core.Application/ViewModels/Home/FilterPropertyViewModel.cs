using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateApp.Core.Application.ViewModels.Home;

public class FilterPropertyViewModel
{
    public string? PropertyCode { get; set; }
    public int? PropertyTypeId { get; set; }
    public int? SaleTypeId { get; set; }
    public int? Bedrooms { get; set; }
    public int? Bathrooms { get; set; }
    public double? MinPrice { get; set; }
    public double? MaxPrice { get; set; }
    public string? FavoriteOfClientId { get; set; } // Opcional: solo propiedades favoritas de ese cliente
    public HashSet<int> FavoriteIds { get; set; } = new(); // Ids favoritos del cliente actual (para marcar el corazón)
    public string? AgentId { get; set; } // Opcional: limita el listado a las propiedades de un agente

    public List<HomePropertyViewModel> Properties { get; set; } = new();
    public SelectList? PropertyTypes { get; set; }
    public SelectList? SaleTypes { get; set; }
}