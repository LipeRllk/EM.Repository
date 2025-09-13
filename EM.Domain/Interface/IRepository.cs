using System.Linq.Expressions;

namespace EM.Domain.Interface
{
    public interface IRepository<T> where T : class, IEntidade
    {
        Task<T?> GetByIdAsync(int id);
        Task<List<T>> GetAllAsync();
        Task<List<T>> GetAsync(Expression<Func<T, bool>> predicate);
        
        Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate);
        
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task RemoveAsync(T entity);
        Task<bool> AnyAsync(Expression<Func<T, bool>> predicate);
        
        Task<int> CountAsync(Expression<Func<T, bool>> predicate);
    }
}