using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Presentation.WebApp.Controllers;

// [Authorize(Roles = "Administrador")]
public class PropertyTypesController : Controller
{
    private readonly ICatalogMaintenanceService _catalogService;

    public PropertyTypesController(ICatalogMaintenanceService catalogService)
    {
        _catalogService = catalogService;
    }

    public async Task<IActionResult> Index()
    {
        var list = await _catalogService.GetAllPropertyTypesAsync();
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Save", new SaveCatalogItemViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveCatalogItemViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("Save", vm);

        await _catalogService.CreatePropertyTypeAsync(vm);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var item = await _catalogService.GetPropertyTypeByIdAsync(id);
        if (item == null) return NotFound();

        var vm = new SaveCatalogItemViewModel
        {
            Id = item.Id,
            Name = item.Name,
            Description = item.Description
        };
        return View("Save", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SaveCatalogItemViewModel vm)
    {
        if (!ModelState.IsValid)
            return View("Save", vm);

        await _catalogService.UpdatePropertyTypeAsync(vm);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var item = await _catalogService.GetPropertyTypeByIdAsync(id);
        if (item == null) return NotFound();

        return View(item);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _catalogService.DeletePropertyTypeWithCascadeAsync(id);
        return RedirectToAction(nameof(Index));
    }
}