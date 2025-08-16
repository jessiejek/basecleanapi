using System.Linq.Expressions;

namespace BaseStarterPack.Application.Interfaces.Common;

public interface IRepository<T> where T : class
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate,
                      string? includeProperties = null,
                      CancellationToken cancellationToken = default);
    Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null,
                                     string? includeProperties = null,
                                     Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                     CancellationToken cancellationToken = default);
    Task AddAsync(T entity, CancellationToken cancellationToken = default);
    void Update(T entity);
    void Remove(T entity);
}
