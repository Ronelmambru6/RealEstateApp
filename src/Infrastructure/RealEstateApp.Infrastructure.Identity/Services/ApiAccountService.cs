using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using RealEstateApp.Core.Application.Dtos.Account;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class ApiAccountService : IApiAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public ApiAccountService(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request)
    {
        var response = new AuthenticationResponse();

        var user = await _userManager.FindByEmailAsync(request.EmailOrUserName)
                   ?? await _userManager.FindByNameAsync(request.EmailOrUserName);

        if (user == null)
        {
            response.HasError = true;
            response.Error = "No existe una cuenta registrada con esas credenciales.";
            return response;
        }

        var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
        if (!isPasswordValid)
        {
            response.HasError = true;
            response.Error = "Credenciales inválidas.";
            return response;
        }

        if (!user.EmailConfirmed || !user.IsActive)
        {
            response.HasError = true;
            response.Error = "Su cuenta se encuentra inactiva.";
            return response;
        }

        var rolesList = await _userManager.GetRolesAsync(user);

        // Regla de negocio Issue #18: Bloquear Cliente y Agente en la WebApi
        if (rolesList.Contains("Cliente") || rolesList.Contains("Agente"))
        {
            response.HasError = true;
            response.Error = "Los usuarios con rol Cliente o Agente no tienen permiso para utilizar la Web API.";
            return response;
        }

        JwtSecurityToken jwtSecurityToken = await GenerateJwtTokenAsync(user, rolesList);

        response.Id = user.Id;
        response.Email = user.Email ?? string.Empty;
        response.UserName = user.UserName ?? string.Empty;
        response.Roles = rolesList.ToList();
        response.IsVerified = user.EmailConfirmed;
        response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);

        return response;
    }

    public async Task<RegisterApiUserResponse> RegisterUserWithRoleAsync(RegisterApiUserRequest request, string roleName)
    {
        var response = new RegisterApiUserResponse();

        var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
        if (userWithSameUserName != null)
        {
            response.HasError = true;
            response.Error = $"El nombre de usuario '{request.UserName}' ya está en uso.";
            return response;
        }

        var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
        if (userWithSameEmail != null)
        {
            response.HasError = true;
            response.Error = $"El correo '{request.Email}' ya está registrado.";
            return response;
        }

        bool nationalIdExists = await _userManager.Users.AnyAsync(u => u.NationalId == request.NationalId);
        if (nationalIdExists)
        {
            response.HasError = true;
            response.Error = $"La cédula '{request.NationalId}' ya está registrada.";
            return response;
        }

        var user = new ApplicationUser
        {
            FirstName = request.FirstName.Trim(),
            LastName = request.LastName.Trim(),
            NationalId = request.NationalId.Trim(),
            Email = request.Email.Trim(),
            UserName = request.UserName.Trim(),
            EmailConfirmed = true,
            IsActive = true
        };

        var result = await _userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            response.HasError = true;
            response.Error = string.Join(" ", result.Errors.Select(e => e.Description));
            return response;
        }

        await _userManager.AddToRoleAsync(user, roleName);

        response.Id = user.Id;
        response.UserName = user.UserName;
        response.Email = user.Email;
        response.Role = roleName;

        return response;
    }

    private async Task<JwtSecurityToken> GenerateJwtTokenAsync(ApplicationUser user, IList<string> roles)
    {
        var userClaims = await _userManager.GetClaimsAsync(user);
        var roleClaims = roles.Select(r => new Claim(ClaimTypes.Role, r)).ToList();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.UserName ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new("uid", user.Id)
        }
        .Union(userClaims)
        .Union(roleClaims);

        string key = _configuration["JWTSettings:Key"] ?? "SuperSecretDefaultKeyForRealEstateApp2026!@#";
        string issuer = _configuration["JWTSettings:Issuer"] ?? "RealEstateAppApi";
        string audience = _configuration["JWTSettings:Audience"] ?? "RealEstateAppApiUsers";
        double durationInMinutes = double.TryParse(_configuration["JWTSettings:DurationInMinutes"], out double dur) ? dur : 60;

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
        var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        return new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(durationInMinutes),
            signingCredentials: signingCredentials
        );
    }
}