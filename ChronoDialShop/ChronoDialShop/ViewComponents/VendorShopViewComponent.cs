using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class VendorShopViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public VendorShopViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var vendors = await _context.Vendors.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
		return View(vendors);
	}
}