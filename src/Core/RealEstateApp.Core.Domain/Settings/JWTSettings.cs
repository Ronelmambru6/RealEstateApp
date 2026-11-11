namespace RealEstateApp.Core.Domain.Settings;

public class JWTSettings
{
    public required string Key { get; set; }
    public required string Issuer { get; set; }
    public required string Audience { get; set; }
    public int DurationInMinutes { get; set; }
}