using ChronoDialShop.Areas.Admin.ViewModels;
using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]
public class ProductSizeController : Controller
{
    private readonly AppDbContext _context;

    public ProductSizeController(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var prSizes = await _context.ProductSizes.Include(x => x.Product).Include(x => x.Size).Where(x => !x.Product.SoftDelete).ToListAsync();
        return View(prSizes);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Sizes = await _context.Sizes.ToListAsync();
        ViewBag.Products = await _context.Products.Where(x => !x.SoftDelete).ToListAsync();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(ProductSizeVm productSizeVm)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Products = await _context.Products.Where(x => !x.SoftDelete).ToListAsync();
            return View(productSizeVm);
        }

        var exist = await _context.ProductSizes
                                  .AnyAsync(x => x.SizeId == productSizeVm.SizeId && x.ProductId == productSizeVm.ProductId);

        if (exist)
        {
            ModelState.AddModelError("", "This product with the selected size already exists.");
            ViewBag.Sizes = await _context.Sizes.ToListAsync();
            ViewBag.Products = await _context.Products.Where(x => !x.SoftDelete).ToListAsync();
            return View(productSizeVm);
        }

        var productSize = new ProductSize
        {
            ProductId = productSizeVm.ProductId,
            SizeId = productSizeVm.SizeId,
            Count = productSizeVm.Count
        };

        _context.ProductSizes.Add(productSize);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }
}

