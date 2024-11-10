using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class ActorRepository : Repository<Actor>, IActorRepository
    {
        private readonly ETicketsAppDbContext context;
        public ActorRepository(ETicketsAppDbContext context) : base(context) => this.context = context;       
    }
}
