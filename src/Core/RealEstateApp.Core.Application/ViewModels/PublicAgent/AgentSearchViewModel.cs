namespace RealEstateApp.Core.Application.ViewModels.PublicAgent;

public class AgentSearchViewModel
{
    public string? Search { get; set; }
    public List<PublicAgentViewModel> Agents { get; set; } = new();
}
