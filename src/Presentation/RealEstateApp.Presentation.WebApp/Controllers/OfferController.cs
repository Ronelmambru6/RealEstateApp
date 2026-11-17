using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Offer;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize]
public class OfferController : Controller
{
    private readonly IOfferService _offerService;

    public OfferController(IOfferService offerService)
    {
        _offerService = offerService;
    }

    // ==========================================
    // SECCIÓN CLIENTE
    // ==========================================

    [Authorize(Roles = "Cliente")]
    [HttpGet]
    public async Task<IActionResult> ClientOffers(int propertyId)
    {
        string clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var vm = await _offerService.GetClientOffersAsync(propertyId, clientId);
        if (vm == null) return NotFound();

        return View(vm);
    }

    [Authorize(Roles = "Cliente")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateOfferViewModel vm)
    {
        string clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "Por favor ingrese un monto válido mayor a cero.";
            return RedirectToAction(nameof(ClientOffers), new { propertyId = vm.PropertyId });
        }

        var (succeeded, error) = await _offerService.CreateOfferAsync(vm, clientId);
        if (!succeeded)
        {
            TempData["ErrorMessage"] = error;
        }
        else
        {
            TempData["SuccessMessage"] = "Tu oferta ha sido enviada exitosamente.";
        }

        return RedirectToAction(nameof(ClientOffers), new { propertyId = vm.PropertyId });
    }

    // ==========================================
    // SECCIÓN AGENTE
    // ==========================================

    [Authorize(Roles = "Agente")]
    [HttpGet]
    public async Task<IActionResult> AgentClients(int propertyId)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var clients = await _offerService.GetClientsWithOffersByPropertyAsync(propertyId, agentId);
        ViewBag.PropertyId = propertyId;
        return View(clients);
    }

    [Authorize(Roles = "Agente")]
    [HttpGet]
    public async Task<IActionResult> AgentClientOffers(int propertyId, string clientId)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var offers = await _offerService.GetOffersByClientAndPropertyAsync(propertyId, clientId, agentId);
        ViewBag.PropertyId = propertyId;
        ViewBag.ClientId = clientId;
        ViewBag.ClientName = offers.FirstOrDefault()?.ClientName ?? "Cliente";
        return View(offers);
    }

    [Authorize(Roles = "Agente")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(int offerId, int propertyId, string clientId, bool accept)
    {
        string agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var (succeeded, error) = await _offerService.RespondToOfferAsync(offerId, accept, agentId);

        if (!succeeded)
        {
            TempData["ErrorMessage"] = error;
        }
        else
        {
            TempData["SuccessMessage"] = accept
                ? "Oferta aceptada. La propiedad ha sido marcada como VENDIDA y las demás ofertas pendientes fueron rechazadas."
                : "La oferta ha sido rechazada.";
        }

        return RedirectToAction(nameof(AgentClientOffers), new { propertyId, clientId });
    }
}