using AutoMapper;
using RealEstateApp.Core.Application.Interfaces.Repositories;
using RealEstateApp.Core.Application.Interfaces.Services;
using RealEstateApp.Core.Domain.Common;

namespace RealEstateApp.Core.Application.Services;

public class GenericService<TSaveViewModel, TViewModel, TEntity> : IGenericService<TSaveViewModel, TViewModel, TEntity>
    where TSaveViewModel : class
    where TViewModel : class
    where TEntity : BaseEntity
{
    private readonly IGenericRepository<TEntity> _repository;
    private readonly IMapper _mapper;

    public GenericService(IGenericRepository<TEntity> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public virtual async Task<List<TViewModel>> GetAllViewModelAsync()
    {
        var entities = await _repository.GetAllAsync();
        return _mapper.Map<List<TViewModel>>(entities);
    }

    public virtual async Task<TSaveViewModel?> GetByIdSaveViewModelAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        return entity is null ? null : _mapper.Map<TSaveViewModel>(entity);
    }

    public virtual async Task<TSaveViewModel> AddAsync(TSaveViewModel saveViewModel)
    {
        var entity = _mapper.Map<TEntity>(saveViewModel);
        entity = await _repository.AddAsync(entity);
        return _mapper.Map<TSaveViewModel>(entity);
    }

    public virtual async Task UpdateAsync(TSaveViewModel saveViewModel, int id)
    {
        var entity = _mapper.Map<TEntity>(saveViewModel);
        await _repository.UpdateAsync(entity, id);
    }

    public virtual async Task DeleteAsync(int id)
    {
        var entity = await _repository.GetByIdAsync(id);
        if (entity is not null)
        {
            await _repository.DeleteAsync(entity);
        }
    }
}
