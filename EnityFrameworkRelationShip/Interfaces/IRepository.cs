using System.Linq.Expressions;

namespace EnityFrameworkRelationShip.Interfaces;

public interface IRepository<T> where T : class
{
    IQueryable<T> GetQuery();
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(Guid id);

    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
