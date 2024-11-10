using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ETickets.Repository
{
    public class MovieRepository : Repository<Movie>, IMovieRepository
    {
        private readonly ETicketsAppDbContext context;
        public MovieRepository(ETicketsAppDbContext context) : base(context) => this.context = context;        
    }
}
