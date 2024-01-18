using System.Linq.Expressions;

namespace EnityFrameworkRelationShip.Interfaces.Repository;

public interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate);
    Task<T?> GetByIdAsync(Guid id);

    Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate);
    Task CreateAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
