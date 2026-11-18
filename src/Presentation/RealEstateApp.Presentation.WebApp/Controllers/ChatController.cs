using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Chat;

namespace RealEstateApp.Presentation.WebApp.Controllers;

[Authorize(Roles = "Cliente,Agente")]
public class ChatController : Controller
{
    private readonly IChatService _chatService;

    public ChatController(IChatService chatService)
    {
        _chatService = chatService;
    }

    // ----- Cliente: envía mensajes desde el detalle de la propiedad -----

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Cliente")]
    public async Task<IActionResult> Send(SendChatMessageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["ChatError"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
        }
        else
        {
            var clientId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sent = await _chatService.SendClientMessageAsync(clientId, viewModel.PropertyId, viewModel.Content);
            if (!sent)
            {
                TempData["ChatError"] = "No se pudo enviar el mensaje.";
            }
        }

        return RedirectToAction("Details", "Home", new { id = viewModel.PropertyId }, "chat");
    }

    // ----- Agente: bandeja de conversaciones y respuestas -----

    [Authorize(Roles = "Agente")]
    public async Task<IActionResult> Inbox()
    {
        var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var conversations = await _chatService.GetAgentConversationsAsync(agentId);
        return View(conversations);
    }

    [HttpGet]
    [Authorize(Roles = "Agente")]
    public async Task<IActionResult> Thread(int propertyId, string clientId)
    {
        var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var conversations = await _chatService.GetAgentConversationsAsync(agentId);
        if (!conversations.Any(c => c.PropertyId == propertyId && c.ClientId == clientId))
        {
            return RedirectToAction(nameof(Inbox));
        }

        var thread = await _chatService.GetThreadAsync(propertyId, clientId);
        if (thread is null)
        {
            return RedirectToAction(nameof(Inbox));
        }

        return View(thread);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Agente")]
    public async Task<IActionResult> Reply(SendChatMessageViewModel viewModel)
    {
        if (!ModelState.IsValid)
        {
            TempData["ChatError"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
        }
        else
        {
            var agentId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
            var sent = await _chatService.SendAgentMessageAsync(agentId, viewModel.PropertyId, viewModel.ClientId ?? string.Empty, viewModel.Content);
            if (!sent)
            {
                TempData["ChatError"] = "No se pudo enviar el mensaje.";
            }
        }

        return RedirectToAction(nameof(Thread), new { propertyId = viewModel.PropertyId, clientId = viewModel.ClientId });
    }
}
