using System.Linq.Expressions;

namespace InvoiceApp.Interfaces
{
    public interface IGenericRepository <T> where T : class
    {
        Task<T> GetByIdAsync(int id, string? includeProperties = null);
        Task<IEnumerable<T>> GetAllAsync(string? includeProperties = null);       
        Task<IEnumerable<T>> GetByIdsAsync(IEnumerable<int> ids);

        Task<T?> GetByIdWithIncludesAsync(int id, params Expression<Func<T, object>>[] includes);
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(int id);

    }
}
