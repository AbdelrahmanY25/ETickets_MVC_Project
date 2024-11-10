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
    public class CategoryController(ICategoryRepository categoryRepository, IMovieRepository movieRepository) : Controller
    {       
        private readonly ICategoryRepository categoryRepository = categoryRepository;
        private readonly IMovieRepository movieRepository = movieRepository;
        
        public IActionResult Index(int pageNumber = 1, string? query = null)
        {
            var categories = categoryRepository.Get();

            const int pageSize = 3;
            int totalCategories = categories.Count();
            double pageCount = Math.Ceiling((double)totalCategories / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                if (query != null)
                    categories = categoryRepository.Get(expression: c => c.Name.Contains(query));

                categories = categories.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);

                ViewBag.Count = pageCount;
                ViewBag.pageNumber = pageNumber;
                return View(categories);
            }
            return RedirectToAction("NotFound", "Home");
        }

        [AllowAnonymous]
        public IActionResult Details(int id)
        {
            var cats = movieRepository.Get(includeProps: [i => i.Cinema, c => c.Category], expression: m => m.CategoryId == id);
            return View(cats);
        }

        public IActionResult Create()
        {
            return View(new Category());
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            categoryRepository.Create(category);
            categoryRepository.Commit();
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var category = categoryRepository.GetOne(expression: m => m.Id == id);
            return View(category);
        }

        [HttpPost]
        public IActionResult Edit(Category category)
        {
            categoryRepository.Update(category);
            categoryRepository.Commit();
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var category = categoryRepository.GetOne(expression: m => m.Id == id);
            if (category != null)
            {
                categoryRepository.Delete(category);
                categoryRepository.Commit();
            } 
            return RedirectToAction("Index");
        }
    }
}
