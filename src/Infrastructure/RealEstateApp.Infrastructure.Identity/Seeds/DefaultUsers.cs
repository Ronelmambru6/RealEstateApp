using Microsoft.AspNetCore.Identity;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Seeds;

public static class DefaultUsers
{
    public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
    {
        // 1. Usuario Administrador por defecto
        var adminUser = new ApplicationUser
        {
            UserName = "adminuser",
            Email = "admin@realestate.com",
            FirstName = "Admin",
            LastName = "Sistema",
            Cedula = "00100000011",
            PhoneNumber = "8095550001",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        await CreateUserIfNotExists(userManager, adminUser, "Pa$$word123!", Roles.Administrador);

        // 2. Usuario Agente por defecto
        var agentUser = new ApplicationUser
        {
            UserName = "agentuser",
            Email = "agente@realestate.com",
            FirstName = "Carlos",
            LastName = "Inmobiliario",
            PhoneNumber = "8095550002",
            PhotoUrl = "/images/default-avatar.png",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        await CreateUserIfNotExists(userManager, agentUser, "Pa$$word123!", Roles.Agente);

        // 3. Usuario Cliente por defecto
        var clientUser = new ApplicationUser
        {
            UserName = "clientuser",
            Email = "cliente@realestate.com",
            FirstName = "Maria",
            LastName = "Compradora",
            PhoneNumber = "8095550003",
            PhotoUrl = "/images/default-avatar.png",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        await CreateUserIfNotExists(userManager, clientUser, "Pa$$word123!", Roles.Cliente);

        // 4. Usuario Desarrollador por defecto (para consumo de la API)
        var devUser = new ApplicationUser
        {
            UserName = "devuser",
            Email = "dev@realestate.com",
            FirstName = "Pedro",
            LastName = "Developer",
            Cedula = "00100000022",
            PhoneNumber = "8095550004",
            EmailConfirmed = true,
            PhoneNumberConfirmed = true,
            IsActive = true
        };
        await CreateUserIfNotExists(userManager, devUser, "Pa$$word123!", Roles.Desarrollador);
    }

    private static async Task CreateUserIfNotExists(
        UserManager<ApplicationUser> userManager,
        ApplicationUser user,
        string password,
        Roles role)
    {
        var existingByEmail = await userManager.FindByEmailAsync(user.Email!);
        var existingByName = await userManager.FindByNameAsync(user.UserName!);

        if (existingByEmail == null && existingByName == null)
        {
            var result = await userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, role.ToString());
            }
        }
    }
}