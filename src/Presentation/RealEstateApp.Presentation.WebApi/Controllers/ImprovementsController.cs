using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Dtos.Catalog;
using RealEstateApp.Core.Application.Interfaces.Services;

namespace RealEstateApp.Presentation.WebApi.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class ImprovementsController : ControllerBase
{
    private readonly IApiCatalogService _catalogService;

    public ImprovementsController(IApiCatalogService catalogService)
    {
        _catalogService = catalogService;
    }

    [Authorize(Roles = "Administrador,Desarrollador")]
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<CatalogItemDto>))]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> List()
    {
        var items = await _catalogService.GetAllImprovementsAsync();
        if (items.Count == 0) return NoContent();

        return Ok(items);
    }

    [Authorize(Roles = "Administrador,Desarrollador")]
    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(CatalogItemDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetById(int id)
    {
        var item = await _catalogService.GetImprovementByIdAsync(id);
        if (item == null) return NotFound();

        return Ok(item);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CatalogItemDto))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Create([FromBody] SaveCatalogItemRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var created = await _catalogService.CreateImprovementAsync(request);
        return StatusCode(StatusCodes.Status201Created, created);
    }

    [Authorize(Roles = "Administrador")]
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateCatalogItemResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Update(int id, [FromBody] SaveCatalogItemRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var updated = await _catalogService.UpdateImprovementAsync(id, request);
        if (updated == null) return NotFound();

        return Ok(updated);
    }

    [Authorize(Roles = "Administrador")]
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> Delete(int id)
    {
        bool deleted = await _catalogService.DeleteImprovementAsync(id);
        if (!deleted) return NotFound();

        return NoContent();
    }
}