using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class VendorController : Controller
{
    private readonly AppDbContext _context;
    public VendorController(AppDbContext context)
    {
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        var vendors = await _context.Vendors/*.Include(x => x.Products)*/.Where(x => !x.SoftDelete).ToListAsync();
        return View(vendors);
    }

    //[HttpGet]
    //public IActionResult Create()
    //{
    //    return View();
    //}

    //[HttpPost]
    //public async Task<IActionResult> Create(Vendor vendor)
    //{
    //    if (ModelState["Name"] == null) return View(vendor);

    //    Vendor newVendor = new Vendor
    //    {
    //        Name = vendor.Name
    //    };

    //    await _context.Vendors.AddAsync(newVendor);
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction("Index");
    //}

    //public async Task<IActionResult> Delete(int id)
    //{
    //    Vendor? vendor = await _context.Vendors.FirstOrDefaultAsync(x => x.Id == id);
    //    if (vendor is null)
    //    {
    //        return NotFound();
    //    }

    //    vendor.SoftDelete = true;  //softdelete
    //    await _context.SaveChangesAsync();
    //    return RedirectToAction(nameof(Index));
    //}
    //public async Task<IActionResult> Edit(int? id)
    //{
    //    if (id == null || id == 0)
    //    {
    //        return View("404");
    //    }
    //    Vendor? vendor = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //    if (vendor == null)
    //    {
    //        return View("404");
    //    }
    //    return View(vendor);
    //}
    //public async Task<IActionResult> Update(int id, Vendor vendor)
    //{
    //    if (id != vendor.Id) return BadRequest();
    //    Vendor? existsVendor = await _context.Vendors.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
    //    if (existsVendor == null) return NotFound();
    //    if (vendor != null)
    //    {
    //        existsVendor.Name = vendor.Name;
    //        _context.Update(existsVendor);
    //    }
    //    else
    //    {
    //        _context.Vendors.Update(vendor);
    //    }
    //    await _context.SaveChangesAsync();
    //    if (vendor.Name == null)
    //    {
    //        return RedirectToAction("Edit", new { id = id });
    //    }

    //    return RedirectToAction("Index");
    //}
}
