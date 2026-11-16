using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Administrador")]
public class AgentsController : Controller
{
    private readonly IAdminService _adminService;

    public AgentsController(IAdminService adminService)
    {
        _adminService = adminService;
    }

    public async Task<IActionResult> Index()
    {
        var agents = await _adminService.GetAllAgentsAsync();
        return View(agents);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(string id)
    {
        await _adminService.ToggleAgentStatusAsync(id);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Delete(string id)
    {
        var agents = await _adminService.GetAllAgentsAsync();
        var agent = agents.FirstOrDefault(a => a.Id == id);
        if (agent == null) return NotFound();

        return View(agent);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        await _adminService.DeleteAgentWithCascadeAsync(id);
        return RedirectToAction(nameof(Index));
    }
}