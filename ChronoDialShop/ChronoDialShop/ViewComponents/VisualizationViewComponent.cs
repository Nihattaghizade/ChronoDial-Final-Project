using ChronoDialShop.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.ViewComponents;

public class VisualizationViewComponent : ViewComponent
{
	private readonly AppDbContext _context;

	public VisualizationViewComponent(AppDbContext context)
	{
		_context = context;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		var visualizations = await _context.Visualizations.Where(x => !x.SoftDelete)/*.Include(x => x.Products)*/.ToListAsync();
		return View(visualizations);
	}
}
