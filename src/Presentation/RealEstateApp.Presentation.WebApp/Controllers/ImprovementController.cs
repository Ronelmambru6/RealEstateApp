using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Improvement;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class ImprovementController : Controller
{
    private readonly IImprovementService _improvementService;

    public ImprovementController(IImprovementService improvementService)
    {
        _improvementService = improvementService;
    }

    public async Task<IActionResult> Index()
    {
        var improvements = await _improvementService.GetAllViewModelAsync();
        return View(improvements.OrderBy(i => i.Name).ToList());
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View("Save", new SaveImprovementViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SaveImprovementViewModel saveViewModel)
    {
        if (await _improvementService.ExistsByNameAsync(saveViewModel.Name))
        {
            ModelState.AddModelError(nameof(saveViewModel.Name), "Ya existe una mejora con ese nombre");
        }

        if (!ModelState.IsValid)
        {
            return View("Save", saveViewModel);
        }

        await _improvementService.AddAsync(saveViewModel);
        TempData["SuccessMessage"] = "La mejora fue creada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        var improvement = await _improvementService.GetByIdSaveViewModelAsync(id);
        if (improvement is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View("Save", improvement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(SaveImprovementViewModel saveViewModel)
    {
        if (await _improvementService.ExistsByNameAsync(saveViewModel.Name, saveViewModel.Id))
        {
            ModelState.AddModelError(nameof(saveViewModel.Name), "Ya existe una mejora con ese nombre");
        }

        if (!ModelState.IsValid)
        {
            return View("Save", saveViewModel);
        }

        await _improvementService.UpdateAsync(saveViewModel, saveViewModel.Id);
        TempData["SuccessMessage"] = "La mejora fue actualizada correctamente.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        var improvement = await _improvementService.GetByIdSaveViewModelAsync(id);
        if (improvement is null)
        {
            return RedirectToAction(nameof(Index));
        }

        return View(improvement);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        // Solo se elimina la mejora y su asociación con las propiedades (FK en cascada sobre PropertyImprovements);
        // las propiedades no se eliminan.
        await _improvementService.DeleteAsync(id);
        TempData["SuccessMessage"] = "La mejora fue eliminada correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
