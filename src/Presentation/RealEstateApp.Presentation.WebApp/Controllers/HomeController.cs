using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Home;

namespace RealEstateApp.Presentation.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    private readonly ICatalogMaintenanceService _catalogService;
    private readonly IFavoriteService _favoriteService;

    public HomeController(IHomeService homeService, ICatalogMaintenanceService catalogService, IFavoriteService favoriteService)
    {
        _homeService = homeService;
        _catalogService = catalogService;
        _favoriteService = favoriteService;
    }

    public async Task<IActionResult> Index(FilterPropertyViewModel filter)
    {
        var propertyTypes = await _catalogService.GetAllPropertyTypesAsync();
        var saleTypes = await _catalogService.GetAllSaleTypesAsync();

        filter.PropertyTypes = new SelectList(propertyTypes, "Id", "Name", filter.PropertyTypeId);
        filter.SaleTypes = new SelectList(saleTypes, "Id", "Name", filter.SaleTypeId);

        filter.FavoriteOfClientId = null;
        filter.Properties = await _homeService.GetFilteredPropertiesAsync(filter);

        // Home del Cliente: marcar las propiedades que ya son favoritas
        if (User.IsInRole("Cliente"))
        {
            filter.FavoriteIds = await _favoriteService.GetFavoritePropertyIdsAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        }

        return View(filter);
    }

    public async Task<IActionResult> Details(int id)
    {
        var propertyDetail = await _homeService.GetPropertyDetailByIdAsync(id);
        if (propertyDetail == null) return NotFound();

        return View(propertyDetail);
    }
}