using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class BreitlingViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public BreitlingViewComponent(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var products = await _context.Products
            .Include(x => x.Brand)
            .Include(x => x.ProductImages)
            .Where(x => !x.SoftDelete && x.Brand.Name == "Breitling")
            .OrderByDescending(x => x.Id).Take(6).ToListAsync();

        return View(products);
    }
}
