using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Chat;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class ChatService : IChatService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public ChatService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ChatThreadViewModel?> GetThreadAsync(int propertyId, string clientId)
    {
        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property is null)
        {
            return null;
        }

        var client = await _userManager.FindByIdAsync(clientId);
        var agent = await _userManager.FindByIdAsync(property.AgentId);
        var clientName = client is null ? "Cliente" : $"{client.FirstName} {client.LastName}";
        var agentName = agent is null ? "Agente" : $"{agent.FirstName} {agent.LastName}";

        var messages = await _context.ChatMessages
            .Where(m => m.PropertyId == propertyId && m.ClientId == clientId)
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .ToListAsync();

        return new ChatThreadViewModel
        {
            PropertyId = property.Id,
            PropertyCode = property.Code,
            ClientId = clientId,
            ClientName = clientName,
            AgentName = agentName,
            Messages = messages.Select(m => new ChatMessageViewModel
            {
                Content = m.Content,
                IsFromAgent = m.IsFromAgent,
                SenderName = m.IsFromAgent ? agentName : clientName,
                CreatedAt = m.CreatedAt
            }).ToList()
        };
    }

    public async Task<List<ChatConversationViewModel>> GetAgentConversationsAsync(string agentId)
    {
        var messages = await _context.ChatMessages
            .Include(m => m.Property)
            .Where(m => m.Property!.AgentId == agentId)
            .ToListAsync();

        var clientIds = messages.Select(m => m.ClientId).Distinct().ToList();
        var clients = await _userManager.Users
            .Where(u => clientIds.Contains(u.Id))
            .ToListAsync();
        var clientNames = clients.ToDictionary(u => u.Id, u => $"{u.FirstName} {u.LastName}");

        return messages
            .GroupBy(m => new { m.PropertyId, m.ClientId })
            .Select(g =>
            {
                var last = g.OrderByDescending(m => m.CreatedAt).ThenByDescending(m => m.Id).First();
                return new ChatConversationViewModel
                {
                    PropertyId = g.Key.PropertyId,
                    PropertyCode = last.Property!.Code,
                    ClientId = g.Key.ClientId,
                    ClientName = clientNames.TryGetValue(g.Key.ClientId, out var name) ? name : "Cliente",
                    LastMessage = last.Content,
                    LastMessageAt = last.CreatedAt,
                    LastMessageFromAgent = last.IsFromAgent,
                    MessageCount = g.Count()
                };
            })
            .OrderByDescending(c => c.LastMessageAt)
            .ToList();
    }

    public async Task<bool> SendClientMessageAsync(string clientId, int propertyId, string content)
    {
        if (string.IsNullOrWhiteSpace(content))
        {
            return false;
        }

        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property is null || property.AgentId == clientId)
        {
            return false;
        }

        _context.ChatMessages.Add(new ChatMessage
        {
            PropertyId = propertyId,
            ClientId = clientId,
            SenderId = clientId,
            IsFromAgent = false,
            Content = content.Trim()
        });
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SendAgentMessageAsync(string agentId, int propertyId, string clientId, string content)
    {
        if (string.IsNullOrWhiteSpace(content) || string.IsNullOrWhiteSpace(clientId))
        {
            return false;
        }

        var property = await _context.Properties.FirstOrDefaultAsync(p => p.Id == propertyId);
        if (property is null || property.AgentId != agentId)
        {
            return false;
        }

        var threadExists = await _context.ChatMessages
            .AnyAsync(m => m.PropertyId == propertyId && m.ClientId == clientId);
        if (!threadExists)
        {
            return false;
        }

        _context.ChatMessages.Add(new ChatMessage
        {
            PropertyId = propertyId,
            ClientId = clientId,
            SenderId = agentId,
            IsFromAgent = true,
            Content = content.Trim()
        });
        await _context.SaveChangesAsync();
        return true;
    }
}
