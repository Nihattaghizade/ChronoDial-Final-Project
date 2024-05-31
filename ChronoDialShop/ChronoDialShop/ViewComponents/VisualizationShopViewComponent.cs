using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class VisualizationShopViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public VisualizationShopViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        var visualizzations = await _context.Visualizations.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
        return View(visualizzations);
    }
}
