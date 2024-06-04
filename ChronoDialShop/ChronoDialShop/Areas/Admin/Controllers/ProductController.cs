using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Extentions;
using ChronoDialShop.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Vendor")]

public class ProductController : Controller
{
    private readonly AppDbContext _context;
    private readonly IWebHostEnvironment _env;
    private readonly UserManager<AppUser> _userManager;

    public ProductController(AppDbContext context, IWebHostEnvironment env, UserManager<AppUser> userManager)
    {
        _context = context;
        _env = env;
        _userManager = userManager;
    }
    public async Task<IActionResult> Index()
    {
        List<Product> products = await _context.Products
                                                .Include(x => x.ProductImages)
                                                .Include(x => x.Brand)
                                                .Where(x => !x.SoftDelete)
                                                .ToListAsync();
        return View(products);
    }
    public async Task<IActionResult> Create()
    {
        ViewBag.Brands = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Sizes = await _context.Sizes.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Vendors = await _context.Vendors.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Visualizations = await _context.Visualizations.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.BandTypes = await _context.BandTypes.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.InnerColors = await _context.InnerColors.Where(x => !x.SoftDelete).ToListAsync();

        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Product product)
    {

        if (_context.Products.Any(p => p.Name == product.Name))
        {
            ModelState.AddModelError("", "Book already exists");
            return View(product);
        }

        product.ProductImages = new List<ProductImage>();
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

                var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "image/product");
                var additionalProductImages = CreateProduct(filename, false, false, product);

                product.ProductImages.Add(additionalProductImages);
            }
        }
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

        var mainFileName = await product.MainFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
        var mainProductImageCreate = CreateProduct(mainFileName, false, true, product);

        product.ProductImages.Add(mainProductImageCreate);

        if (!product.HoverFile.CheckFileSize(2))
        {
            ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
            return View(product);
        }


        if (!product.HoverFile.CheckFileType("image"))
        {
            ModelState.AddModelError("HoverFile", "Files must be image type!");
            return View(product);
        }

        var hoverFileName = await product.HoverFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
        var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, product);
        product.ProductImages.Add(hoverProductImageCreate);

        await _context.Products.AddAsync(product);

        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    public ProductImage CreateProduct(string url, bool isHover, bool isMain, Product product)
    {
        return new ProductImage
        {
            Url = url,
            IsHover = isHover,
            IsMain = isMain,
            Product = product
        };
    }
    public async Task<IActionResult> Detail(int? id)
    {
        if (id == null || id <= 0) return BadRequest();
        var product = await _context.Products.Include(x => x.Brand).Include(x => x.ProductImages)
                                             .Include(x => x.BandType)
                                             .Include(x => x.InnerColor)
                                             .Include(x => x.Visualization)
                                             .Include(x => x.Vendor)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return NotFound();
        return View(product);
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null || id < 1) return View("404");
        ViewBag.Brands = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Sizes = await _context.Sizes.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Vendors = await _context.Vendors.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.Visualizations = await _context.Visualizations.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.BandTypes = await _context.BandTypes.Where(x => !x.SoftDelete).ToListAsync();
        ViewBag.InnerColors = await _context.InnerColors.Where(x => !x.SoftDelete).ToListAsync();
        var product = await _context.Products.Include(x => x.ProductImages)
                                             .Include(x => x.Brand)
                                             .FirstOrDefaultAsync(x => x.Id == id);
        if (product == null) return View("404");


        return View(product);
    }
    [HttpPost]
    public async Task<IActionResult> Update(int id, Product product)
    {
        if (id != product.Id || id == null || id < 1) return BadRequest();

        var existProduct = await _context.Products.Include(p => p.ProductImages).FirstOrDefaultAsync(p => p.Id == id);
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
                var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "image/product");
                var additionalProductImages = CreateProduct(filename, false, false, product);
                existProduct.ProductImages.Add(additionalProductImages);


                //var isFileUrl = existProduct.BookImages.Where(x => !x.IsMain && !x.IsHover).Select(x => x.Url).ToList();
                //foreach (var fileUrl in isFileUrl)
                //{
                //    product.Files.DeleteFile(_env.WebRootPath, "client", "image/products", fileUrl);
                //}
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

            var iSMainUrl = existProduct.ProductImages.FirstOrDefault(x => x.IsMain).Url;
            product.MainFile.DeleteFile(_env.WebRootPath, "client", "image/products", iSMainUrl);
            var productImages = await _context.ProductImages.Where(p => p.IsMain == true).FirstOrDefaultAsync(p => p.ProductId == id);
            _context.ProductImages.Remove(productImages);
            var mainFileName = await product.MainFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
            var mainProductImage = CreateProduct(mainFileName, false, true, product);
            existProduct.ProductImages.Add(mainProductImage);

        }
        if (product.HoverFile != null)
        {
            if (!product.HoverFile.CheckFileSize(2))
            {
                ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
                return View(product);
            }

            if (!product.HoverFile.CheckFileType("image"))
            {
                ModelState.AddModelError("HoverFile", "Files must be image type!");
                return View(product);
            }
            var iSHoverUrl = existProduct.ProductImages.FirstOrDefault(x => x.IsHover).Url;
            product.MainFile.DeleteFile(_env.WebRootPath, "client", "image/product", iSHoverUrl);
            var productImages = await _context.ProductImages.Where(p => p.IsHover == true).FirstOrDefaultAsync(p => p.ProductId == id);
            _context.ProductImages.Remove(productImages);
            var hoverFileName = await product.HoverFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
            var hoverProductImage = CreateProduct(hoverFileName, true, false, product);
            existProduct.ProductImages.Add(hoverProductImage);
        }

        existProduct.Name = product.Name;
        existProduct.Description = product.Description;
        existProduct.SellPrice = product.SellPrice;
        existProduct.Rating = product.Rating;
        existProduct.DiscountPrice = product.DiscountPrice;
        existProduct.BrandId = product.BrandId;


        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }
    
    public async Task<IActionResult> Delete(int id)
    {
        Product? product = await _context.Products.FirstOrDefaultAsync(x => x.Id == id);
        if (product is null)
        {
            return NotFound();
        }
        //category.File.DeleteFile(_env.WebRootPath, "client", "assets", "categoryIcons", existsCategory.Icon);

        product.SoftDelete = true; // soft delete
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}


