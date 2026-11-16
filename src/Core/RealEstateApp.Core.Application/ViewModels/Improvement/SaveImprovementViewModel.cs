using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Improvement;

public class SaveImprovementViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Debe ingresar el nombre de la mejora")]
    [StringLength(100, ErrorMessage = "El nombre no puede tener más de 100 caracteres")]
    [Display(Name = "Nombre")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar la descripción de la mejora")]
    [Display(Name = "Descripción")]
    public string Description { get; set; } = string.Empty;
}
