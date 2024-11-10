using ETickets.Data;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace ETickets.Repository
{
    public class Repository<T>(ETicketsAppDbContext context) : IRepository<T> where T : class
    {
        private readonly ETicketsAppDbContext context = context;
        private DbSet<T> dbSet = context.Set<T>();

        public IEnumerable<T>? Get(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            IQueryable<T> query = dbSet;

            if(expression != null)
                query = query.Where(expression);
            if (includeProps != null)
            {
                foreach (var prop in includeProps)
                {
                    query = query.Include(prop);
                }
            }

            if (!tracked)
                query = query.AsNoTracking();

            return query.ToList();
        }

        public T? GetOne(Expression<Func<T, object>>[]? includeProps = null, Expression<Func<T, bool>>? expression = null, bool tracked = true)
        {
            return Get(includeProps, expression, tracked).FirstOrDefault();
        }

        public void Create(T entity)
        {
            dbSet.Add(entity);
        }
                
        public void Update(T entity)
        {
            dbSet.Update(entity);

        }
        public void Delete(T entity)
        {
            dbSet.Remove(entity);
        }

        public void Commit()
        {
            context.SaveChanges();
        }
    }
}
