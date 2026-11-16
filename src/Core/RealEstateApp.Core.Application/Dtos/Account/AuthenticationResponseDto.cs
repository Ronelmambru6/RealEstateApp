namespace RealEstateApp.Core.Application.Dtos.Account;

public class AuthenticationResponseDto
{
    public bool HasError { get; set; }
    public string? Error { get; set; }
    public string? Role { get; set; }
}
