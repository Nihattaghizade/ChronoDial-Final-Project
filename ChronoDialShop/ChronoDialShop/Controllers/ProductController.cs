using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChronoDialShop.Controllers
{
	public class ProductController : Controller
	{
		private readonly AppDbContext _context;
		public ProductController(AppDbContext context)
		{
			_context = context;
		}
		public async Task<IActionResult> Index(int page = 1, int pageSize = 1)
		{
            //var products = _context.Products.AsQueryable();
            //products = products
            //   .Include(x => x.Brand)
            //   .Include(x => x.ProductImages)
            //   .Skip((page - 1) * pageSize)
            //   .Take(pageSize);
            //var count = GetPageCount(pageSize);
            //PaginateVm paginateVm = new PaginateVm()
            //{
            //    CurrentPage = page,
            //    TotalPageCount = count,
            //    Products = await products.ToListAsync()
            //};
            //return View(paginateVm);

            var books = await _context.Products.Where(x => !x.SoftDelete).Include(x => x.Brand)
              .Include(x => x.ProductImages)
              .Include(x => x.ProductSize)
              .ThenInclude(bt => bt.Size)
              .Include(x => x.Vendor).Take(8).ToListAsync();
            return View(books);
        }

        //public int GetPageCount(int pageSize)
        //{
        //    var productCount = _context.Products.Count();
        //    return (int)Math.Ceiling((decimal)productCount / pageSize);
        //}

        //public IActionResult ChangePage(int page = 1, int pageSize = 1)
        //{
        //    return ViewComponent("ProductMain", new { page = page, pageSize = pageSize });
        //}

        public  IActionResult ProductBrandFilter(int? id)
        {
            return ViewComponent("ProductMain", new { brandId = id });
        }

        public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
               .Include(x => x.Brand)
               .Include(x => x.ProductImages)
               .Include(x => x.ProductSize)
               .ThenInclude(x => x.Size)
               .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null) return NotFound();
            var brands = await _context.Brands.Include(x => x.Products).ToListAsync();
			
			ProductVm productVm = new ProductVm()
			{
				Product = product,
				Brands = brands
			};
            return View(productVm);
        }

        public async Task<IActionResult> AddToCart(int id)
        {
            var existProduct = await _context.Products.AnyAsync(x => x.Id == id);
            if (!existProduct) return NotFound();

            List<BasketVm>? basketVm = GetBasket();
            BasketVm cartVm = basketVm.Find(x => x.Id == id);
            if (cartVm != null)
            {
                cartVm.Count++;
            }
            else
            {
                basketVm.Add(new BasketVm
                {
                    Count = 1,
                    Id = id
                });
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVm));
            return RedirectToAction("Index");
        }
        private List<BasketVm> GetBasket()
        {
            List<BasketVm> basketVms;
            if (Request.Cookies["basket"] != null)
            {
                basketVms = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies["basket"]);
            }
            else basketVms = new List<BasketVm>();
            return basketVms;
        }

        [HttpPost]
        public async Task<IActionResult> SortBooks(int id)
        {

            var data = await _context.Products.Where(x => !x.SoftDelete).Include(x => x.Brand)
               .Include(x => x.ProductImages)
               .Include(x => x.ProductSize)
               .ThenInclude(bt => bt.Size)
                .Include(x => x.Vendor).Take(8).ToListAsync();
            switch (id)
            {
                case 1:
                    data = data.OrderBy(x => x.Name).ToList();
                    break;

                case 2:
                    data = data.OrderByDescending(x => x.Name).ToList();
                    break;
                case 3:
                    data = data.OrderBy(x => x.SellPrice).ToList();
                    break;
                case 4:
                    data = data.OrderByDescending(x => x.SellPrice).ToList();
                    break;
                case 5:
                    data = data.OrderBy(x => x.Rating).ToList();
                    break;
                case 6:
                    data = data.OrderByDescending(x => x.Rating).ToList();
                    break;
                default:
                    break;
            }
            return PartialView("_ProductPartial", data);
        }
    }
}
