

using E_Commerce.Domain.Commons;
using System.Linq.Expressions;

namespace E_Commerce.Data.Repositories;

public interface IGenericRepository<T> where T : Auditable
{
    IQueryable<T> GetAll(Expression<Func<T, bool>> expression, string[] includes = null, bool isTracking = true);
    ValueTask<T> GetAsync(Expression<Func<T, bool>> expression, string[] includes = null, bool isTracking = true);
    ValueTask<T> CreateAsync(T entity);
    T Update(T entity);
    ValueTask<bool> DeleteAsync(long id);
    ValueTask SaveChangesAsync();
}
