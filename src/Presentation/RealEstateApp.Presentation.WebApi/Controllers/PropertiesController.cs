using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Property;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Administrador,Desarrollador")]
public class PropertiesController : ControllerBase
{
    private readonly IApiPropertyService _propertyService;

    public PropertiesController(IApiPropertyService propertyService)
    {
        _propertyService = propertyService;
    }

    /// <summary>Lista todas las propiedades.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<PropertyDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List()
    {
        var properties = await _propertyService.GetAllAsync();
        if (properties.Count == 0) return NoContent();

        return Ok(properties);
    }

    /// <summary>Obtiene una propiedad por su Id.</summary>
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var property = await _propertyService.GetByIdAsync(id);
        if (property == null) return NotFound();

        return Ok(property);
    }

    /// <summary>Obtiene una propiedad por su código único de 6 dígitos.</summary>
    [HttpGet("code/{code}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(PropertyDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetByCode(string code)
    {
        var property = await _propertyService.GetByCodeAsync(code);
        if (property == null) return NotFound();

        return Ok(property);
    }
}
