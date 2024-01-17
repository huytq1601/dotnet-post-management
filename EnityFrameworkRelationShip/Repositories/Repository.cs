using EnityFrameworkRelationShip.Data;
using EnityFrameworkRelationShip.Interfaces;
using EnityFrameworkRelationShip.Interfaces.Repository;
using EnityFrameworkRelationShip.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using System;

namespace EnityFrameworkRelationShip.Repositories
{
    public class Repository<T> : IRepository<T> where T : class, IBaseEntity
    {
        private readonly DataContext _context;
        private DbSet<T> _table;

        public Repository(DataContext context)
        {
            _context = context;
            _table = context.Set<T>();
        }

        public IQueryable<T> GetAll()
        {
            return _table;
        }

        public T? GetById(Guid id)
        {
            return _table.SingleOrDefault(e => !e.IsDeleted && e.Id == id);
        }

        public void Create(T entity)
        {
           _table.Add(entity);
        }

        public void Update(T entity)
        {
            _table.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _table.Update(entity);
        }

        public void  SaveChanges()
        {
            _context.SaveChanges();
        }
    }
}
