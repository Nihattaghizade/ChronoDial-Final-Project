using ChronoDialShop.Data;
using ChronoDialShop.Models;
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



        [HttpPost]
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 3)
            {
                return BadRequest("Search term must be at least 3 characters long.");
            }

            var searchResults = await _context.Products
                .Where(p => p.Name.Contains(searchTerm) && !p.SoftDelete)
                .Select(p => new
                {
                    p.Id,
                    p.Name,
                    p.SellPrice,
                    ImageUrl = "/client/image/product/" + p.ProductImages.FirstOrDefault(x => x.IsMain).Url
                })
                .ToListAsync();

            return Json(searchResults);
        }
    }
}
