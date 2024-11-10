using ETickets.Data;
using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Repository
{
    public class CinemaRepository : Repository<Cinema>, ICinemaRepository
    {
        private readonly ETicketsAppDbContext context;
        public CinemaRepository(ETicketsAppDbContext context) : base(context) => this.context = context;              
    }
}
