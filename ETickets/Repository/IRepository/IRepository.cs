using ETickets.Models;
using System.Linq.Expressions;

namespace ETickets.Repository.IRepository
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T>? Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true);
        void Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void Commit();
    }
}
