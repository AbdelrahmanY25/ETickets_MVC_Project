using ETickets.Models;
using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using Stripe;

namespace ETickets.Controllers
{
    public class TicketController(ITicketRepository ticketRepository, IUsersOrdersRepository usersOrdersRepository, UserManager<ApplicationUser> userManager ) : Controller
    {
        private readonly ITicketRepository ticketRepository = ticketRepository;
        private readonly IUsersOrdersRepository usersOrdersRepository = usersOrdersRepository;
        private readonly UserManager<ApplicationUser> userManager = userManager;

        public IActionResult BookTicket(int movieId)
        {

            var appUser = userManager.GetUserId(User);

            if (appUser == null)
            {
                return RedirectToAction("Login", "Account");
            }
            else
            {
                Ticket ticket = new Ticket()
                {
                    MovieId = movieId,
                    ApplicationUserId = appUser
                };

                return View(ticket);
            }
        }

        [HttpPost]
        public IActionResult BookTicket(Ticket ticket)
        {
            if(ModelState.IsValid)
            {
                var oldUser = ticketRepository.GetOne(expression: e => e.MovieId == ticket.MovieId && e.ApplicationUserId == ticket.ApplicationUserId);

                if (oldUser != null)
                {
                    if (ticket.BookedTickets >= 1 && ticket.BookedTickets <= 100)
                    {
                        oldUser.BookedTickets += ticket.BookedTickets;
                       
                    }
                }
                else
                {
                    if (ticket.BookedTickets >= 1 && ticket.BookedTickets <= 100)
                    {
                        ticketRepository.Create(ticket);                       
                    }
                }

                ticketRepository.Commit();
                TempData["Added"] = "Ticket Booked Successfully";
                return RedirectToAction("Index", "Home");
            }
            
            return View(ticket);
        }

        public IActionResult Index()
        {
            var bookedTickets = ticketRepository.Get([e => e.Movie.Category,e => e.ApplicationUser], e => e.ApplicationUserId == userManager.GetUserId(User));
            if(bookedTickets != null)
                ViewBag.totalPrice = bookedTickets.Sum(e => e.Movie.Price * e.BookedTickets);
            return View(bookedTickets);
        }

        public IActionResult IncreaseTickets(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var selectedMovie = ticketRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (selectedMovie != null)
            {
                if (selectedMovie.BookedTickets < 100)
                {
                    selectedMovie.BookedTickets++;
                    ticketRepository.Update(selectedMovie);
                    ticketRepository.Commit();
                }
            }                
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseTickets(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var selectedMovie = ticketRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (selectedMovie != null)
            {
                if(selectedMovie.BookedTickets > 0)
                {
                    selectedMovie.BookedTickets--;
                    ticketRepository.Update(selectedMovie);
                    ticketRepository.Commit();
                }
                if(selectedMovie.BookedTickets == 0)
                {
                    ticketRepository.Delete(selectedMovie);
                    ticketRepository.Commit();
                }
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTicket(int movieId)
        {
            var appUser = userManager.GetUserId(User);
            var selectedMovie = ticketRepository.GetOne(expression: e => e.MovieId == movieId && e.ApplicationUserId == appUser);

            if (selectedMovie != null)
            {
                ticketRepository.Delete(selectedMovie);
                ticketRepository.Commit();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Pay()
        {
            var userApp = userManager.GetUserId(User);
            var bookedTickets = ticketRepository.Get([e => e.Movie], expression: e => e.ApplicationUserId == userApp);

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                LineItems = new List<SessionLineItemOptions>
            {                      
            },
                Mode = "payment",
                SuccessUrl = $"{Request.Scheme}://{Request.Host}/Ticket/DecrementQuantity",
                CancelUrl = $"{Request.Scheme}://{Request.Host}/Home/Index",
            };

            foreach(var ticket in bookedTickets)
            {
                options.LineItems.Add(new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        Currency = "USD",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = ticket.Movie.Name,
                        },
                        UnitAmount = (long)Math.Ceiling(ticket.Movie.Price * 100),
                    },
                    Quantity = ticket.BookedTickets,
                });
            }

            var service = new SessionService();
            var session = service.Create(options);
            return Redirect(session.Url);
        }

        public IActionResult DecrementQuantity()
        {
            var payedTickets = ticketRepository.Get([e => e.Movie, e => e.ApplicationUser], expression: e => e.ApplicationUserId == userManager.GetUserId(User));

            if (payedTickets != null)
            {
                foreach(var tick in payedTickets)
                {
                    tick.Movie.NumberOfTickets -= tick.BookedTickets;

                    UsersOrders usersOrders = new UsersOrders()
                    {
                        ApplicationUserId = tick.ApplicationUserId,
                        ApplicationName = tick.ApplicationUser.UserName,
                        MovieId = tick.Movie.Id,
                        MovieName = tick.Movie.Name,
                        MoviePrice = tick.Movie.Price,
                        NumberOfTickets = tick.BookedTickets,
                    };

                    usersOrdersRepository.Create(usersOrders);
                    ticketRepository.Delete(tick);
                }
            }

            ticketRepository.Commit();

            TempData["Payed"] = "Tickets purchased successfully";

            return RedirectToAction("Index", "Home");
        }
    }
}
