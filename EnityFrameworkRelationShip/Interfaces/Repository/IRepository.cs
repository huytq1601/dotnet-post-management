namespace EnityFrameworkRelationShip.Interfaces.Repository;

public interface IRepository<T> where T : class, IBaseEntity
{
    IQueryable<T> GetAll();
    T? GetById(Guid id);
    void Create(T entity);
    void Update(T entity);
    void Delete(T entity);
    void SaveChanges();
}
