using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using ETickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ETickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class CinemaController(ICinemaRepository cinemaRepository, IMovieRepository movieRepository) : Controller
    {
        private readonly ICinemaRepository cinemaRepository = cinemaRepository;
        private readonly IMovieRepository movieRepository = movieRepository;
        
        public IActionResult Index(int pageNumber = 1, string? query = null)
        {
            var cinemas = cinemaRepository.Get();
            const int pageSize = 3;
            int totalCategories = cinemas.Count();
            double pageCount = Math.Ceiling((double)totalCategories / pageSize);
            if (pageNumber - 1 < pageCount)
            {
                if (query != null)
                    cinemas = cinemaRepository.Get(expression: c => c.Name.Contains(query));

                cinemas = cinemas.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);

                ViewBag.Count = pageCount;
                ViewBag.pageNumber = pageNumber;
                return View(cinemas);
            }
            return RedirectToAction("NotFound", "Home");
        }
        public IActionResult Details(int id)
        {
            var moviesInCinemas = movieRepository.Get(includeProps: [i => i.Cinema, c => c.Category], expression: m => m.CinemaId == id);
            return View(moviesInCinemas);
        }
        public IActionResult Create()
        {            
            return View(new Cinema());
        }

        [HttpPost]
        public IActionResult Create(Cinema cinema)
        {
            cinemaRepository.Create(cinema);
            cinemaRepository.Commit();
            return RedirectToAction("Index");
        }
        public IActionResult Edit(int cinemaId)
        {
            var cinema = cinemaRepository.GetOne(expression: c => c.Id == cinemaId);
            return View(cinema);
        }
        [HttpPost]
        public IActionResult Edit(Cinema cinema)
        {
            cinemaRepository.Update(cinema);
            cinemaRepository.Commit();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int cinemaId)
        {           
            var cinema = cinemaRepository.GetOne(expression: c => c.Id == cinemaId);
            if (cinema != null)
            {
                cinemaRepository.Delete(cinema);
                cinemaRepository.Commit();
            }
            return RedirectToAction("Index");
        }
        
    }
}
