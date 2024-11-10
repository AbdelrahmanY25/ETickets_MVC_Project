using ETickets.Data;
using ETickets.Models;
using ETickets.Repository;
using ETickets.Repository.IRepository;
using ETickets.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Controllers
{
    [Authorize(Roles = $"{SD.adminRole}")]
    public class ActorController(IActorRepository actorRepository) : Controller
    {
        private readonly IActorRepository actorRepository = actorRepository;
        
        public IActionResult Index(int pageNumber = 1, string? query = null)
        {
            var actors = actorRepository.Get();
            const int pageSize = 10;
            int totalactors = actors.Count();
            double pageCount = Math.Ceiling((double)totalactors / pageSize);

            if (pageNumber - 1 < pageCount)
            {
                if (query != null)
                    actors = actorRepository.Get(expression: c => c.FirstName.Contains(query) || c.LastName.Contains(query));

                actors = actors.Skip((pageNumber - 1) * pageSize).Take(pageSize);
                pageNumber = Math.Clamp(pageNumber, 1, (int)pageCount);

                ViewBag.Count = pageCount;
                ViewBag.pageNumber = pageNumber;
                return View(actors);
            }
            return RedirectToAction("NotFound", "Home");
        }

        [AllowAnonymous]
        public IActionResult Details(int actorId)
        {
            var actor = actorRepository.GetOne(expression: a => a.Id == actorId);
            return View(actor);
        }
        public IActionResult Create()
        {
            return View(new Actor());
        }

        [HttpPost]
        public IActionResult Create(Actor actor, IFormFile ProfilePicture)
        {
            if (ModelState.IsValid)
            {
                if (ProfilePicture.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                    {
                        ProfilePicture.CopyTo(stream);
                    }

                    actor.ProfilePicture = fileName;
                }
                actorRepository.Create(actor);
                actorRepository.Commit();
                return RedirectToAction("Index");
            }
            return View(actor);
        }

        public IActionResult Edit(int id)
        {
            var actor = actorRepository.GetOne(expression: a => a.Id == id);
            return View(actor);
        }
        [HttpPost]
        public IActionResult Edit(Actor actor, IFormFile ProfilePicture)
        {
            var oldActor = actorRepository.GetOne(expression: a => a.Id == actor.Id, tracked: false);
            if(ModelState.IsValid)
            {
                if (ProfilePicture.Length > 0)
                {
                    var oldPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", oldActor.ProfilePicture);

                    if (System.IO.File.Exists(oldPath))
                        System.IO.File.Delete(oldPath);

                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(ProfilePicture.FileName);
                    var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images\\cast", fileName);

                    using (var stream = System.IO.File.Create(filePath))
                        ProfilePicture.CopyTo(stream);
                    actor.ProfilePicture = fileName;
                }
                else
                {
                    actor.ProfilePicture = oldActor.ProfilePicture;
                }
                actorRepository.Update(actor);
                actorRepository.Commit();
                return RedirectToAction("Index");
            }
            actor.ProfilePicture = oldActor.ProfilePicture;
            return View(actor);
        }

        public IActionResult Delete(int id)
        {
            var actor = actorRepository.GetOne(expression: a => a.Id == id);
            if (actor != null)
            {
                actorRepository.Delete(actor);
                actorRepository.Commit();
            }
            return RedirectToAction("Index");
        }
    }
}
