using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Account;

public class LoginViewModel
{
    [Required(ErrorMessage = "Debe ingresar su usuario o correo electrónico")]
    [Display(Name = "Usuario o correo electrónico")]
    public string UserNameOrEmail { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe ingresar su contraseña")]
    [DataType(DataType.Password)]
    [Display(Name = "Contraseña")]
    public string Password { get; set; } = string.Empty;

    [Display(Name = "Recordarme")]
    public bool RememberMe { get; set; }
}
