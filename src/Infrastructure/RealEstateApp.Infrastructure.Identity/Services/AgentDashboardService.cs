using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Agent;
using RealEstateApp.Core.Domain.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;
using RealEstateApp.Infrastructure.Persistence.Contexts;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AgentDashboardService : IAgentDashboardService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ApplicationDbContext _context;

    public AgentDashboardService(UserManager<ApplicationUser> userManager, ApplicationDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<List<AgentPropertyViewModel>> GetPropertiesByAgentIdAsync(string agentId)
    {
        return await _context.Properties
            .Include(p => p.PropertyType)
            .Include(p => p.SaleType)
            .Include(p => p.Images)
            .Where(p => p.AgentId == agentId)
            .OrderByDescending(p => p.CreatedAt)
            .Select(p => new AgentPropertyViewModel
            {
                Id = p.Id,
                Code = p.Code,
                PropertyTypeName = p.PropertyType!.Name,
                SaleTypeName = p.SaleType!.Name,
                Price = p.Price,
                Bedrooms = p.Bedrooms,
                Bathrooms = p.Bathrooms,
                SizeInMeters = p.SizeInMeters,
                MainImageUrl = p.Images.Select(img => img.ImageUrl).FirstOrDefault(),
                IsAvailable = p.Status == PropertyStatus.Available
            })
            .ToListAsync();
    }

    public async Task<AgentProfileViewModel?> GetAgentProfileByIdAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent == null) return null;

        return new AgentProfileViewModel
        {
            Id = agent.Id,
            FirstName = agent.FirstName,
            LastName = agent.LastName,
            PhoneNumber = agent.PhoneNumber ?? string.Empty,
            ExistingPhotoUrl = agent.PhotoUrl
        };
    }

    public async Task<bool> UpdateAgentProfileAsync(AgentProfileViewModel vm)
    {
        var agent = await _userManager.FindByIdAsync(vm.Id);
        if (agent == null) return false;

        agent.FirstName = vm.FirstName.Trim();
        agent.LastName = vm.LastName.Trim();
        agent.PhoneNumber = vm.PhoneNumber.Trim();

        // Manejo de la subida de foto de perfil si se adjuntó un archivo
        if (vm.PhotoFile != null && vm.PhotoFile.Length > 0)
        {
            // Ruta relativa de almacenamiento (ej. /images/profiles/)
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
            if (!Directory.Exists(uploadsFolder)) Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(vm.PhotoFile.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await vm.PhotoFile.CopyToAsync(fileStream);
            }

            agent.PhotoUrl = $"/images/profiles/{uniqueFileName}";
        }

        var result = await _userManager.UpdateAsync(agent);
        return result.Succeeded;
    }
}