using ChronoDialShop.Data;
using ChronoDialShop.Extentions;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]

public class VisualizationController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    public VisualizationController(AppDbContext context, IWebHostEnvironment env)
    {
        _context = context;
        _env = env;
    }
    public async Task<IActionResult> Index()
    {
        var visualizations = await _context.Visualizations.Include(x => x.Products).Where(x => !x.SoftDelete).ToListAsync();
        return View(visualizations);
    }
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Visualization visualization)
    {
        if (ModelState["Name"] == null ||
            ModelState["File"] == null) return View(visualization);

        if (!visualization.File.CheckFileType("image"))
        {
            ModelState.AddModelError("", "Invalid File");
            return View(visualization);
        }
        if (!visualization.File.CheckFileSize(2))
        {
            ModelState.AddModelError("", "Invalid File Size");
            return View(visualization);
        }

        string uniqueFileName = await visualization.File.SaveFileAsync(_env.WebRootPath, "client", "image/numberdial");

        Visualization newVisualization = new Visualization
        {
            Name = visualization.Name,
            Icon = uniqueFileName,
        };

        await _context.Visualizations.AddAsync(newVisualization);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index");
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id == 0)
        {
            return View("404");
        }
        Visualization? visualization = await _context.Visualizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (visualization == null)
        {
            return View("404");
        }
        return View(visualization);
    }


    public async Task<IActionResult> Update(int id, Visualization visualization)
    {
        if (id != visualization.Id) return BadRequest();
        Visualization? existsVisualization = await _context.Visualizations.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id);
        if (existsVisualization == null) return NotFound();
        if (visualization.File != null)
        {
            if (!visualization.File.CheckFileSize(2))
            {
                ModelState.AddModelError("File", "File size more than 2mb");
                return View(visualization);
            }
            if (!visualization.File.CheckFileType("image"))
            {
                ModelState.AddModelError("File", "File type is incorrect");
                return View(visualization);
            }

            visualization.File.DeleteFile(_env.WebRootPath, "client", "image/numberdial", existsVisualization.Icon);

            var uniqueFileName = await visualization.File.
                SaveFileAsync(_env.WebRootPath, "client", "image/numberdial");

            existsVisualization.Icon = uniqueFileName;
            existsVisualization.Name = visualization.Name;
            _context.Update(existsVisualization);
        }
        else
        {
            visualization.Icon = existsVisualization.Icon;
            _context.Visualizations.Update(visualization);
        }
        await _context.SaveChangesAsync();
        if (visualization.Name == null)
        {
            return RedirectToAction("Edit", new { id = id });
        }
        return RedirectToAction("Index");
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        Visualization? visualization = await _context.Visualizations.FirstOrDefaultAsync(x => x.Id == id);
        if (visualization is null)
        {
            return NotFound();
        }

        visualization.SoftDelete = true; // soft delete
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}
