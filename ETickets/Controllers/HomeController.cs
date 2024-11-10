using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ETickets.Controllers
{
    public class HomeController : Controller
    {
        private readonly ETicketsAppDbContext _context;
        private readonly IMovieRepository movieRepository;
        private readonly IActorMoviesRepository actorMoviesRepository;
        private readonly IActorRepository actorRepository;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IMovieRepository movieRepository, IActorMoviesRepository actorMoviesRepository, IActorRepository actorRepository, ETicketsAppDbContext context, ILogger<HomeController> logger)
        {
            this.movieRepository = movieRepository;
            this.actorMoviesRepository = actorMoviesRepository;
            this.actorRepository = actorRepository;
            _context = context;
            _logger = logger;
        }
        public IActionResult Index(int pageNumber = 1, string? query = null)
        {
            var movies = movieRepository.Get([ i => i.Cinema, c => c.Category ]);

            const int pageSize = 12;
            int totalMovies = movies.Count();
            double pageCount = Math.Ceiling((double)movies.Count() / pageSize);

            if(pageNumber - 1 < pageCount)
            {
                if (query != null)
                {
                    movies = movieRepository.Get([c => c.Category, i => i.Cinema], expression: m => m.Name.Contains(query));
                }
                movies = movies.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.PageNumber = pageNumber;

                return View(model: movies);                
            }

            return RedirectToAction("NotFound");            
        }

        public IActionResult Details(int movieId)
        {
            var movie = movieRepository.GetOne([m => m.Cinema, m => m.Category], expression: m => m.Id == movieId);
            
            if (movie == null) return NotFound();

            var actorIds = actorMoviesRepository.Get(expression: am => am.MoviesId == movieId).Select(am => am.ActorsId);

            var actors = actorRepository.Get(expression: a => actorIds.Contains(a.Id));

            ViewBag.actors = actors;
            return View(movie);
        }

        public new IActionResult NotFound()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
