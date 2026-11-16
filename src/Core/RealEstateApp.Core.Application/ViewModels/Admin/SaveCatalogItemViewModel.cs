using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Admin;

public class SaveCatalogItemViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [MaxLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "La descripción es obligatoria.")]
    [MaxLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;
}