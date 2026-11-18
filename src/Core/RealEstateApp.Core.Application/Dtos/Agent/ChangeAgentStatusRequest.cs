using System.ComponentModel.DataAnnotations;

namespace RealEstateApp.Core.Application.Dtos.Agent;

public class ChangeAgentStatusRequest
{
    [Required(ErrorMessage = "El estado del agente es obligatorio.")]
    public bool? IsActive { get; set; }
}
