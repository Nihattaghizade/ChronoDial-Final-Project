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
        public async Task<IActionResult> Search(ProductSearchVm vm)
        {
            var products = _context.Products.Where(x => !x.SoftDelete).Include(x => x.ProductImages).AsQueryable();

            if (vm.Name != null)
            {
                products = products.Where(x => x.Name.ToLower().StartsWith(vm.Name.ToLower()));
            }
            else
            {
                products = products.Where(x => x.Name.ToLower().StartsWith(vm.Name.ToLower()));
            }
            return View(await products.ToListAsync());
        }
    }
}
