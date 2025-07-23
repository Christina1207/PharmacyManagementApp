using Domain.IRepositories;
using Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class Repository<T, TKey> : IRepository<T, TKey> where T : class
    {
        protected readonly PharmacyDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(PharmacyDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }


        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }

        public async Task DeleteByIdAsync(TKey id)
        {
            var entity = await GetByIdAsync(id);
            if (entity != null)
                Delete(entity);
        }

        public async Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes)
        {

            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query= query.Include(include);
            }


            return await query.ToListAsync();
        }

        public async Task<IEnumerable<T>> GetAllByPredicateAsync(Expression<Func<T, bool>> predicate,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.Where(predicate).ToListAsync();
        }

        public async Task<T?> GetByIdAsync(TKey id,params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            var parameter = Expression.Parameter(typeof(T), "e");
            var property = Expression.Property(parameter, "Id");
            var equals = Expression.Equal(property, Expression.Constant(id));
            var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
            return await query.FirstOrDefaultAsync(lambda);
        }

        public async Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }
    }
}
