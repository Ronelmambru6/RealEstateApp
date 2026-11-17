using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.ViewModels.Offer;

public class CreateOfferViewModel
{
    [Required]
    public int PropertyId { get; set; }

    [Required(ErrorMessage = "Debe ingresar una cifra para la oferta.")]
    [Range(1, double.MaxValue, ErrorMessage = "El monto de la oferta debe ser mayor a cero.")]
    [Display(Name = "Monto a Ofertar (DOP)")]
    public decimal Amount { get; set; }
}