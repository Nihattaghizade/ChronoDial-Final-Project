using ChronoDialShop.Data;
using ChronoDialShop.Models;
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
		public async Task<IActionResult> Index(int page = 1, int pageSize = 10)
		{
			var products = _context.Products.Where(x => !x.SoftDelete).AsQueryable();
			products = products
			   .Include(x => x.ProductImages)
			   .Skip((page - 1) * pageSize)
			   .Take(pageSize);
			var count = GetPageCount(pageSize);
			PaginateVm paginateVm = new PaginateVm()
			{
				CurrentPage = page,
				TotalPageCount = count,
				Products = await products.ToListAsync()
			};
			return View(paginateVm);
        }

		public int GetPageCount(int pageSize)
		{
			var productCount = _context.Products.Count();
			return (int)Math.Ceiling((decimal)productCount / pageSize);
		}

		public IActionResult ChangePage(int page = 1, int pageSize = 10)
		{
			return PartialView("_ProductPartial", new { page = page, pageSize = pageSize });
		}

		public IActionResult ProductBrandFilter(int? id)
		{
			return ViewComponent("ProductMain", new { brandId = id });
		}

		public async Task<IActionResult> Detail(int? id)
        {
            if (id == null) return NotFound();
            var product = await _context.Products
               .Include(x => x.Brand)
			   .Include(x => x.BandType)
			   .Include(x => x.Vendor)
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

        public async Task<IActionResult> Cart()
        {
            var products = await _context.Products.Where(x => !x.SoftDelete).Include(x => x.Brand)
              .Include(x => x.ProductImages)
              .Include(x => x.ProductSize)
              .ThenInclude(bt => bt.Size)
              .Include(x => x.Vendor).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> Wishlist()
        {
            var products = await _context.Products.Where(x => !x.SoftDelete).Include(x => x.Brand)
              .Include(x => x.ProductImages)
              .Include(x => x.ProductSize)
              .ThenInclude(bt => bt.Size)
              .Include(x => x.Vendor).ToListAsync();
            return View(products);
        }

        public async Task<IActionResult> AddToWishlist(int id)
        {
            var existProduct = await _context.Products.AnyAsync(x => x.Id == id);
            if (!existProduct) return NotFound();

            List<WishlistVm>? basketVm = GetWishlist();
            WishlistVm cartVm = basketVm.Find(x => x.Id == id);
            if(cartVm == null) 
            {
                basketVm.Add(new WishlistVm
                {
                    Count = 1,
                    Id = id
                });
            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVm));
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> RemoveFromWishlist(int id)
        {
            List<WishlistVm>? basketVm = GetWishlist();
            WishlistVm cartVm = basketVm.Find(x => x.Id == id);

            if (cartVm != null)
            {
                basketVm.Remove(cartVm);

            }
            Response.Cookies.Append("basket", JsonConvert.SerializeObject(basketVm));
            return RedirectToAction("Index");
        }


        private List<WishlistVm> GetWishlist()
        {
            List<WishlistVm> basketVms;
            if (Request.Cookies["basket"] != null)
            {
                basketVms = JsonConvert.DeserializeObject<List<WishlistVm>>(Request.Cookies["basket"]);
            }
            else basketVms = new List<WishlistVm>();
            return basketVms;
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
        



        public async Task<IActionResult> RemoveFromCart(int id)
        {
            List<BasketVm>? basketVm = GetBasket();
            BasketVm cartVm = basketVm.Find(x => x.Id == id);

            if (cartVm != null)
            {
                basketVm.Remove(cartVm);
               
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

        //[HttpPost]
        //public async Task<IActionResult> SortProducts(int id, ProductFilterVM dto)
        //{

        //	var data = await FilterProducts(dto);
        //	switch (id)
        //	{
        //		case 1:
        //			data = data.OrderBy(Resume => Resume.Name).ToList();
        //			break;

        //		case 2:
        //			data = data.OrderByDescending(Resume => Resume.Name).ToList();
        //			break;
        //		case 3:
        //			data = data.OrderByDescending(Resume => Resume.SellPrice).ToList();
        //			break;
        //		case 4:
        //			data = data.OrderBy(Resume => Resume.SellPrice).ToList();
        //			break;
        //		default:
        //			break;
        //	}
        //	return PartialView("_ProductPartial", data);
        //}

        //[HttpPost]
        //public async Task<List<Product>> FilterProducts(ProductFilterVM dto)
        //{
        //	var query = await _context.Products
        //						.Include(x => x.ProductImages)
        //						.Include(x => x.Brand)
        //						.Include(x => x.BandType)
        //						.Include(x => x.Vendor)
        //						.Include(x => x.InnerColor)
        //						.Include(x => x.Visualization)
        //						.Where(x => !x.SoftDelete)
        //						.ToListAsync();


        //	// Filter by brand IDs
        //	if (dto.BrandsIds != null && dto.BrandsIds.Count > 0)
        //	{
        //		var activeBrands = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
        //		var brands = activeBrands.Where(cat =>
        //			dto.BrandsIds.Any(brandId => cat.Id == brandId)
        //		);

        //		query = query.Where(c => brands.Any(cat => cat.Id == c.Brand.Id)).ToList();

        //	}

        //	// Filter by bandtype IDs
        //	if (dto.BandTypesIds != null && dto.BandTypesIds.Count > 0)
        //	{
        //		var activeBandTypes = await _context.BandTypes.Where(x => !x.SoftDelete).ToListAsync();
        //		var bandTypes = activeBandTypes.Where(edu =>
        //			dto.BandTypesIds.Any(btId => edu.Id == btId)
        //		);

        //		query = query.Where(c => bandTypes.Any(edu => edu.Id == c.BandType.Id)).ToList();

        //	}

        //	// Filter by vendor IDs
        //	if (dto.VendorsIds != null && dto.VendorsIds.Count > 0)
        //	{
        //		var activeVendors = await _context.Vendors.Where(x => !x.SoftDelete).ToListAsync();
        //		var vendors = activeVendors.Where(lan =>
        //			dto.VendorsIds.Any(venId => lan.Id == venId)
        //		);

        //		query = query.Where(c => vendors.Any(lan => lan.Id == c.Vendor.Id)).ToList();
        //	}

        //	// Filter by visualization IDs
        //	if (dto.VisualizationsIds != null && dto.VisualizationsIds.Count > 0)
        //	{
        //		var activeVisualizations = await _context.Visualizations.Where(x => !x.SoftDelete).ToListAsync();
        //		var visualizations = activeVisualizations.Where(lan =>
        //			dto.VisualizationsIds.Any(visId => lan.Id == visId)
        //		);

        //		query = query.Where(c => visualizations.Any(lan => lan.Id == c.Visualization.Id)).ToList();
        //	}

        //	// Filter by innercolor IDs
        //	if (dto.InnerColorsIds != null && dto.InnerColorsIds.Count > 0)
        //	{
        //		var activeInnerColors = await _context.InnerColors.Where(x => !x.SoftDelete).ToListAsync();
        //		var innerColors = activeInnerColors.Where(lan =>
        //			dto.InnerColorsIds.Any(icId => lan.Id == icId)
        //		);

        //		query = query.Where(c => innerColors.Any(lan => lan.Id == c.InnerColor.Id)).ToList();
        //	}

        //	// Filter by minimum salary
        //	//if (dto.MinPrice > 0)
        //	//{
        //	//	query = query.Where(Resume => Resume.SellPrice >= dto.MinPrice).ToList();
        //	//}

        //	//// Filter by maximum salary
        //	//if (dto.MaxPrice < 20001)
        //	//{
        //	//	query = query.Where(Resume => Resume.SellPrice <= dto.MaxPrice).ToList();
        //	//}

        //	//// Filter by selected gender
        //	//if (dto.Gender != Gender.None)
        //	//{
        //	//	query = query.Where(Resume => Resume.Gender == dto.Gender).ToList();
        //	//}
        //	return query;

        //}

        //[HttpPost]
        //public async Task<IActionResult> FilterViewProducts(ProductFilterVM dto)
        //{
        //	var query = await _context.Products
        //						.Include(x => x.ProductImages)
        //						.Include(x => x.Brand)
        //						.Include(x => x.BandType)
        //						.Include(x => x.Vendor)
        //						.Include(x => x.InnerColor)
        //						.Include(x => x.Visualization)
        //						.Where(x => !x.SoftDelete)
        //						.ToListAsync();


        //	// Filter by brand IDs
        //	if (dto.BrandsIds != null && dto.BrandsIds.Count > 0)
        //	{
        //		var activeBrands = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
        //		var brands = activeBrands.Where(cat =>
        //			dto.BrandsIds.Any(brandId => cat.Id == brandId)
        //		);

        //		query = query.Where(c => brands.Any(cat => cat.Id == c.Brand.Id)).ToList();

        //	}

        //	// Filter by bandtype IDs
        //	if (dto.BandTypesIds != null && dto.BandTypesIds.Count > 0)
        //	{
        //		var activeBandTypes = await _context.BandTypes.Where(x => !x.SoftDelete).ToListAsync();
        //		var bandTypes = activeBandTypes.Where(edu =>
        //			dto.BandTypesIds.Any(btId => edu.Id == btId)
        //		);

        //		query = query.Where(c => bandTypes.Any(edu => edu.Id == c.BandType.Id)).ToList();

        //	}

        //	// Filter by vendor IDs
        //	if (dto.VendorsIds != null && dto.VendorsIds.Count > 0)
        //	{
        //		var activeVendors = await _context.Vendors.Where(x => !x.SoftDelete).ToListAsync();
        //		var vendors = activeVendors.Where(lan =>
        //			dto.VendorsIds.Any(venId => lan.Id == venId)
        //		);

        //		query = query.Where(c => vendors.Any(lan => lan.Id == c.Vendor.Id)).ToList();
        //	}

        //	// Filter by visualization IDs
        //	if (dto.VisualizationsIds != null && dto.VisualizationsIds.Count > 0)
        //	{
        //		var activeVisualizations = await _context.Visualizations.Where(x => !x.SoftDelete).ToListAsync();
        //		var visualizations = activeVisualizations.Where(lan =>
        //			dto.VisualizationsIds.Any(visId => lan.Id == visId)
        //		);

        //		query = query.Where(c => visualizations.Any(lan => lan.Id == c.Visualization.Id)).ToList();
        //	}

        //	// Filter by innercolor IDs
        //	if (dto.InnerColorsIds != null && dto.InnerColorsIds.Count > 0)
        //	{
        //		var activeInnerColors = await _context.InnerColors.Where(x => !x.SoftDelete).ToListAsync();
        //		var innerColors = activeInnerColors.Where(lan =>
        //			dto.InnerColorsIds.Any(icId => lan.Id == icId)
        //		);

        //		query = query.Where(c => innerColors.Any(lan => lan.Id == c.InnerColor.Id)).ToList();
        //	}

        //	// Filter by minimum salary
        //	//if (dto.MinPrice > 0)
        //	//{
        //	//    query = query.Where(Resume => Resume.SellPrice >= dto.MinPrice).ToList();
        //	//}

        //	//// Filter by maximum salary
        //	//if (dto.MaxPrice < 20001)
        //	//{
        //	//    query = query.Where(Resume => Resume.SellPrice <= dto.MaxPrice).ToList();
        //	//}

        //	//// Filter by selected gender
        //	//if (dto.Gender != Gender.None)
        //	//{
        //	//	query = query.Where(Resume => Resume.Gender == dto.Gender).ToList();
        //	//}
        //	return PartialView("_ProductPartial", query);

        //}











        [HttpPost]
        public async Task<IActionResult> SortProducts(int id, [FromBody] ProductFilterVM dto)
        {
            var data = await FilterProducts(dto);
            switch (id)
            {
                case 1:
                    data = data.OrderBy(Resume => Resume.Name).ToList();
                    break;
                case 2:
                    data = data.OrderByDescending(Resume => Resume.Name).ToList();
                    break;
                case 3:
                    data = data.OrderByDescending(Resume => Resume.SellPrice).ToList();
                    break;
                case 4:
                    data = data.OrderBy(Resume => Resume.SellPrice).ToList();
                    break;
                default:
                    break;
            }
            return PartialView("_ProductPartial", data);
        }


        [HttpPost]
        public async Task<IActionResult> FilterViewProducts([FromBody] ProductFilterVM dto)
        {
            var filteredProducts = await FilterProducts(dto);
            return PartialView("_ProductPartial", filteredProducts);
        }

        private async Task<List<Product>> FilterProducts(ProductFilterVM dto)
        {
            var query = _context.Products
                .Include(x => x.ProductImages)
                .Include(x => x.Brand)
                .Include(x => x.BandType)
                .Include(x => x.Vendor)
                .Include(x => x.InnerColor)
                .Include(x => x.Visualization)
                .Where(x => !x.SoftDelete)
                .AsQueryable();

            if (dto.BrandsIds != null && dto.BrandsIds.Count > 0)
            {
                query = query.Where(p => dto.BrandsIds.Contains(p.Brand.Id));
            }

            if (dto.VendorsIds != null && dto.VendorsIds.Count > 0)
            {
                query = query.Where(p => dto.VendorsIds.Contains(p.Vendor.Id));
            }

            if (dto.BandTypesIds != null && dto.BandTypesIds.Count > 0)
            {
                query = query.Where(p => dto.BandTypesIds.Contains(p.BandType.Id));
            }

            if (dto.VisualizationsIds != null && dto.VisualizationsIds.Count > 0)
            {
                query = query.Where(p => dto.VisualizationsIds.Contains(p.Visualization.Id));
            }

            if (dto.InnerColorsIds != null && dto.InnerColorsIds.Count > 0)
            {
                query = query.Where(p => dto.InnerColorsIds.Contains(p.InnerColor.Id));
            }

            return await query.ToListAsync();
        }

    }
}


#region sortbook
//public async Task<IActionResult> SortBooks(int id)
//{

//    var data = await _context.Products.Where(x => !x.SoftDelete).Include(x => x.Brand)
//       .Include(x => x.ProductImages)
//       .Include(x => x.ProductSize)
//       .ThenInclude(bt => bt.Size)
//        .Include(x => x.Vendor).Take(8).ToListAsync();
//    switch (id)
//    {
//        case 1:
//            data = data.OrderBy(x => x.Name).ToList();
//            break;

//        case 2:
//            data = data.OrderByDescending(x => x.Name).ToList();
//            break;
//        case 3:
//            data = data.OrderBy(x => x.SellPrice).ToList();
//            break;
//        case 4:
//            data = data.OrderByDescending(x => x.SellPrice).ToList();
//            break;
//        case 5:
//            data = data.OrderBy(x => x.Rating).ToList();
//            break;
//        case 6:
//            data = data.OrderByDescending(x => x.Rating).ToList();
//            break;
//        default:
//            break;
//    }
//    return PartialView("_ProductPartial", data);
//}
#endregion











