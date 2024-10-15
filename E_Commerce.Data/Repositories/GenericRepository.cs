using E_Commerce.Data.Contexts;
using E_Commerce.Domain.Commons;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace E_Commerce.Data.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : Auditable
{
    private readonly Context _context;
    private readonly DbSet<T> _dbSet;
    public GenericRepository(Context context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }


    public async ValueTask<T> CreateAsync(T entity) => (await _dbSet.AddAsync(entity)).Entity;

    public async ValueTask<bool> DeleteAsync(long id)
    {
        var entity = await GetAsync(x => x.Id == id);
        if (entity == null) return false;
        _dbSet.Remove(entity);
        return true;
    }

    public virtual IQueryable<T> GetAll(Expression<Func<T, bool>> expression, string[] includes = null!, bool isTracking = true)
    {
        var query = expression is null ? _dbSet : _dbSet.Where(expression);

        if(includes != null && includes.Length > 0)
            foreach (var include in includes)
                if(!string.IsNullOrWhiteSpace(include))
                    query = query.Include(include);

        if (!isTracking)
            query = query.AsNoTracking();
        return query;   
    }

    public async ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, string[] includes = null!, bool isTracking = true)
     => await GetAll(expression, includes, isTracking).FirstOrDefaultAsync();
 
    public T Update(T entity) => _dbSet.Update(entity).Entity;

    public async ValueTask SaveChangesAsync() => await _context.SaveChangesAsync();

}
