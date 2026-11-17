using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Account;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IApiAccountService _accountService;

    public AccountController(IApiAccountService accountService)
    {
        _accountService = accountService;
    }

    /// <summary>
    /// Inicia sesión para Administradores y Desarrolladores y devuelve un token JWT.
    /// (Bloqueado para Clientes y Agentes).
    /// </summary>
    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login([FromBody] AuthenticationRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _accountService.AuthenticateAsync(request);
        if (response.HasError)
            return Unauthorized(new { message = response.Error });

        return Ok(response);
    }

    /// <summary>
    /// Registra un nuevo usuario con el rol de Desarrollador.
    /// </summary>
    [HttpPost("register-developer")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterApiUserResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RegisterDeveloper([FromBody] RegisterApiUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _accountService.RegisterUserWithRoleAsync(request, "Desarrollador");
        if (response.HasError)
            return BadRequest(new { message = response.Error });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// Registra un nuevo usuario con el rol de Administrador (Solo accesible por Administradores).
    /// </summary>
    [Authorize(Roles = "Administrador")]
    [HttpPost("register-admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RegisterApiUserResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RegisterAdmin([FromBody] RegisterApiUserRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var response = await _accountService.RegisterUserWithRoleAsync(request, "Administrador");
        if (response.HasError)
            return BadRequest(new { message = response.Error });

        return StatusCode(StatusCodes.Status201Created, response);
    }
}