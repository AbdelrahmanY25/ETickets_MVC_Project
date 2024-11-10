using ETickets.Models;
using ETickets.Utility;
using ETickets.ViewModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace ETickets.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public AccountController(UserManager<ApplicationUser> userManager,
                                SignInManager<ApplicationUser> signInManager,
                                RoleManager<IdentityRole> roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
        }

        public async Task<IActionResult> Register()
        {
            if(roleManager.Roles.IsNullOrEmpty())
            {
                await roleManager.CreateAsync(new(SD.adminRole));
                await roleManager.CreateAsync(new(SD.cinemaRole));
                await roleManager.CreateAsync(new(SD.customerRole));
            }

            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(ApplicationUserlVM userVM)
        {
            if(ModelState.IsValid)
            {
                ApplicationUser applicationUser = new ApplicationUser()
                {
                    UserName = userVM.Name,
                    Email = userVM.Email,
                    City = userVM.City,
                };

                var result = await userManager.CreateAsync(applicationUser, userVM.Password);   

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(applicationUser, SD.customerRole);
                    await signInManager.SignInAsync(applicationUser, isPersistent: false);

                    return RedirectToAction("Index", "Home");
                }

                else
                {
                    ModelState.AddModelError("Password", "Invalid Password");
                }
            }
            return View(userVM);
        }

        public IActionResult Login()
        {
            signInManager.SignOutAsync();
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult>Login(LoginVM loginVM)
        {
            if (ModelState.IsValid)
            {
                var userDb = await userManager.FindByEmailAsync(loginVM.UserName);

                if (userDb != null)
                {
                     var finalResult = await userManager.CheckPasswordAsync(userDb, loginVM.Password);

                    if (finalResult)
                    {
                        await signInManager.SignInAsync(userDb, loginVM.RememberMe);
                        return RedirectToAction("Index", "Home");
                    }
                    else
                        ModelState.AddModelError("", "There is invalid user name or password");
                }
                else
                    ModelState.AddModelError("", "There is invalid user name or password");
            }

            return View(loginVM);
        }

        public IActionResult Logout()
        {
            signInManager.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
