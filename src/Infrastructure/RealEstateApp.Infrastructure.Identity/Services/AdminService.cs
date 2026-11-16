using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Admin;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AdminService : IAdminService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IPropertyStatsService _propertyStatsService;

    public AdminService(
        UserManager<ApplicationUser> userManager,
        IPropertyStatsService propertyStatsService)
    {
        _userManager = userManager;
        _propertyStatsService = propertyStatsService;
    }

    public async Task<AdminDashboardViewModel> GetDashboardStatsAsync()
    {
        var (availableProps, soldProps) = await _propertyStatsService.GetPropertyCountsAsync();

        var agents = await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString());
        var clients = await _userManager.GetUsersInRoleAsync(Roles.Cliente.ToString());
        var developers = await _userManager.GetUsersInRoleAsync(Roles.Desarrollador.ToString());

        return new AdminDashboardViewModel
        {
            AvailablePropertiesCount = availableProps,
            SoldPropertiesCount = soldProps,
            ActiveAgentsCount = agents.Count(u => u.IsActive),
            InactiveAgentsCount = agents.Count(u => !u.IsActive),
            ActiveClientsCount = clients.Count(u => u.IsActive),
            InactiveClientsCount = clients.Count(u => !u.IsActive),
            ActiveDevelopersCount = developers.Count(u => u.IsActive),
            InactiveDevelopersCount = developers.Count(u => !u.IsActive)
        };
    }

    public async Task<List<AgentListItemViewModel>> GetAllAgentsAsync()
    {
        var agents = await _userManager.GetUsersInRoleAsync(Roles.Agente.ToString());
        var agentIds = agents.Select(a => a.Id);
        var propCounts = await _propertyStatsService.GetPropertyCountByAgentIdsAsync(agentIds);

        return agents
            .OrderBy(a => a.FirstName)
            .ThenBy(a => a.LastName)
            .Select(a => new AgentListItemViewModel
            {
                Id = a.Id,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Email = a.Email ?? string.Empty,
                PhoneNumber = a.PhoneNumber,
                IsActive = a.IsActive,
                PropertiesCount = propCounts.TryGetValue(a.Id, out var count) ? count : 0
            })
            .ToList();
    }

    public async Task<bool> ToggleAgentStatusAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent == null) return false;

        agent.IsActive = !agent.IsActive;
        var result = await _userManager.UpdateAsync(agent);
        return result.Succeeded;
    }

    public async Task<bool> DeleteAgentWithCascadeAsync(string agentId)
    {
        var agent = await _userManager.FindByIdAsync(agentId);
        if (agent == null) return false;

        await _propertyStatsService.DeletePropertiesByAgentIdAsync(agentId);
        var result = await _userManager.DeleteAsync(agent);
        return result.Succeeded;
    }


    // =========================================================================
    // ISSUE #8: MANTENIMIENTO DE ADMINISTRADORES Y DESARROLLADORES
    // =========================================================================
    public async Task<List<SystemUserViewModel>> GetSystemUsersByRoleAsync(Roles role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role.ToString());
        return users
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new SystemUserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                UserName = u.UserName ?? string.Empty,
                Cedula = u.Cedula ?? string.Empty,
                Email = u.Email ?? string.Empty,
                IsActive = u.IsActive,
                Role = role.ToString()
            })
            .ToList();
    }

    public async Task<EditSystemUserViewModel?> GetSystemUserForEditAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null) return null;

        var roles = await _userManager.GetRolesAsync(user);

        return new EditSystemUserViewModel
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Cedula = user.Cedula ?? string.Empty,
            Email = user.Email ?? string.Empty,
            UserName = user.UserName ?? string.Empty,
            Role = roles.FirstOrDefault() ?? string.Empty
        };
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> CreateSystemUserAsync(CreateSystemUserViewModel vm, Roles role)
    {
        // 1. Validar correo único
        var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
        if (existingEmail != null)
            return (false, "El correo electrónico ya está registrado por otro usuario.");

        // 2. Validar nombre de usuario único
        var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
        if (existingUserName != null)
            return (false, "El nombre de usuario ya está en uso.");

        // 3. Validar cédula única
        var existingCedula = await _userManager.Users.AnyAsync(u => u.Cedula == vm.Cedula);
        if (existingCedula)
            return (false, "La cédula ingresada ya pertenece a otro usuario en el sistema.");

        var newUser = new ApplicationUser
        {
            FirstName = vm.FirstName.Trim(),
            LastName = vm.LastName.Trim(),
            Cedula = vm.Cedula.Trim(),
            Email = vm.Email.Trim(),
            UserName = vm.UserName.Trim(),
            EmailConfirmed = true,
            IsActive = true // Administradores y Desarrolladores nacen activos por defecto
        };

        var result = await _userManager.CreateAsync(newUser, vm.Password);
        if (!result.Succeeded)
        {
            var errors = string.Join(" ", result.Errors.Select(e => e.Description));
            return (false, errors);
        }

        await _userManager.AddToRoleAsync(newUser, role.ToString());
        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> UpdateSystemUserAsync(EditSystemUserViewModel vm, string? currentAdminId)
    {
        // Impedir que el administrador en sesión se edite a sí mismo
        if (!string.IsNullOrEmpty(currentAdminId) && vm.Id == currentAdminId)
            return (false, "No tiene permitido editar su propia cuenta de administrador.");

        var user = await _userManager.FindByIdAsync(vm.Id);
        if (user == null)
            return (false, "El usuario especificado no existe.");

        // 1. Validar correo único (excluyendo al usuario actual)
        var existingEmail = await _userManager.FindByEmailAsync(vm.Email);
        if (existingEmail != null && existingEmail.Id != user.Id)
            return (false, "El correo electrónico ya está registrado por otro usuario.");

        // 2. Validar nombre de usuario único (excluyendo al usuario actual)
        var existingUserName = await _userManager.FindByNameAsync(vm.UserName);
        if (existingUserName != null && existingUserName.Id != user.Id)
            return (false, "El nombre de usuario ya está en uso por otra cuenta.");

        // 3. Validar cédula única (excluyendo al usuario actual)
        var existingCedula = await _userManager.Users.AnyAsync(u => u.Cedula == vm.Cedula && u.Id != user.Id);
        if (existingCedula)
            return (false, "La cédula ingresada ya pertenece a otro usuario.");

        user.FirstName = vm.FirstName.Trim();
        user.LastName = vm.LastName.Trim();
        user.Cedula = vm.Cedula.Trim();
        user.Email = vm.Email.Trim();
        user.UserName = vm.UserName.Trim();

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            var errors = string.Join(" ", updateResult.Errors.Select(e => e.Description));
            return (false, errors);
        }

        // Si se proporcionó una nueva contraseña, actualizarla
        if (!string.IsNullOrWhiteSpace(vm.Password))
        {
            var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var passResult = await _userManager.ResetPasswordAsync(user, resetToken, vm.Password);
            if (!passResult.Succeeded)
            {
                var errors = string.Join(" ", passResult.Errors.Select(e => e.Description));
                return (false, errors);
            }
        }

        return (true, null);
    }

    public async Task<(bool Succeeded, string? ErrorMessage)> ToggleSystemUserStatusAsync(string userId, string? currentAdminId)
    {
        // Impedir que el administrador en sesión se inactive/active a sí mismo
        if (!string.IsNullOrEmpty(currentAdminId) && userId == currentAdminId)
            return (false, "No puede cambiar el estado de su propia cuenta de administrador.");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return (false, "El usuario no fue encontrado.");

        user.IsActive = !user.IsActive;
        var result = await _userManager.UpdateAsync(user);

        return result.Succeeded
            ? (true, null)
            : (false, "Error al actualizar el estado del usuario.");
    }
}