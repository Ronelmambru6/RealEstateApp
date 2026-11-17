using RealEstateApp.Core.Application.ViewModels.Home;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IHomeService
{
    Task<List<HomePropertyViewModel>> GetFilteredPropertiesAsync(FilterPropertyViewModel filter);
    Task<PropertyDetailViewModel?> GetPropertyDetailByIdAsync(int id);
}