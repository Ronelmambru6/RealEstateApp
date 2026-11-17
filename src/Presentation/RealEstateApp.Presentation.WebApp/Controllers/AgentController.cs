using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Agent;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Agente")]
public class AgentController : Controller
{
    private readonly IAgentDashboardService _agentService;

    public AgentController(IAgentDashboardService agentService)
    {
        _agentService = agentService;
    }

    public async Task<IActionResult> Index()
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var properties = await _agentService.GetPropertiesByAgentIdAsync(agentId);
        return View(properties);
    }

    [HttpGet]
    public async Task<IActionResult> Profile()
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var profileVm = await _agentService.GetAgentProfileByIdAsync(agentId);
        if (profileVm == null) return NotFound();

        return View(profileVm);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Profile(AgentProfileViewModel vm)
    {
        if (!ModelState.IsValid)
            return View(vm);

        bool updated = await _agentService.UpdateAgentProfileAsync(vm);
        if (!updated)
        {
            ModelState.AddModelError(string.Empty, "No se pudo actualizar el perfil.");
            return View(vm);
        }

        TempData["SuccessMessage"] = "Perfil actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }
}
