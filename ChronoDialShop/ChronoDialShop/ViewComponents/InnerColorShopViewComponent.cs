using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class InnerColorShopViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public InnerColorShopViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var innerColors = await _context.InnerColors.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
        return View(innerColors);
    }
}
