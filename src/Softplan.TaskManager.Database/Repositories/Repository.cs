using Microsoft.EntityFrameworkCore;
using Softplan.TaskManager.Database.Context;

namespace Softplan.TaskManager.Database.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbset;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbset = context.Set<T>();
    }
    
    public async Task<IEnumerable<T>> GetAllAsync() => await _dbset.ToListAsync();

    public async Task<T?> GetByIdAsync(Guid id) => await _dbset.FindAsync(id);
  
    public async Task AddAsync(T entity)
    {
        await _dbset.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        _dbset.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity is not null)
        {
            _dbset.Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}