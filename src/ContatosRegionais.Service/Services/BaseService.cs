using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;

namespace ContatosRegionais.Service.Services;

public class BaseService<TEntity>(IBaseRepository<TEntity> baseRepository)
    : IBaseService<TEntity> where TEntity : BaseEntity
{
    protected readonly IBaseRepository<TEntity> _baseRepository = baseRepository;

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        await _baseRepository.InsertAsync(entity);
        return entity;
    }

    public async Task DeleteAsync(int id) => await _baseRepository.DeleteAsync(id);

    public async Task<IList<TEntity>> GetAllAsync() => await _baseRepository.SelectAllAsync();

    public async Task<IEnumerable<TEntity>> FilterAsync(Func<TEntity, bool> predicate) =>
        await _baseRepository.FilterAsync(predicate);

    public async Task<TEntity> GetByIdAsync(int id) => (await _baseRepository.SelectAsync(id))!;

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        await _baseRepository.UpdateAsync(entity);
        return entity;
    }
}
