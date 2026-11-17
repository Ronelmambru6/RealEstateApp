using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Home;

namespace RealEstateApp.Presentation.WebApp.Controllers;

public class HomeController : Controller
{
    private readonly IHomeService _homeService;
    private readonly ICatalogMaintenanceService _catalogService;

    public HomeController(IHomeService homeService, ICatalogMaintenanceService catalogService)
    {
        _homeService = homeService;
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(FilterPropertyViewModel filter)
    {
        var propertyTypes = await _catalogService.GetAllPropertyTypesAsync();
        var saleTypes = await _catalogService.GetAllSaleTypesAsync();

        filter.PropertyTypes = new SelectList(propertyTypes, "Id", "Name", filter.PropertyTypeId);
        filter.SaleTypes = new SelectList(saleTypes, "Id", "Name", filter.SaleTypeId);

        filter.Properties = await _homeService.GetFilteredPropertiesAsync(filter);

        return View(filter);
    }

    public async Task<IActionResult> Details(int id)
    {
        var propertyDetail = await _homeService.GetPropertyDetailByIdAsync(id);
        if (propertyDetail == null) return NotFound();

        return View(propertyDetail);
    }
}