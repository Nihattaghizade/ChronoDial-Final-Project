using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Controllers
{
	public class HomeController : Controller
	{
        private readonly AppDbContext _context;
        public HomeController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
		{
			return View();
		}
       
    }
}
