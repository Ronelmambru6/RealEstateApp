using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Offer;
using RealEstateApp.Core.Domain.Entities;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Persistence.Services;

public class OfferService : IOfferService
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public OfferService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ClientPropertyOffersViewModel?> GetClientOffersAsync(int propertyId, string clientId)
    {
        var property = await _context.Properties
            .Include(p => p.Offers)
            .FirstOrDefaultAsync(p => p.Id == propertyId);

        if (property == null) return null;

        var clientOffers = property.Offers
            .Where(o => o.ClientId == clientId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OfferViewModel
            {
                Id = o.Id,
                PropertyId = o.PropertyId,
                ClientId = o.ClientId,
                Amount = o.Amount,
                CreatedAt = o.CreatedAt,
                Status = o.Status
            })
            .ToList();

        bool propertySoldOrAccepted = property.Status == PropertyStatus.Sold ||
                                      property.Offers.Any(o => o.Status == OfferStatus.Accepted);
        bool clientHasPending = clientOffers.Any(o => o.Status == OfferStatus.Pending);

        string? blockReason = null;
        if (propertySoldOrAccepted)
            blockReason = "Esta propiedad ya ha sido vendida o tiene una oferta aceptada.";
        else if (clientHasPending)
            blockReason = "Ya tienes una oferta pendiente en esta propiedad. Debes esperar la respuesta del agente antes de realizar otra.";

        return new ClientPropertyOffersViewModel
        {
            PropertyId = property.Id,
            PropertyCode = property.Code,
            PropertyPrice = property.Price,
            CanCreateOffer = !propertySoldOrAccepted && !clientHasPending,
            BlockReason = blockReason,
            Offers = clientOffers,
            NewOffer = new CreateOfferViewModel { PropertyId = property.Id }
        };
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> CreateOfferAsync(CreateOfferViewModel vm, string clientId)
    {
        var property = await _context.Properties
            .Include(p => p.Offers)
            .FirstOrDefaultAsync(p => p.Id == vm.PropertyId);

        if (property == null)
            return (false, "La propiedad especificada no existe.");

        if (property.Status == PropertyStatus.Sold || property.Offers.Any(o => o.Status == OfferStatus.Accepted))
            return (false, "No se pueden realizar ofertas porque la propiedad ya fue vendida.");

        bool hasPending = property.Offers.Any(o => o.ClientId == clientId && o.Status == OfferStatus.Pending);
        if (hasPending)
            return (false, "Ya posees una oferta pendiente para esta propiedad.");

        var offer = new Offer
        {
            PropertyId = vm.PropertyId,
            ClientId = clientId,
            Amount = vm.Amount,
            Status = OfferStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _context.Offers.Add(offer);
        await _context.SaveChangesAsync();
        return (true, null);
    }

    public async Task<List<AgentOfferClientViewModel>> GetClientsWithOffersByPropertyAsync(int propertyId, string agentId)
    {
        var property = await _context.Properties
            .Include(p => p.Offers)
            .FirstOrDefaultAsync(p => p.Id == propertyId && p.AgentId == agentId);

        if (property == null) return new List<AgentOfferClientViewModel>();

        var groupedByClient = property.Offers
            .GroupBy(o => o.ClientId)
            .ToList();

        var result = new List<AgentOfferClientViewModel>();

        foreach (var group in groupedByClient)
        {
            var user = await _userManager.FindByIdAsync(group.Key);
            var latestOffer = group.OrderByDescending(o => o.CreatedAt).First();

            result.Add(new AgentOfferClientViewModel
            {
                ClientId = group.Key,
                ClientName = user != null ? $"{user.FirstName} {user.LastName}" : "Cliente",
                ClientEmail = user?.Email ?? string.Empty,
                TotalOffersCount = group.Count(),
                HasPendingOffer = group.Any(o => o.Status == OfferStatus.Pending),
                LatestOfferAmount = latestOffer.Amount,
                LatestOfferDate = latestOffer.CreatedAt
            });
        }

        return result.OrderByDescending(r => r.HasPendingOffer).ThenByDescending(r => r.LatestOfferDate).ToList();
    }

    public async Task<List<OfferViewModel>> GetOffersByClientAndPropertyAsync(int propertyId, string clientId, string agentId)
    {
        bool ownsProperty = await _context.Properties.AnyAsync(p => p.Id == propertyId && p.AgentId == agentId);
        if (!ownsProperty) return new List<OfferViewModel>();

        var user = await _userManager.FindByIdAsync(clientId);
        string clientName = user != null ? $"{user.FirstName} {user.LastName}" : "Cliente";

        return await _context.Offers
            .Where(o => o.PropertyId == propertyId && o.ClientId == clientId)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OfferViewModel
            {
                Id = o.Id,
                PropertyId = o.PropertyId,
                ClientId = o.ClientId,
                ClientName = clientName,
                Amount = o.Amount,
                CreatedAt = o.CreatedAt,
                Status = o.Status
            })
            .ToListAsync();
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> RespondToOfferAsync(int offerId, bool accept, string agentId)
    {
        var offer = await _context.Offers
            .Include(o => o.Property)
            .FirstOrDefaultAsync(o => o.Id == offerId);

        if (offer == null || offer.Property == null || offer.Property.AgentId != agentId)
            return (false, "No se encontró la oferta o no tienes permiso sobre esta propiedad.");

        if (offer.Status != OfferStatus.Pending)
            return (false, "Solo se pueden responder ofertas que estén en estado pendiente.");

        if (offer.Property.Status == PropertyStatus.Sold)
            return (false, "Esta propiedad ya figura como vendida.");

        if (accept)
        {
            // 1. Aceptar esta oferta
            offer.Status = OfferStatus.Accepted;

            // 2. Cambiar el estado de la propiedad a Vendida
            offer.Property.Status = PropertyStatus.Sold;

            // 3. Rechazar automáticamente todas las demás ofertas pendientes en esta propiedad
            var otherPendingOffers = await _context.Offers
                .Where(o => o.PropertyId == offer.PropertyId && o.Id != offer.Id && o.Status == OfferStatus.Pending)
                .ToListAsync();

            foreach (var pending in otherPendingOffers)
            {
                pending.Status = OfferStatus.Rejected;
            }
        }
        else
        {
            // Rechazar únicamente esta oferta
            offer.Status = OfferStatus.Rejected;
        }

        await _context.SaveChangesAsync();
        return (true, null);
    }
}