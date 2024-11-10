using ETickets.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;

namespace ETickets.Controllers
{
    public class UsersOrderController : Controller
    {
        private readonly IUsersOrdersRepository usersOrdersRepository;

        public UsersOrderController(IUsersOrdersRepository usersOrdersRepository)
        {
            this.usersOrdersRepository = usersOrdersRepository;
        }
        public IActionResult Index()
        {
            var orders = usersOrdersRepository.Get();
            return View(orders);
        }
    }
}
