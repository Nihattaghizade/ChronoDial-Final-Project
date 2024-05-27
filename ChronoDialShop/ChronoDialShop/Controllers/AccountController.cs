using ChronoDialShop.Models;
using Microsoft.AspNetCore.Mvc;

namespace ChronoDialShop.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(AppUser appUser)
        {
            return View();
        }
    }
}
