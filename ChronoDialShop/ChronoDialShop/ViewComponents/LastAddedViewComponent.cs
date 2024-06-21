using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class LastAddedViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public LastAddedViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var products = await _context.Products.Include(x => x.Brand)
            .Include(x => x.ProductImages)
            .Where(x => !x.SoftDelete)
            .OrderByDescending(x => x.Id).Take(6).ToListAsync();

        return View(products);
    }
}
