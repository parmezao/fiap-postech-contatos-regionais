using ContatosRegionais.Domain.Entities;

namespace ContatosRegionais.Domain.Interfaces;

public interface IBaseService<TEntity> where TEntity : BaseEntity
{
    Task<TEntity> AddAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<IList<TEntity>> GetAllAsync();
    Task<IEnumerable<TEntity>> FilterAsync(Func<TEntity, bool> predicate);
    Task<TEntity> GetByIdAsync(int id);
    Task<TEntity> UpdateAsync(TEntity entity);
}