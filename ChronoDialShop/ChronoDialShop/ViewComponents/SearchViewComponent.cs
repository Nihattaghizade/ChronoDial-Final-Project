using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace ChronoDialShop.ViewComponents;

public class SearchViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public SearchViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        return View();
    }
}
