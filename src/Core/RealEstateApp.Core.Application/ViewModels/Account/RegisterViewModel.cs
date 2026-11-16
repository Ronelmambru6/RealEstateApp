using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace RealEstateApp.Core.Application.ViewModels.Account;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Debe ingresar su nombre")]
    [Display(Name = "Nombre")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar su apellido")]
    [Display(Name = "Apellido")]
    public string LastName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar un nombre de usuario")]
    [Display(Name = "Usuario")]
    public string UserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar su correo electrónico")]
    [EmailAddress(ErrorMessage = "Debe ingresar un correo electrónico válido")]
    [Display(Name = "Correo electrónico")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar su teléfono")]
    [Phone(ErrorMessage = "Debe ingresar un teléfono válido")]
    [Display(Name = "Teléfono")]
    public string PhoneNumber { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un tipo de usuario")]
    [Display(Name = "Tipo de usuario")]
    public string Role { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar una contraseña")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe confirmar su contraseña")]
    [Compare(nameof(Password), ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    [Display(Name = "Confirmar contraseña")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Display(Name = "Foto de perfil")]
    public IFormFile? Photo { get; set; }

    public string? PhotoUrl { get; set; }
}
