using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class BrandViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public BrandViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var brands = await _context.Brands.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
        return View(brands);
    }
}