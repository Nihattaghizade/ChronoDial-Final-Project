using ChronoDialShop.Data;
using ChronoDialShop.Extentions;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]

public class BandTypeController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    public BandTypeController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task<IActionResult> Index()
    {
        var bandTypes = await _context.BandTypes.Include(x => x.Products).Where(x => !x.SoftDelete).ToListAsync();
        return View(bandTypes);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(BandType bandType)
    {
        if (ModelState["Name"] == null ||
            ModelState["File"] == null) return View(bandType);

        if (!bandType.File.CheckFileType("image"))
        {
            ModelState.AddModelError("", "Invalid File");
            return View(bandType);
        }
        if (!bandType.File.CheckFileSize(2))
        {
            ModelState.AddModelError("", "Invalid File Size");
            return View(bandType);
        }

        string uniqueFileName = await bandType.File.SaveFileAsync(_env.WebRootPath, "client", "image/bandtype");

        BandType newBandType = new BandType
        {
            Name = bandType.Name,
            Icon = uniqueFileName,
        };

        await _context.BandTypes.AddAsync(newBandType);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return View("404");
        }
        BandType? bandType = await _context.BandTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (bandType == null)
        {
            return View("404");
        }
        return View(bandType);
    }


    public async Task<IActionResult> Update(int id, BandType bandType)
    {
        if (id != bandType.Id) return BadRequest();
        BandType? existsBandType = await _context.BandTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existsBandType == null) return NotFound();
        if (bandType.File != null)
        {
            if (!bandType.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "File size more than 2mb");
                return View(bandType);
            }
            if (!bandType.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "File type is incorrect");
                return View(bandType);
            }

            bandType.File.DeleteFile(_env.WebRootPath, "client", "image/bandtype", existsBandType.Icon);

            var uniqueFileName = await bandType.File.
                SaveFileAsync(_env.WebRootPath, "client", "image/bandtype");

            existsBandType.Icon = uniqueFileName;
            existsBandType.Name = bandType.Name;
            _context.Update(existsBandType);
        }
        else
        {
            bandType.Icon = existsBandType.Icon;
            _context.BandTypes.Update(bandType);
        }
        await _context.SaveChangesAsync();
        if (bandType.Name == null)
        {
            return RedirectToAction("Edit", new { id = id });
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        BandType? bandType = await _context.BandTypes.FirstOrDefaultAsync(x => x.Id == id);
        if (bandType is null)
        {
            return NotFound();
        }

        bandType.SoftDelete = true; // soft delete
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
