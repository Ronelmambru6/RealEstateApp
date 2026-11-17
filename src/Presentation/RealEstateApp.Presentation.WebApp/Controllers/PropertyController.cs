using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Property;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Agente")]
public class PropertyController : Controller
{
    private readonly IPropertyManagementService _propertyService;

    public PropertyController(IPropertyManagementService propertyService)
    {
        _propertyService = propertyService;
    }

    public async Task<IActionResult> Index()
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var properties = await _propertyService.GetPropertiesByAgentAsync(agentId);
        return View(properties);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var vm = await _propertyService.PrepareSaveViewModelAsync();
        return View("Save", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SavePropertyViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await _propertyService.PrepareSaveViewModelAsync(vm);
            return View("Save", vm);
        }

        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (succeeded, error) = await _propertyService.CreatePropertyAsync(vm, agentId);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al registrar la propiedad.");
            await _propertyService.PrepareSaveViewModelAsync(vm);
            return View("Save", vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var vm = await _propertyService.GetPropertyForEditAsync(id, agentId);
        if (vm == null) return NotFound();

        return View("Save", vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SavePropertyViewModel vm)
    {
        if (!ModelState.IsValid)
        {
            await _propertyService.PrepareSaveViewModelAsync(vm);
            return View("Save", vm);
        }

        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (succeeded, error) = await _propertyService.UpdatePropertyAsync(vm, agentId);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar la propiedad.");
            await _propertyService.PrepareSaveViewModelAsync(vm);
            return View("Save", vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var properties = await _propertyService.GetPropertiesByAgentAsync(agentId);
        var property = properties.FirstOrDefault(p => p.Id == id);
        if (property == null) return NotFound();

        return View(property);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        await _propertyService.DeletePropertyAsync(id, agentId);
        return RedirectToAction(nameof(Index));
    }
}