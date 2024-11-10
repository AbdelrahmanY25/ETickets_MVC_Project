using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;

namespace ETickets.Repository
{
    public class ActorMoviesRepository : Repository<ActorMovies>, IActorMoviesRepository
    {
        private readonly ETicketsAppDbContext context;
        public ActorMoviesRepository(ETicketsAppDbContext context) : base(context) => this.context = context;
    }
}
