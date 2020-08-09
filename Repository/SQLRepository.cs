using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Repository
{
    public class SQLRepository<T> : IBaseRepository<T> where T : BaseEntity
    {
        private readonly SQLContext _context;

        private DbSet<T> _entity;



        public SQLRepository(SQLContext context)
        {
            _context = context;
            _entity = context.Set<T>();
        }
        public int GetCount()
        {
            return _entity.Count();
        }

        public async Task<List<T>> GetAllSkipTake(int pageSize, int page)
        {
            return await _entity.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
        }

        public async Task<List<T>> GetAll()
        {
            return await _entity.ToListAsync();
        }
        public async Task<T> GetById(object id)
        {
            return await _entity.SingleOrDefaultAsync(s => s.Id == (Guid)id);
        }

        public async Task<List<T>> Where(Expression<Func<T, bool>> exp, string[] navProperties = null)
        {
            var data = _entity.Where(exp);
            if (navProperties != null)
            {
                foreach (var navProperty in navProperties)
                {
                    data = data.Include(navProperty);
                }
            }
            return await data.ToListAsync();
        }

        public async Task<List<T>> WhereOrdered(Expression<Func<T, bool>> exp, Expression<Func<T, object>> keyselector)
        {
            return await _entity.Where(exp).OrderByDescending(keyselector).ToListAsync();
        }
        public async Task<T> GetFirstWhereOrdered(Expression<Func<T, bool>> exp, Expression<Func<T, object>> keyselector)
        {
            return await _entity.Where(exp).OrderByDescending(keyselector).FirstOrDefaultAsync();
        }
        public async void Insert(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Input data is null");

            await _entity.AddAsync(entity);
            _context.SaveChanges();
        }

        public async void InsertRange(List<T> entities)
        {
            if (entities == null)
                throw new ArgumentNullException("Input data is null");

            await _entity.AddRangeAsync(entities);
            _context.SaveChanges();
        }

        public void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Input data is null");

            var oldEntity = _context.FindAsync<T>(entity.Id).Result;
            entity.UpdatedDate = DateTime.Now;
            _context.Entry(oldEntity).CurrentValues.SetValues(entity);
            _context.SaveChanges();
        }
        public void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("Input data is null");

            _entity.Remove(entity);
            _context.SaveChanges();
        }
    }

}
