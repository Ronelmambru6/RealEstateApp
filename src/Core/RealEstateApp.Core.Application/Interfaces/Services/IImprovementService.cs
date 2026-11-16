using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IImprovementService : IGenericService<SaveImprovementViewModel, ImprovementViewModel, Improvement>
{
    Task<bool> ExistsByNameAsync(string name, int? excludeId = null);
}
