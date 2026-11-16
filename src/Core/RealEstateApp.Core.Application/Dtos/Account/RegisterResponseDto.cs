namespace RealEstateApp.Core.Application.Dtos.Account;

public class RegisterResponseDto
{
    public bool HasError => Errors.Count > 0;
    public List<string> Errors { get; set; } = new();
}
