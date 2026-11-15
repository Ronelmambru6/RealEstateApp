using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApp.Controllers;

// Nota: Cuando el Login esté conectado puedes descomentar [Authorize(Roles = "Administrador")]
// [Authorize(Roles = "Administrador")]
public class AdminController : Controller
{
    private readonly IAdminService _adminService;

    public AdminController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var dashboardVm = await _adminService.GetDashboardStatsAsync();
        return View(dashboardVm);
    }
}