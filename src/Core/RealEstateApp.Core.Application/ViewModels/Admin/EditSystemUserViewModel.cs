using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Admin;

public class EditSystemUserViewModel
{
    [Required]
    public string Id { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre es obligatorio.")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "El apellido es obligatorio.")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La cédula es obligatoria.")]
    [RegularExpression(@"^\d{11}$", ErrorMessage = "La cédula debe contener exactamente 11 dígitos numéricos.")]
    [Display(Name = "Cédula")]
    public string Cedula { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido.")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [Display(Name = "Nombre de usuario")]
    public string UserName { get; set; } = string.Empty;

    [DataType(DataType.Password)]
    [MinLength(6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres.")]
    [Display(Name = "Nueva contraseña (opcional)")]
    public string? Password { get; set; }

    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden.")]
    [Display(Name = "Confirmar nueva contraseña")]
    public string? ConfirmPassword { get; set; }

    public string Role { get; set; } = string.Empty;
}