using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace RealEstateApp.Core.Application.ViewModels.Property;

public class SavePropertyViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(1, double.MaxValue, ErrorMessage = "El precio debe ser mayor a cero.")]
    [Display(Name = "Precio (DOP)")]
    public double Price { get; set; }

    [Required(ErrorMessage = "La cantidad de habitaciones es obligatoria.")]
    [Range(1, 50, ErrorMessage = "Debe especificar al menos 1 habitación.")]
    [Display(Name = "Habitaciones")]
    public int Bedrooms { get; set; }

    [Required(ErrorMessage = "La cantidad de baños es obligatoria.")]
    [Range(1, 50, ErrorMessage = "Debe especificar al menos 1 baño.")]
    [Display(Name = "Baños")]
    public int Bathrooms { get; set; }

    [Required(ErrorMessage = "Los metros cuadrados son obligatorios.")]
    [Range(10, 100000, ErrorMessage = "Ingrese un tamaño válido en metros cuadrados.")]
    [Display(Name = "Metros Cuadrados (m²)")]
    public double SizeInMeters { get; set; }

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(1000, ErrorMessage = "La descripción no puede exceder los 1000 caracteres.")]
    [Display(Name = "Descripción detallada")]
    public string Description { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un tipo de propiedad.")]
    [Display(Name = "Tipo de Propiedad")]
    public int PropertyTypeId { get; set; }

    [Required(ErrorMessage = "Debe seleccionar un tipo de venta.")]
    [Display(Name = "Tipo de Venta")]
    public int SaleTypeId { get; set; }

    // Mejoras seleccionadas por el agente (IDs)
    [Display(Name = "Mejoras")]
    public List<int> ImprovementIds { get; set; } = new();

    // Carga de imágenes (de 1 a 4 archivos)
    [Display(Name = "Imágenes de la Propiedad (1 a 4 imágenes)")]
    public List<IFormFile>? ImageFiles { get; set; } = new();

    public List<string>? ExistingImageUrls { get; set; } = new();

    // Listas desplegables para los SelectLists en las vistas
    public SelectList? PropertyTypes { get; set; }
    public SelectList? SaleTypes { get; set; }
    public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>? ImprovementsList { get; set; }
}