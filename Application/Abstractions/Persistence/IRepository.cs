using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Abstractions.Persistence.Repository;

public interface IRepository<T> where T : class
{
    Task<long> GetCount(Expression<Func<T, bool>>? filter = null);
    Task<bool> IsExistsAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(object id);
    Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes);
    Task<T?> GetByIdAsync(object id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = true, bool splitQuery = true);
    Task<IEnumerable<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        int? skip = null,
        int? take = null,
        params Expression<Func<T, object>>[] includes);
    Task<IEnumerable<T>> GetAllAsync(
       Expression<Func<T, bool>>? filter = null,
       Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
      Func<IQueryable<T>, IQueryable<T>>? include = null,
       int? skip = null,
       int? take = null,
       bool asNoTracking = true,
       bool splitQuery = true);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null);
    Task AddAsync(T entity);
    void Update(T entity);
    Task UpdateAsync(object id, T entity);
    void Remove(T entity);
    Task<int> SaveChangesAsync();
}