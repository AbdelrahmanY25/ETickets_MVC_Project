using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using ETickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ETickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class MovieController(IMovieRepository movieRepository, ICategoryRepository categoryRepository, ICinemaRepository cinemaRepository, IActorRepository actorRepository, IActorMoviesRepository actorMoviesRepository) : Controller
    {
        private readonly IMovieRepository movieRepository = movieRepository;
        private readonly ICinemaRepository cinemaRepository = cinemaRepository;
        private readonly ICategoryRepository categoryRepository = categoryRepository;
        private readonly IActorRepository actorRepository = actorRepository;
        private readonly IActorMoviesRepository actorMoviesRepository = actorMoviesRepository;

        public IActionResult Index(int pageNumber = 1, string? query = null)
        {
            var movies = movieRepository.Get(includeProps: [i => i.Cinema, c => c.Category]);

            const int pageSize = 6;
            int totalMovies = movies.Count();
            double pageCount = Math.Ceiling((double)totalMovies / pageSize);    

            if (pageNumber - 1 < pageCount)
            {
                if (query != null)
                    movies = movieRepository.Get([c => c.Category, i => i.Cinema], expression: m => m.Name.Contains(query));
                movies = movies.Skip((pageNumber - 1) * pageSize).Take(pageSize);

                ViewBag.Count = pageCount;
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);
                ViewBag.pageNumber = pageNumber;
                return View(movies);
            }
            return RedirectToAction("NotFound", "Home");
        }
        public IActionResult Create()
        {
            ViewBag.Categories = categoryRepository.Get();
            ViewBag.Cinemas = cinemaRepository.Get();
            ViewBag.Actors = actorRepository.Get();
            return View(new Movie());
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Create(Movie movie, int[] arrOfActors, IFormFile ImgUrl)
        {
            ModelState.Remove(nameof(movie.ImgUrl));
            if(ModelState.IsValid)
            {
                if(ImgUrl.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

                    movie.ImgUrl = fileName;
                }
                movieRepository.Create(movie);
                movieRepository.Commit();

                ActorMovies actorMovies = new ActorMovies();

                for(int i = 0; i < arrOfActors.Length; i++)
                {
                    actorMovies.ActorsId = arrOfActors[i];                   
                    actorMovies.MoviesId = movie.Id;

                    actorMoviesRepository.Create(actorMovies);
                    actorMoviesRepository.Commit();
                }

                TempData["Added"] = "Movie Added Succussfully";
                return RedirectToAction("Index");
            }
            ViewBag.Categories = categoryRepository.Get();
            ViewBag.Cinemas = cinemaRepository.Get();
            ViewBag.Actors = actorRepository.Get();
            return View(movie);
        }
        public IActionResult Edit(int movieId)
        {
            var movie = movieRepository.GetOne(expression: m => m.Id == movieId);
            ViewBag.Categories = categoryRepository.Get();
            ViewBag.Cinemas = cinemaRepository.Get();
            ViewBag.Actors = actorRepository.Get();
            return View(movie);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Edit(Movie movie, IFormFile ImgUrl)
        {
            var oldMovie = movieRepository.GetOne(expression: m => m.Id == movie.Id, tracked: false);
            if(ModelState.IsValid)
            {
                if(ImgUrl.Length > 0)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", oldMovie.ImgUrl);

                    if(System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImgUrl.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\movies", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ImgUrl.CopyTo(stream);
                    }

                    movie.ImgUrl = fileName;
                } 
                else
                {
                    movie.ImgUrl = oldMovie.ImgUrl;
                }
                movieRepository.Update(movie);
                movieRepository.Commit();
                return RedirectToAction("Index");
            }
            ViewBag.Categories = categoryRepository.Get();
            ViewBag.Cinemas = cinemaRepository.Get();
            ViewBag.Actors = actorRepository.Get();
            movie.ImgUrl = oldMovie.ImgUrl;
            return View(movie);
        }
        public IActionResult Delete(int movieId)
        {
            var movie = movieRepository.GetOne(expression: m => m.Id == movieId);
            if (movie != null)
            {
                movieRepository.Delete(movie);
                movieRepository.Commit();
            }
            return RedirectToAction("Index");
        }        
    }
}