//public async Task<IActionResult> Create()
//{
//    var user = await _userManager.GetUserAsync(User);
//    var roles = await _userManager.GetRolesAsync(user);
//    ViewBag.UserRole = roles;

//    ViewBag.Brands = await _context.Brands.Where(x => !x.SoftDelete).ToListAsync();
//    ViewBag.Sizes = await _context.Sizes.Where(x => !x.SoftDelete).ToListAsync();
//    ViewBag.Vendors = await _context.Vendors.Where(x => !x.SoftDelete).ToListAsync();
//    ViewBag.Visualizations = await _context.Visualizations.Where(x => !x.SoftDelete).ToListAsync();
//    ViewBag.BandTypes = await _context.BandTypes.Where(x => !x.SoftDelete).ToListAsync();
//    ViewBag.InnerColors = await _context.InnerColors.Where(x => !x.SoftDelete).ToListAsync();

//    return View();
//}


//[HttpPost]
//public async Task<IActionResult> Create(Product product)
//{
//    if (_context.Products.Any(p => p.Name == product.Name))
//    {
//        ModelState.AddModelError("", "Product already exists");
//        return View(product);
//    }

//    var user = await _userManager.GetUserAsync(User);
//    var userRoles = await _userManager.GetRolesAsync(user);

//    if (userRoles.Contains(Roles.Vendor.ToString()))
//    {
//        // Assuming the Vendor ID is the same as the user's ID or some property linked to the user
//        var vendor = await _context.Vendors.FirstOrDefaultAsync(v => v.AppUserId == user.Id);
//        if (vendor != null)
//        {
//            product.VendorId = vendor.Id;
//        }
//        else
//        {
//            ModelState.AddModelError("", "Vendor information not found.");
//            return View(product);
//        }
//    }

//    // Process product images
//    product.ProductImages = new List<ProductImage>();

//    if (product.Files != null)
//    {
//        foreach (var file in product.Files)
//        {
//            if (!file.CheckFileSize(2))
//            {
//                ModelState.AddModelError("Files", "Files cannot be more than 2mb");
//                return View(product);
//            }

//            if (!file.CheckFileType("image"))
//            {
//                ModelState.AddModelError("Files", "Files must be image type!");
//                return View(product);
//            }

//            var filename = await file.SaveFileAsync(_env.WebRootPath, "client", "image/product");
//            var additionalProductImages = CreateProduct(filename, false, false, product);

//            product.ProductImages.Add(additionalProductImages);
//        }
//    }

//    if (!product.MainFile.CheckFileSize(2))
//    {
//        ModelState.AddModelError("MainFile", "Files cannot be more than 2mb");
//        return View(product);
//    }

//    if (!product.MainFile.CheckFileType("image"))
//    {
//        ModelState.AddModelError("MainFile", "Files must be image type!");
//        return View(product);
//    }

//    var mainFileName = await product.MainFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
//    var mainProductImageCreate = CreateProduct(mainFileName, false, true, product);
//    product.ProductImages.Add(mainProductImageCreate);

//    if (!product.HoverFile.CheckFileSize(2))
//    {
//        ModelState.AddModelError("HoverFile", "Files cannot be more than 2mb");
//        return View(product);
//    }

//    if (!product.HoverFile.CheckFileType("image"))
//    {
//        ModelState.AddModelError("HoverFile", "Files must be image type!");
//        return View(product);
//    }

//    var hoverFileName = await product.HoverFile.SaveFileAsync(_env.WebRootPath, "client", "image/product");
//    var hoverProductImageCreate = CreateProduct(hoverFileName, true, false, product);
//    product.ProductImages.Add(hoverProductImageCreate);

//    await _context.Products.AddAsync(product);
//    await _context.SaveChangesAsync();

//    return RedirectToAction("Index");
//}
//public async Task<IActionResult> DeleteImage(int id)
//{
//    var existsImage = await _context.BookImages.FindAsync(id);
//    var product = await _context.Books.Include(x => x.Category)
//                                          .Include(x => x.BookImages)
//                                          .FirstOrDefaultAsync(x => x.Id == existsImage.BookId);
//    existsImage.File.DeleteFile(_env.WebRootPath, "client", "image/products", existsImage.Url);
//    _context.Remove(existsImage);
//    await _context.SaveChangesAsync();
//    return PartialView("_ProductImagePartial", product.BookImages);
//}



//public async Task<IActionResult> DeleteImage(int id)
//{
//    var existsImage = await _context.BookImages.FindAsync(id);
//    var product = await _context.Books.Include(x => x.Category)
//                                          .Include(x => x.BookImages)
//                                          .FirstOrDefaultAsync(x => x.Id == existsImage.BookId);
//    existsImage.File.DeleteFile(_env.WebRootPath, "client", "image/products", existsImage.Url);
//    _context.Remove(existsImage);
//    await _context.SaveChangesAsync();
//    return PartialView("_ProductImagePartial", product.BookImages);
//}