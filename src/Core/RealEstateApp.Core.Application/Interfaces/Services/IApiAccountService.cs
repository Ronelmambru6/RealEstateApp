using RealEstateApp.Core.Application.Dtos.Account;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IApiAccountService
{
    Task<AuthenticationResponse> AuthenticateAsync(AuthenticationRequest request);
    Task<RegisterApiUserResponse> RegisterUserWithRoleAsync(RegisterApiUserRequest request, string roleName);
}