namespace RealEstateApp.Core.Application.Dtos.Email;

public class EmailRequestDto
{
    public required string To { get; set; }
    public required string Subject { get; set; }
    public required string Body { get; set; }
    public string? From { get; set; }
}
