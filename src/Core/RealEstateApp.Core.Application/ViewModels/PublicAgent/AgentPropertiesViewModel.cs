using RealEstateApp.Core.Application.ViewModels.Home;

namespace RealEstateApp.Core.Application.ViewModels.PublicAgent;

public class AgentPropertiesViewModel
{
    public required PublicAgentViewModel Agent { get; set; }
    public required FilterPropertyViewModel Filter { get; set; }
}
