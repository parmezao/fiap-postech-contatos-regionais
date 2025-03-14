using ContatosRegionais.Domain.Entities;
using ContatosRegionais.Domain.Interfaces;
using ContatosRegionais.Infra.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace ContatosRegionais.Infra.Data.Repository;

public class BaseRepository<TEntity>(SqlServerDbContext context)
    : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    private readonly SqlServerDbContext _context = context;

    public async Task DeleteAsync(int id)
    {
        _context.Set<TEntity>().Remove((await SelectAsync(id))!);
        await _context.SaveChangesAsync();
    }

    public async Task InsertAsync(TEntity entity)
    {
        await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task<IList<TEntity>> SelectAllAsync() => await _context.Set<TEntity>().ToListAsync();

    public async Task<TEntity?> SelectAsync(int id) => await _context.Set<TEntity>().FindAsync(id);

    public async Task<IEnumerable<TEntity>> FilterAsync(Func<TEntity, bool> predicate) =>
        await Task.FromResult(_context.Set<TEntity>().Where(predicate));

    public async Task UpdateAsync(TEntity entity)
    {
        _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
    }
}

