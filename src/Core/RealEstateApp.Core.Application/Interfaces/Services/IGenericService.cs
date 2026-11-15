using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Interfaces.Services;

public interface IGenericService<TSaveViewModel, TViewModel, TEntity>
    where TSaveViewModel : class
    where TViewModel : class
    where TEntity : BaseEntity
{
    Task<List<TViewModel>> GetAllViewModelAsync();
    Task<TSaveViewModel?> GetByIdSaveViewModelAsync(int id);
    Task<TSaveViewModel> AddAsync(TSaveViewModel saveViewModel);
    Task UpdateAsync(TSaveViewModel saveViewModel, int id);
    Task DeleteAsync(int id);
}
