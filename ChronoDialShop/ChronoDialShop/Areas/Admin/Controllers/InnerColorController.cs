using ChronoDialShop.Data;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]

public class InnerColorController : Controller
{
    private readonly AppDbContext _context;
    public InnerColorController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var innerColors = await _context.InnerColors.Include(x => x.Products).Where(x => !x.SoftDelete).ToListAsync();
        return View(innerColors);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(InnerColor innerColor)
    {
        if (ModelState["Name"] == null) return View(innerColor);

        InnerColor newInnerColor = new InnerColor
        {
            Name = innerColor.Name
        };

        await _context.InnerColors.AddAsync(newInnerColor);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        InnerColor? innerColor = await _context.InnerColors.FirstOrDefaultAsync(x => x.Id == id);
        if (innerColor is null)
        {
            return NotFound();
        }

        innerColor.SoftDelete = true;  //softdelete
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return View("404");
        }
        InnerColor? innerColor = await _context.InnerColors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (innerColor == null)
        {
            return View("404");
        }
        return View(innerColor);
    }
    public async Task<IActionResult> Update(int id, InnerColor innerColor)
    {
        if (id != innerColor.Id) return BadRequest();
        InnerColor? existsInnerColor = await _context.InnerColors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existsInnerColor == null) return NotFound();
        if (innerColor != null)
        {
            existsInnerColor.Name = innerColor.Name;
            _context.Update(existsInnerColor);
        }
        else
        {
            _context.InnerColors.Update(innerColor);
        }
        await _context.SaveChangesAsync();
        if (innerColor.Name == null)
        {
            return RedirectToAction("Edit", new { id = id });
        }

        return RedirectToAction("Index");
    }
}
