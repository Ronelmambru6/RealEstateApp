namespace RealEstateApp.Core.Application.ViewModels.Admin;

public class AdminDashboardViewModel
{
    public int AvailablePropertiesCount { get; set; }
    public int SoldPropertiesCount { get; set; }
    public int ActiveAgentsCount { get; set; }
    public int InactiveAgentsCount { get; set; }
    public int ActiveClientsCount { get; set; }
    public int InactiveClientsCount { get; set; }
    public int ActiveDevelopersCount { get; set; }
    public int InactiveDevelopersCount { get; set; }
}