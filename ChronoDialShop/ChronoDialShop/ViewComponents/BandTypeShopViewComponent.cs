using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class BandTypeShopViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public BandTypeShopViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var shopBandTypes = await _context.BandTypes.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
        return View(shopBandTypes);
    }
}
