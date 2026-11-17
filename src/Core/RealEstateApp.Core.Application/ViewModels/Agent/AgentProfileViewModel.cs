using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.Agent;

public class AgentProfileViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El teléfono es obligatorio.")]
    [Phone(ErrorMessage = "Formato de teléfono no válido.")]
    [Display(Name = "Teléfono")]
    public string PhoneNumber { get; set; } = string.Empty;

    public string? ExistingPhotoUrl { get; set; }

    [Display(Name = "Actualizar Foto de Perfil")]
    public IFormFile? PhotoFile { get; set; }
}