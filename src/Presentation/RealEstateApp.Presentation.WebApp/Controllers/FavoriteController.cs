using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Home;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Cliente")]
public class FavoriteController : Controller
{
    private readonly IFavoriteService _favoriteService;
    private readonly IHomeService _homeService;
    private readonly ICatalogMaintenanceService _catalogService;

    public FavoriteController(
        IFavoriteService favoriteService,
        IHomeService homeService,
        ICatalogMaintenanceService catalogService)
    {
        _favoriteService = favoriteService;
        _homeService = homeService;
        _catalogService = catalogService;
    }

    // Mis Propiedades: solo favoritas en estado Disponible, con filtros.
    public async Task<IActionResult> Index(FilterPropertyViewModel filter)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        var propertyTypes = await _catalogService.GetAllPropertyTypesAsync();
        var saleTypes = await _catalogService.GetAllSaleTypesAsync();

        filter.AgentId = null;
        filter.FavoriteOfClientId = clientId;
        filter.PropertyTypes = new SelectList(propertyTypes, "Id", "Name", filter.PropertyTypeId);
        filter.SaleTypes = new SelectList(saleTypes, "Id", "Name", filter.SaleTypeId);
        filter.Properties = await _homeService.GetFilteredPropertiesAsync(filter);
        filter.FavoriteIds = filter.Properties.Select(p => p.Id).ToHashSet();

        return View(filter);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Toggle(int propertyId, string? returnUrl)
    {
        var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _favoriteService.ToggleFavoriteAsync(clientId, propertyId);

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }
}
