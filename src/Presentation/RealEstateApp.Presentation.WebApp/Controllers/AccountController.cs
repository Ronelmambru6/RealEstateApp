using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RealEstateApp.Core.Application.Enums;
using RealEstateApp.Core.Application.Helpers;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Account;

namespace RealEstateApp.Presentation.WebApp.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IWebHostEnvironment _environment;

    public AccountController(IAccountService accountService, IWebHostEnvironment environment)
    {
        _accountService = accountService;
        _environment = environment;
    }

    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(GetCurrentRole());
        }

        ViewData["ReturnUrl"] = returnUrl;
        return View(new LoginViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel loginViewModel, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;

        if (!ModelState.IsValid)
        {
            return View(loginViewModel);
        }

        var response = await _accountService.LoginAsync(loginViewModel);
        if (response.HasError)
        {
            ModelState.AddModelError(string.Empty, response.Error!);
            return View(loginViewModel);
        }

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
        {
            return LocalRedirect(returnUrl);
        }

        return RedirectByRole(response.Role);
    }

    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectByRole(GetCurrentRole());
        }

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel registerViewModel)
    {
        if (!ImageHelper.IsValidImage(registerViewModel.Photo))
        {
            ModelState.AddModelError(nameof(registerViewModel.Photo), "Debe seleccionar una foto de perfil válida (.jpg, .jpeg o .png)");
        }

        if (!ModelState.IsValid)
        {
            return View(registerViewModel);
        }

        registerViewModel.PhotoUrl = await ImageHelper.UploadAsync(registerViewModel.Photo, _environment.WebRootPath, "Users");

        var origin = $"{Request.Scheme}://{Request.Host}";
        var response = await _accountService.RegisterAsync(registerViewModel, origin);

        if (response.HasError)
        {
            ImageHelper.Delete(registerViewModel.PhotoUrl, _environment.WebRootPath);
            foreach (var error in response.Errors)
            {
                ModelState.AddModelError(string.Empty, error);
            }
            return View(registerViewModel);
        }

        TempData["SuccessMessage"] = registerViewModel.Role == Roles.Cliente.ToString()
            ? "Registro exitoso. Le enviamos un correo para que active su cuenta."
            : "Registro exitoso. Un administrador debe activar su cuenta antes de que pueda iniciar sesión.";

        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Index", "Home");
        }

        var (succeeded, message) = await _accountService.ConfirmEmailAsync(userId, token);
        ViewData["Succeeded"] = succeeded;
        ViewData["Message"] = message;
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _accountService.SignOutAsync();
        return RedirectToAction(nameof(Login));
    }

    [HttpGet]
    public IActionResult AccessDenied()
    {
        return View();
    }

    private string? GetCurrentRole()
    {
        if (User.IsInRole(Roles.Administrador.ToString())) return Roles.Administrador.ToString();
        if (User.IsInRole(Roles.Agente.ToString())) return Roles.Agente.ToString();
        if (User.IsInRole(Roles.Cliente.ToString())) return Roles.Cliente.ToString();
        return null;
    }

    private IActionResult RedirectByRole(string? role)
    {
        if (role == Roles.Administrador.ToString())
        {
            return RedirectToAction("Index", "Admin");
        }

        if (role == Roles.Agente.ToString())
        {
            return RedirectToAction("Index", "Agent");
        }

        // Cliente: su home es el catálogo con favoritas.
        return RedirectToAction("Index", "Home");
    }
}
