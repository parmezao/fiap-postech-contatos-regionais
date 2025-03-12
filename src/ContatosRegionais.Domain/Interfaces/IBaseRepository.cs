using ContatosRegionais.Domain.Entities;

namespace ContatosRegionais.Domain.Interfaces;
public interface IBaseRepository<TEntity> where TEntity : BaseEntity
{
    Task InsertAsync(TEntity entity);
    Task UpdateAsync(TEntity entity);
    Task DeleteAsync(int id);
    Task<IList<TEntity>> SelectAllAsync();
    Task<TEntity?> SelectAsync(int id);
    Task<IEnumerable<TEntity>> FilterAsync(Func<TEntity, bool> predicate);
}