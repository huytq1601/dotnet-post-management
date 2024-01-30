using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace EnityFrameworkRelationShip.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;
        private readonly DbSet<T> _table;

        public Repository(DataContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public IQueryable<T> GetQuery()
        {
            return _table.AsQueryable();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _table.AsNoTracking().ToListAsync();
        }

        public async Task<IEnumerable<T>> FindManyAsync(Expression<Func<T, bool>> predicate)
        {
            return await _table.AsNoTracking().Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _table.AsNoTracking().Where(predicate).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
           await _table.AddAsync(entity);
           await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
