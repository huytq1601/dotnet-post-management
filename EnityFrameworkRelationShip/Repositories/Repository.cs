using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq.Expressions;

namespace EnityFrameworkRelationShip.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DataContext _context;
        private DbSet<T> _table;

        public Repository(DataContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _table.ToListAsync();
        }

        public async Task<IEnumerable<T>> SearchAsync(Expression<Func<T, bool>> predicate)
        {
            return await _table.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _table.FindAsync(id);
        }

        public async Task<T?> FindOneAsync(Expression<Func<T, bool>> predicate)
        {
            return await _table.Where(predicate).FirstOrDefaultAsync();
        }

        public async Task CreateAsync(T entity)
        {
           await _table.AddAsync(entity);
           await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }
    }
}
