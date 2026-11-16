using RealEstateApp.Core.Application.Dtos.Account;
using RealEstateApp.Core.Application.ViewModels.Account;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IAccountService
{
    Task<AuthenticationResponseDto> LoginAsync(LoginViewModel loginViewModel);
    Task SignOutAsync();
    Task<RegisterResponseDto> RegisterAsync(RegisterViewModel registerViewModel, string origin);
    Task<(bool Succeeded, string Message)> ConfirmEmailAsync(string userId, string token);
}
