using Application.Abstractions.Persistence.Repository;
using Infrastructure.Persistence.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Persistence.Services;

public class Repository<T>(CrmDbContext context) : IRepository<T> where T : class
{
    public async Task<long> GetCount(Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();
        if (filter is not null)
            query = query.Where(filter);
        return await query.CountAsync();
    }

    public async Task<bool> IsExistsAsync(Expression<Func<T, bool>> predicate)
    {
        if (predicate == null) throw new ArgumentNullException(nameof(predicate));
        return await context.Set<T>().AsNoTracking().AnyAsync(predicate);
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await context.Set<T>().FindAsync(id);
    }

    public async Task<T?> GetByIdAsync(object id, Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null, bool asNoTracking = true, bool splitQuery = true)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();

        if (include is not null)
            query = include(query);

        if (splitQuery)
            query = query.AsSplitQuery();

        var keyProperty = context.Model.FindEntityType(typeof(T))?
            .FindPrimaryKey()?.Properties.FirstOrDefault()
            ?? throw new InvalidOperationException($"'{typeof(T).Name}' entity'sinde birincil anahtar tanımlı değil.");

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, keyProperty.Name);

        // id'yi PK tipine çevir → tip uyumsuzluğunu önler
        var typedId = Convert.ChangeType(id, property.Type);
        var equals = Expression.Equal(property, Expression.Constant(typedId, property.Type));
        var lambda = Expression.Lambda<Func<T, bool>>(equals, param);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.SingleOrDefaultAsync(lambda);
    }

    public async Task<T?> GetByIdAsync(object id, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = context.Set<T>();

        if (includes is not null && includes.Any())
        {
            foreach (var include in includes)
            {
                if (include != null)
                {
                    try
                    {
                        query = query.Include(include);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        var keyProperty = context.Model.FindEntityType(typeof(T))?
            .FindPrimaryKey()?.Properties.FirstOrDefault();

        if (keyProperty == null)
            throw new InvalidOperationException($"'{typeof(T).Name}' entity'sinde birincil anahtar tanımlı değil.");

        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, keyProperty.Name);
        var equals = Expression.Equal(property, Expression.Constant(id));
        var lambda = Expression.Lambda<Func<T, bool>>(equals, param);

        return await query.AsNoTracking().SingleOrDefaultAsync(lambda);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, Func<IQueryable<T>, IQueryable<T>>? include = null, int? skip = null, int? take = null, bool asNoTracking = true, bool splitQuery = true)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();

        if (include is not null)
            query = include(query);

        if (filter is not null)
            query = query.Where(filter);

        if (orderBy is not null)
            query = orderBy(query);

        if ((skip.HasValue || take.HasValue) && orderBy is null)
            query = ApplyDefaultPrimaryKeyOrdering(query);

        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        if (splitQuery) query = query.AsSplitQuery();

        return asNoTracking
            ? await query.AsNoTracking().ToListAsync()
            : await query.ToListAsync();
    }


    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, int? skip = null, int? take = null, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();

        if (includes is not null && includes.Any())
        {
            foreach (var include in includes)
            {
                if (include != null)
                {
                    try
                    {
                        query = query.Include(include);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        if (filter != null)
            query = query.Where(filter);

        if (orderBy != null)
            query = orderBy(query);

        if ((skip.HasValue || take.HasValue) && orderBy is null)
            query = ApplyDefaultPrimaryKeyOrdering(query);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.AsNoTracking().ToListAsync();
    }

    private IQueryable<T> ApplyDefaultPrimaryKeyOrdering(IQueryable<T> query)
    {
        var primaryKey = context.Model.FindEntityType(typeof(T))?.FindPrimaryKey()
            ?? throw new InvalidOperationException($"'{typeof(T).Name}' entity'sinde birincil anahtar tanımlı değil.");
        var entityParameter = Expression.Parameter(typeof(T), "entity");
        IOrderedQueryable<T>? orderedQuery = null;

        foreach (var keyProperty in primaryKey.Properties)
        {
            var keyAccess = Expression.Call(
                typeof(EF),
                nameof(EF.Property),
                [keyProperty.ClrType],
                entityParameter,
                Expression.Constant(keyProperty.Name));
            var keySelector = Expression.Lambda(keyAccess, entityParameter);
            var sourceExpression = (orderedQuery ?? query).Expression;
            var orderingExpression = Expression.Call(
                typeof(Queryable),
                orderedQuery is null ? nameof(Queryable.OrderBy) : nameof(Queryable.ThenBy),
                [typeof(T), keyProperty.ClrType],
                sourceExpression,
                Expression.Quote(keySelector));

            orderedQuery = (IOrderedQueryable<T>)query.Provider.CreateQuery<T>(orderingExpression);
        }

        return orderedQuery
            ?? throw new InvalidOperationException($"'{typeof(T).Name}' entity'sinin birincil anahtar alanı bulunamadı.");
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();

        if (include is not null)
            query = include(query);

        return await query.AsNoTracking().AsSplitQuery().SingleOrDefaultAsync(predicate);
    }

    public async Task<T?> SingleOrDefaultAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = context.Set<T>().AsQueryable();

        if (includes is not null && includes.Any())
        {
            foreach (var include in includes)
            {
                if (include != null)
                {
                    try
                    {
                        query = query.Include(include);
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
        }

        return await query.AsNoTracking().AsSplitQuery().SingleOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        await context.Set<T>().AddAsync(entity);
    }

    public void Update(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        var idProp = context.Entry(entity).Property("Id").CurrentValue;
        var local = context.Set<T>().Local.FirstOrDefault(e => context.Entry(e).Property("Id").CurrentValue?.Equals(idProp) == true);
        if (local != null)
            context.Entry(local).State = EntityState.Detached;
        context.Set<T>().Attach(entity);
        context.Entry(entity).State = EntityState.Modified;

        //if (entity == null) throw new ArgumentNullException(nameof(entity)); 
        //context.Set<T>().Update(entity);
    }

    public void Remove(T entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        context.Set<T>().Remove(entity);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await context.SaveChangesAsync();
    }

    public async Task UpdateAsync(object id, T entity)
    {
        var existing = await context.Set<T>().FindAsync(id) ?? throw new KeyNotFoundException("Entity not found");
        context.Entry(existing).CurrentValues.SetValues(entity);

        var existingEntry = context.Entry(existing);
        foreach (var reference in existingEntry.References)
        {
            if (!reference.Metadata.TargetEntityType.IsOwned())
            {
                continue;
            }

            var propertyInfo = reference.Metadata.PropertyInfo;
            if (propertyInfo is null)
            {
                continue;
            }

            var updatedOwnedValue = propertyInfo.GetValue(entity);
            if (updatedOwnedValue is null)
            {
                reference.CurrentValue = null;
                continue;
            }

            if (reference.CurrentValue is null)
            {
                reference.CurrentValue = updatedOwnedValue;
            }
            else
            {
                context.Entry(reference.CurrentValue).CurrentValues.SetValues(updatedOwnedValue);
            }

            context.Entry(reference.CurrentValue!).State = EntityState.Modified;
        }
    }
}