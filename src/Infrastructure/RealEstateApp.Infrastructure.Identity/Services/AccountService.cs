using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using RealEstateApp.Core.Application.Dtos.Account;
using RealEstateApp.Core.Application.Dtos.Email;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Account;
using RealEstateApp.Infrastructure.Identity.Entities;

namespace RealEstateApp.Infrastructure.Identity.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEmailService _emailService;

    public AccountService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IEmailService emailService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
    }

    public async Task<AuthenticationResponseDto> LoginAsync(LoginViewModel loginViewModel)
    {
        var response = new AuthenticationResponseDto();

        var user = loginViewModel.UserNameOrEmail.Contains('@')
            ? await _userManager.FindByEmailAsync(loginViewModel.UserNameOrEmail)
            : await _userManager.FindByNameAsync(loginViewModel.UserNameOrEmail);

        if (user is null || !await _userManager.CheckPasswordAsync(user, loginViewModel.Password))
        {
            response.HasError = true;
            response.Error = "Usuario o contraseña incorrectos.";
            return response;
        }

        var roles = await _userManager.GetRolesAsync(user);
        var role = roles.FirstOrDefault();

        if (role == Roles.Desarrollador.ToString())
        {
            response.HasError = true;
            response.Error = "Los usuarios desarrolladores no tienen acceso a la aplicación web. Utilice la API.";
            return response;
        }

        if (role == Roles.Cliente.ToString() && !user.EmailConfirmed)
        {
            response.HasError = true;
            response.Error = "Su cuenta no está activa. Revise su correo electrónico y confirme su cuenta.";
            return response;
        }

        if (!user.IsActive)
        {
            response.HasError = true;
            response.Error = "Su cuenta está inactiva. Comuníquese con un administrador.";
            return response;
        }

        await _signInManager.SignInAsync(user, loginViewModel.RememberMe);
        response.Role = role;
        return response;
    }

    public async Task SignOutAsync()
    {
        await _signInManager.SignOutAsync();
    }

    public async Task<RegisterResponseDto> RegisterAsync(RegisterViewModel registerViewModel, string origin)
    {
        var response = new RegisterResponseDto();

        if (registerViewModel.Role != Roles.Cliente.ToString() && registerViewModel.Role != Roles.Agente.ToString())
        {
            response.Errors.Add("El tipo de usuario seleccionado no es válido.");
            return response;
        }

        var user = new ApplicationUser
        {
            FirstName = registerViewModel.FirstName,
            LastName = registerViewModel.LastName,
            UserName = registerViewModel.UserName,
            Email = registerViewModel.Email,
            PhoneNumber = registerViewModel.PhoneNumber,
            PhotoUrl = registerViewModel.PhotoUrl,
            EmailConfirmed = false,
            IsActive = false // Cliente: se activa al confirmar el correo. Agente: lo activa un administrador.
        };

        var result = await _userManager.CreateAsync(user, registerViewModel.Password);
        if (!result.Succeeded)
        {
            response.Errors.AddRange(result.Errors.Select(TranslateError));
            return response;
        }

        await _userManager.AddToRoleAsync(user, registerViewModel.Role);

        if (registerViewModel.Role == Roles.Cliente.ToString())
        {
            try
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                token = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));

                var uri = new Uri($"{origin}/Account/ConfirmEmail");
                var verificationUri = QueryHelpers.AddQueryString(uri.ToString(), "userId", user.Id);
                verificationUri = QueryHelpers.AddQueryString(verificationUri, "token", token);

                await _emailService.SendAsync(new EmailRequestDto
                {
                    To = user.Email!,
                    Subject = "Confirme su cuenta en RealEstateApp",
                    Body = $"<h3>Hola {user.FirstName}, bienvenido a RealEstateApp</h3>" +
                           $"<p>Para activar su cuenta haga clic en el siguiente enlace:</p>" +
                           $"<p><a href=\"{verificationUri}\">Confirmar mi cuenta</a></p>"
                });
            }
            catch
            {
                await _userManager.DeleteAsync(user);
                response.Errors.Add("No se pudo enviar el correo de confirmación. Intente nuevamente más tarde.");
            }
        }

        return response;
    }

    public async Task<(bool Succeeded, string Message)> ConfirmEmailAsync(string userId, string token)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user is null)
        {
            return (false, "No existe una cuenta asociada a este enlace.");
        }

        try
        {
            token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        }
        catch (FormatException)
        {
            return (false, "El enlace de confirmación no es válido.");
        }

        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
        {
            return (false, "Ocurrió un error al confirmar su cuenta. El enlace puede haber expirado.");
        }

        user.IsActive = true;
        await _userManager.UpdateAsync(user);
        return (true, "Su cuenta ha sido confirmada. Ya puede iniciar sesión.");
    }

    private static string TranslateError(IdentityError error) => error.Code switch
    {
        "DuplicateUserName" => "El nombre de usuario ya está en uso.",
        "DuplicateEmail" => "El correo electrónico ya está registrado.",
        "PasswordTooShort" => "La contraseña debe tener al menos 6 caracteres.",
        "PasswordRequiresDigit" => "La contraseña debe contener al menos un número.",
        _ => error.Description
    };
}
