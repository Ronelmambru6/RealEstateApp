using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Chat;

public class SendChatMessageViewModel
{
    public int PropertyId { get; set; }

    // Solo lo usa el agente para indicar a qué cliente responde.
    public string? ClientId { get; set; }

    [Required(ErrorMessage = "Debe escribir un mensaje")]
    [StringLength(1000, ErrorMessage = "El mensaje no puede tener más de 1000 caracteres")]
    [Display(Name = "Mensaje")]
    public string Content { get; set; } = string.Empty;
}
