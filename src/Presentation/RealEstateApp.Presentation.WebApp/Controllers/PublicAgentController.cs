using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Home;
using RealEstateApp.Core.Application.ViewModels.PublicAgent;

namespace RealEstateApp.Presentation.WebApp.Controllers;

// Módulo público: no requiere inicio de sesión.
public class PublicAgentController : Controller
{
    private readonly IPublicAgentService _publicAgentService;
    private readonly IHomeService _homeService;
    private readonly ICatalogMaintenanceService _catalogService;

    public PublicAgentController(
        IPublicAgentService publicAgentService,
        IHomeService homeService,
        ICatalogMaintenanceService catalogService)
    {
        _publicAgentService = publicAgentService;
        _homeService = homeService;
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index(string? search)
    {
        var viewModel = new AgentSearchViewModel
        {
            Search = search,
            Agents = await _publicAgentService.GetActiveAgentsAsync(search)
        };

        return View(viewModel);
    }

    public async Task<IActionResult> Properties(string id, FilterPropertyViewModel filter)
    {
        var agent = await _publicAgentService.GetActiveAgentByIdAsync(id);
        if (agent is null)
        {
            return NotFound();
        }

        var propertyTypes = await _catalogService.GetAllPropertyTypesAsync();
        var saleTypes = await _catalogService.GetAllSaleTypesAsync();

        filter.AgentId = agent.Id;
        filter.PropertyTypes = new SelectList(propertyTypes, "Id", "Name", filter.PropertyTypeId);
        filter.SaleTypes = new SelectList(saleTypes, "Id", "Name", filter.SaleTypeId);
        filter.Properties = await _homeService.GetFilteredPropertiesAsync(filter);

        return View(new AgentPropertiesViewModel { Agent = agent, Filter = filter });
    }
}
