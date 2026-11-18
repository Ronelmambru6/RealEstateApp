using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class DevelopersController : Controller
{
    private readonly IAdminService _adminService;

    public DevelopersController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var devs = await _adminService.GetSystemUsersByRoleAsync(Roles.Desarrollador);
        return View(devs);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSystemUserViewModel { Role = Roles.Desarrollador.ToString() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSystemUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var (succeeded, error) = await _adminService.CreateSystemUserAsync(vm, Roles.Desarrollador);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al crear el desarrollador.");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var vm = await _adminService.GetSystemUserForEditAsync(id);
        if (vm == null) return NotFound();

        return View(vm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EditSystemUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var (succeeded, error) = await _adminService.UpdateSystemUserAsync(vm, currentUserId);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar el desarrollador.");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var (succeeded, error) = await _adminService.ToggleSystemUserStatusAsync(id, currentUserId);
        if (!succeeded)
        {
            TempData["ErrorMessage"] = error;
        }

        return RedirectToAction(nameof(Index));
    }
}