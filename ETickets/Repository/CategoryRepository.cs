using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class CategoryRepository : Repository<Category>, ICategoryRepository
    {
        private readonly ETicketsAppDbContext context;
        public CategoryRepository(ETicketsAppDbContext context) : base(context) => this.context = context;       
    }
}
