using System.Linq.Expressions;
using BaseStarterPack.Application.Interfaces.Common;
using BaseStarterPack.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace BaseStarterPack.Infrastructure.Repositories.Common;

public class Repository<T>(AppDbContext context) : IRepository<T> where T : class
{
    protected readonly AppDbContext Context = context;
    protected DbSet<T> DbSet => Context.Set<T>();

    public async Task<T?> GetAsync(Expression<Func<T, bool>> predicate, string? includeProperties = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet;
        if (!string.IsNullOrWhiteSpace(includeProperties))
            foreach (var inc in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(inc.Trim());
        return await query.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
    }

    public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, string? includeProperties = null, Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null, CancellationToken cancellationToken = default)
    {
        IQueryable<T> query = DbSet;
        if (predicate != null) query = query.Where(predicate);
        if (!string.IsNullOrWhiteSpace(includeProperties))
            foreach (var inc in includeProperties.Split(',', StringSplitOptions.RemoveEmptyEntries))
                query = query.Include(inc.Trim());
        if (orderBy != null) query = orderBy(query);
        return await query.AsNoTracking().ToListAsync(cancellationToken);
    }

    public async Task AddAsync(T entity, CancellationToken cancellationToken = default)
        => await DbSet.AddAsync(entity, cancellationToken);
    public void Update(T entity) => DbSet.Update(entity);
    public void Remove(T entity) => DbSet.Remove(entity);
}
