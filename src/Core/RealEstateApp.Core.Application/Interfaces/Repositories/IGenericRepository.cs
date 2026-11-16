namespace RealEstateApp.Core.Application.Interfaces.Repositories;

public interface IGenericRepository<TEntity> where TEntity : class
{
    Task<List<TEntity>> GetAllAsync();
    Task<List<TEntity>> GetAllWithIncludeAsync(List<string> properties);
    Task<TEntity?> GetByIdAsync(int id);
    Task<TEntity> AddAsync(TEntity entity);
    Task<TEntity?> UpdateAsync(TEntity entity, int id);
    Task DeleteAsync(TEntity entity);
}
