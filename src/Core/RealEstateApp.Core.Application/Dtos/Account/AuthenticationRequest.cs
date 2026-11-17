using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Account;

public class AuthenticationRequest
{
    [Required(ErrorMessage = "El correo o nombre de usuario es obligatorio.")]
    public string EmailOrUserName { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    public string Password { get; set; } = string.Empty;
}