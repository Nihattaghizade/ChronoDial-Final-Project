using ChronoDialShop.Data;
using ChronoDialShop.Extentions;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin")]

public class HomeSliderController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    public HomeSliderController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task<IActionResult> Index()
    {
        var homeSliders = await _context.HomeSliders.Where(x => !x.SoftDelete).ToListAsync();
        return View(homeSliders);
    }


    public async Task<IActionResult> Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(HomeSlider homeSlider)
    {

        if (_context.HomeSliders.Any(p => p.Name == homeSlider.Name))
        {
            ModelState.AddModelError("", "Book already exists");
            return View(homeSlider);
        }
        homeSlider.SliderImages = new List<SliderImage>();
        if (homeSlider.Files != null)
        {
            foreach (var file in homeSlider.Files)
            {

                if (!file.CheckFileSize(2))
                {
                    ModelState.AddModelError("Files", "Files cannot be more than 2mb");
                    return View(homeSlider);
                }


                if (!file.CheckFileType("image"))
                {
                    ModelState.AddModelError("Files", "Files must be image type!");
                    return View(homeSlider);
                }

                var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "image/homeslider");
                var additionalBookImages = CreateHomeSlider(filename, false, homeSlider);

                homeSlider.SliderImages.Add(additionalBookImages);
            }
        }
        if (!homeSlider.MainFile.CheckFileSize(2))
        {
            ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
            return View(homeSlider);
        }


        if (!homeSlider.MainFile.CheckFileType("image"))
        {
            ModelState.AddModelError("MainFile", "Files must be image type!");
            return View(homeSlider);
        }

        var mainFileName = await homeSlider.MainFile.SaveFileAsync(_env.WebRootPath, "client", "image/homeslider");
        var mainBookImageCreate = CreateHomeSlider(mainFileName, true, homeSlider);

        homeSlider.SliderImages.Add(mainBookImageCreate);


        await _context.HomeSliders.AddAsync(homeSlider);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    public SliderImage CreateHomeSlider(string url, bool isMain, HomeSlider homeSlider)
    {
        return new SliderImage
        {
            Url = url,
            IsMain = isMain,
            HomeSlider = homeSlider
        };
    }



    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id < 1) return View("404");
        var product = await _context.HomeSliders.Include(x => x.SliderImages)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return View("404");


        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int id, HomeSlider product)
    {
        if (id != product.Id || id == null || id < 1) return BadRequest();

        var existProduct = await _context.HomeSliders.Include(p => p.SliderImages).FirstOrDefaultAsync(p => p.Id == id);
        if (existProduct is null)
        {
            return NotFound();
        }

        if (product.Files != null)
        {
            foreach (var file in product.Files)
            {

                if (!file.CheckFileSize(2))
                {
                    ModelState.AddModelError("Files", "Files cannot be more than 2mb");
                    return View(product);
                }


                if (!file.CheckFileType("image"))
                {
                    ModelState.AddModelError("Files", "Files must be image type!");
                    return View(product);
                }
                var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "image/homeslider");
                var additionalProductImages = CreateHomeSlider(filename,  false, product);
                existProduct.SliderImages.Add(additionalProductImages);
            }
        }
        if (product.MainFile != null)
        {
            if (!product.MainFile.CheckFileSize(2))
            {
                ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
                return View(product);
            }


            if (!product.MainFile.CheckFileType("image"))
            {
                ModelState.AddModelError("MainFile", "Files must be image type!");
                return View(product);
            }

            var iSMainUrl = existProduct.SliderImages.FirstOrDefault(x => x.IsMain).Url;
            product.MainFile.DeleteFile(_env.WebRootPath, "client", "image/homeslider", iSMainUrl);
            var bookImages = await _context.SliderImages.Where(p => p.IsMain == true).FirstOrDefaultAsync(p => p.HomeSliderId == id);
            _context.SliderImages.Remove(bookImages);
            var mainFileName = await product.MainFile.SaveFileAsync(_env.WebRootPath, "client", "image/homeslider");
            var mainProductImage = CreateHomeSlider(mainFileName, true, product);
            existProduct.SliderImages.Add(mainProductImage);

        }

        existProduct.Name = product.Name;
        existProduct.Description = product.Description;


        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    public async Task<IActionResult> Delete(int id)
    {
        HomeSlider? book = await _context.HomeSliders.FirstOrDefaultAsync(x => x.Id == id);
        if (book is null)
        {
            return NotFound();
        }
        //category.File.DeleteFile(_env.WebRootPath, "client", "assets", "categoryIcons", existsCategory.Icon);

        book.SoftDelete = true; // soft delete
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
}
