using ETickets.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Identity;

namespace ETickets.Data
{
    public class ETicketsAppDbContext(DbContextOptions<ETicketsAppDbContext> options) :
                 IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Movie> Movies { get; set; }
        public DbSet<Actor> Actors { get; set; }
        public DbSet<Cinema> Cinemas { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ActorMovies> ActorMovies { get; set; }      
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<UsersOrders> usersOrders { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Movie>()
                .HasMany(e => e.Actors)
                .WithMany(e => e.Movies)
                .UsingEntity<ActorMovies>();

            modelBuilder.Entity<ApplicationUser>()
                .HasMany(e => e.Movies)
                .WithMany(e => e.ApplicationUsers)
                .UsingEntity<Ticket>();
        }
        public DbSet<ETickets.ViewModel.ApplicationUserlVM> ApplicationModelVM { get; set; } = default!;
        public DbSet<ETickets.ViewModel.LoginVM> LoginVM { get; set; } = default!;
    }
}
