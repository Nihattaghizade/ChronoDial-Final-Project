using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class ProductMainViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public ProductMainViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
		//var products = _context.Products
		//    .Include(x => x.Brand)
		//    .Include(x => x.ProductImages);

		//var count = GetPageCount(pageSize);
		//PaginateVm paginateVm = new PaginateVm()
		//{
		//    CurrentPage = page,
		//    TotalPageCount = count,
		//};
		//if (brandId == null)
		//{
		//    var orderedWithCatProducts = await products.Skip((page - 1) * pageSize)
		//                                               .Take(pageSize)
		//                                               .OrderByDescending(x => x.Id)
		//                                               .ToListAsync();

		//    paginateVm.Products = orderedWithCatProducts;

		//    return View(paginateVm);
		//}
		//if (productName != null)
		//{
		//    paginateVm.Products = await products.Skip((page - 1) * pageSize)
		//                                        .Take(pageSize)
		//                                        .Where(x => x.BrandId == brandId && x.Name.StartsWith(productName))
		//                                        .OrderByDescending(x => x.Id)
		//                                        .ToListAsync();
		//    return View(paginateVm);
		//}
		//var orderedProducts = await products.Skip((page - 1) * pageSize)
		//                                        .Take(pageSize)
		//                                        .Where(x => x.BrandId == brandId)
		//                                        .OrderByDescending(x => x.Id)
		//                                        .ToListAsync();

		//paginateVm.Products=orderedProducts;
		//return View(paginateVm);


		//if (brandId == null)
		//{
		//    return View(await _context.Products
		//        .Take(20)
		//        .Include(x => x.Brand)
		//        .Include(x => x.ProductImages).ToListAsync());
		//}
		//var products = await _context.Products.Where(x => x.BrandId == brandId)
		//    .Take(20)
		//    .Include(x => x.Brand)
		//    .Include(x => x.ProductImages).ToListAsync();

		//return View(products);


		var products = await _context.Products
		   .Include(x => x.ProductImages)
								.Include(x => x.Brand)
								.Include(x => x.BandType)
								.Include(x => x.Vendor)
								.Include(x => x.InnerColor)
								.Include(x => x.Visualization)
		   .Where(x => !x.SoftDelete).ToListAsync();

		return View(products);
	}

    //public int GetPageCount(int pageSize)
    //{
    //    var productCount = _context.Products.Count();
    //    return (int)Math.Ceiling((decimal)productCount / pageSize);
    //}
}
