using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Agent;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AgentsController : ControllerBase
{
    private readonly IApiAgentService _agentService;
    private readonly IApiPropertyService _propertyService;

    public AgentsController(IApiAgentService agentService, IApiPropertyService propertyService)
    {
        _agentService = agentService;
        _propertyService = propertyService;
    }

    /// <summary>Lista todos los agentes.</summary>
    [Authorize(Roles = "Administrador,Desarrollador")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<AgentDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List()
    {
        var agents = await _agentService.GetAllAsync();
        if (agents.Count == 0) return NoContent();

        return Ok(agents);
    }

    /// <summary>Obtiene un agente por su Id.</summary>
    [Authorize(Roles = "Administrador,Desarrollador")]
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AgentDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(string id)
    {
        var agent = await _agentService.GetByIdAsync(id);
        if (agent == null) return NotFound();

        return Ok(agent);
    }

    /// <summary>Lista las propiedades de un agente.</summary>
    [Authorize(Roles = "Administrador,Desarrollador")]
    [HttpGet("{id}/properties")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetAgentProperty(string id)
    {
        var agent = await _agentService.GetByIdAsync(id);
        if (agent == null) return NotFound();

        var properties = await _propertyService.GetByAgentIdAsync(id);
        if (properties.Count == 0) return NoContent();

        return Ok(properties);
    }

    /// <summary>Activa o inactiva a un agente. Solo el Administrador.</summary>
    [Authorize(Roles = "Administrador")]
    [HttpPatch("{id}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ChangeStatus(string id, [FromBody] ChangeAgentStatusRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var changed = await _agentService.ChangeStatusAsync(id, request.IsActive!.Value);
        if (!changed) return NotFound();

        return NoContent();
    }
}
