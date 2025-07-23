using System.Linq.Expressions;

namespace Domain.IRepositories;
public interface IRepository<T, TKey> where T : class
{
    public Task<IEnumerable<T>> GetAllAsync(params Expression<Func<T, object>>[] includes);
    public Task<IEnumerable<T>> GetAllByPredicateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);
    public Task<T?> GetByIdAsync(TKey id, params Expression<Func<T, object>>[] includes);
    public Task<T?> GetByPredicateAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includes);


    public Task AddAsync(T entity);
    public void Update(T entity);
    public void Delete(T entity);
    public Task DeleteByIdAsync(TKey id);
    public Task SaveAsync();
}