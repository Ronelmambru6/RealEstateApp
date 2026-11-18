using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApp.ViewComponents;

/// <summary>Chat del cliente con el agente, dentro del detalle de la propiedad.</summary>
public class PropertyChatViewComponent : ViewComponent
{
    private readonly IChatService _chatService;

    public PropertyChatViewComponent(IChatService chatService)
    {
        _chatService = chatService;
    }

    public async Task<IViewComponentResult> InvokeAsync(int propertyId)
    {
        if (UserClaimsPrincipal.Identity?.IsAuthenticated != true)
        {
            return View("Anonymous");
        }

        // Los agentes y administradores no chatean desde el detalle; el agente usa su bandeja de mensajes.
        if (!UserClaimsPrincipal.IsInRole("Cliente"))
        {
            return Content(string.Empty);
        }

        var clientId = UserClaimsPrincipal.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var thread = await _chatService.GetThreadAsync(propertyId, clientId);
        if (thread is null)
        {
            return Content(string.Empty);
        }

        return View(thread);
    }
}
