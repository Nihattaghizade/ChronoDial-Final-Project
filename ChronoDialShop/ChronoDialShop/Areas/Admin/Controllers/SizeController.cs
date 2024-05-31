using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class SizeController : Controller
{
    private readonly AppDbContext _context;

    public SizeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var sizes = await _context.Sizes.Where(x => !x.SoftDelete).ToListAsync();
        return View(sizes);
    }

    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Size size)
    {
        size.Name = size.Name.ToUpper().Trim();
        if (_context.Sizes.Any(x => x.Name.ToUpper().Trim() == size.Name))
        {
            ModelState.AddModelError("", "Size already exist");
            return View();
        }
        await _context.Sizes.AddAsync(size);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}
