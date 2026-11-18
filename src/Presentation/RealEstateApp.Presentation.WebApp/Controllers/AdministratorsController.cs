using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AdministratorsController : Controller
{
    private readonly IAdminService _adminService;

    public AdministratorsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        ViewBag.CurrentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var admins = await _adminService.GetSystemUsersByRoleAsync(Roles.Administrador);
        return View(admins);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateSystemUserViewModel { Role = Roles.Administrador.ToString() });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateSystemUserViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        var (succeeded, error) = await _adminService.CreateSystemUserAsync(vm, Roles.Administrador);
        if (!succeeded)
        {
            ModelState.AddModelError(string.Empty, error ?? "Error al crear el administrador.");
            return View(vm);
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(string id)
    {
        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!string.IsNullOrEmpty(currentUserId) && id == currentUserId)
        {
            TempData["ErrorMessage"] = "No tiene permitido editar su propia cuenta de administrador.";
            return RedirectToAction(nameof(Index));
        }

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
            ModelState.AddModelError(string.Empty, error ?? "Error al actualizar el administrador.");
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