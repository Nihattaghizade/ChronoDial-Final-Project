using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BrandController : Controller
{
    private readonly AppDbContext _context;
    public BrandController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var brands = await _context.Brands.Include(x => x.Products).Where(x => !x.SoftDelete).ToListAsync();
        return View(brands);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(Brand brand)
    {
        if (ModelState["Name"] == null) return View(brand);

        Brand newBrand = new Brand
        {
            Name = brand.Name
        };

        await _context.Brands.AddAsync(newBrand);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Delete(int id)
    {
        Brand? brand = await _context.Brands.FirstOrDefaultAsync(x => x.Id == id);
        if (brand is null)
        {
            return NotFound();
        }
        
        brand.SoftDelete = true;  //softdelete
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return View("404");
        }
        Brand? brand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (brand == null)
        {
            return View("404");
        }
        return View(brand);
    }
    public async Task<IActionResult> Update(int id, Brand brand)
    {
        if (id != brand.Id) return BadRequest();
        Brand? existsBrand = await _context.Brands.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existsBrand == null) return NotFound();
        if (brand != null)
        {
            existsBrand.Name = brand.Name;
            _context.Update(existsBrand);
        }
        else
        {
            _context.Brands.Update(brand);
        }
        await _context.SaveChangesAsync();
        if (brand.Name == null)
        {
            return RedirectToAction("Edit", new { id = id });
        }

        return RedirectToAction("Index");
    }
}
