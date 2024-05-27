using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;


public class BandTypeViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public BandTypeViewComponent(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IViewComponentResult> InvokeAsync()
    {
        var bandTypes = await _context.BandTypes
            .Where(x => !x.SoftDelete)
            .OrderByDescending(x => x.Id).ToListAsync();

        return View(bandTypes);
    }
}
