using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using System.Linq.Expressions;

namespace ETickets.Repository
{
    public class UsersOrdersRepository : Repository<UsersOrders>, IUsersOrdersRepository
    {
        private readonly ETicketsAppDbContext context;
        public UsersOrdersRepository(ETicketsAppDbContext context) : base(context) => this.context = context;       
    }
}
