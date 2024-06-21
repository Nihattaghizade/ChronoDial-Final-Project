using ChronoDialShop.Data;
using ChronoDialShop.Models;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ChronoDialShop.ViewComponents;

public class WishlistPageItemViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    public WishlistPageItemViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    //public async Task<IViewComponentResult> InvokeAsync()
    //{
    //    List<WishlistVm>? basketVm = GetWishlistFromCookies();
    //    List<WishlisItemVm> basketItemsVm = new List<WishlisItemVm>();
    //    foreach (var basketData in basketVm)
    //    {
    //        var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == basketData.Id);
    //        basketItemsVm.Add(new WishlisItemVm()
    //        {
    //            Count = basketData.Count,
    //            Id = basketData.Id,
    //            Image = product.ProductImages.FirstOrDefault(x => x.IsMain).Url,
    //            Price = product.SellPrice,
    //            Name = product.Name,
    //        });
    //    }

    //    return View(basketItemsVm);
    //}
    //private List<WishlistVm> GetWishlistFromCookies()
    //{
    //    List<WishlistVm> basketVms;
    //    if (Request.Cookies["wishlist"] != null)
    //    {
    //        basketVms = JsonConvert.DeserializeObject<List<WishlistVm>>(Request.Cookies["wishlist"]);
    //    }
    //    else basketVms = new List<WishlistVm>();
    //    return basketVms;
    //}




    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<WishlisItemVm> basketItemsVm = new List<WishlisItemVm>();

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            var basketItems = await _context.WishlistItems
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Product)
                .ThenInclude(p => p.ProductImages)
                .ToListAsync();

            basketItemsVm = basketItems.Select(b => new WishlisItemVm
            {
                Count = b.Count,
                Id = b.ProductId,
                Image = b.Product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url,
                Price = b.Product.SellPrice,
                Name = b.Product.Name
            }).ToList();
        }
        else
        {
            List<WishlistVm> basketVm = GetWishlistFromCookies();
            foreach (var basketData in basketVm)
            {
                var product = await _context.Products
                    .Include(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.Id == basketData.Id);

                if (product != null)
                {
                    basketItemsVm.Add(new WishlisItemVm
                    {
                        Count = basketData.Count,
                        Id = basketData.Id,
                        Image = product.ProductImages.FirstOrDefault(x => x.IsMain)?.Url,
                        Price = product.SellPrice,
                        Name = product.Name
                    });
                }
            }
        }

        return View(basketItemsVm);
    }

    private List<WishlistVm> GetWishlistFromCookies()
    {
        List<WishlistVm> basketVms;
        if (Request.Cookies["wishlist"] != null)
        {
            basketVms = JsonConvert.DeserializeObject<List<WishlistVm>>(Request.Cookies["wishlist"]);
        }
        else
        {
            basketVms = new List<WishlistVm>();
        }
        return basketVms;
    }
}
