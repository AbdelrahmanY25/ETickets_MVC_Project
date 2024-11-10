using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class TicketRepository : Repository<Ticket>, ITicketRepository
    {
        private readonly ETicketsAppDbContext context;
        public TicketRepository(ETicketsAppDbContext context) : base(context) => this.context = context;
    }
}
