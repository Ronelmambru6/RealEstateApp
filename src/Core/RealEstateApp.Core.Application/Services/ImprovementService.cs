using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Repositories;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Application.ViewModels.Improvement;
using RealEstateApp.Core.Domain.Entities;

namespace RealEstateApp.Core.Application.Services;

public class ImprovementService : GenericService<SaveImprovementViewModel, ImprovementViewModel, Improvement>, IImprovementService
{
    private readonly IGenericRepository<Improvement> _repository;

    public ImprovementService(IGenericRepository<Improvement> repository, IMapper mapper) : base(repository, mapper)
    {
        _repository = repository;
    }

    public async Task<bool> ExistsByNameAsync(string name, int? excludeId = null)
    {
        var improvements = await _repository.GetAllAsync();
        return improvements.Any(i =>
            i.Id != excludeId &&
            string.Equals(i.Name.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}
