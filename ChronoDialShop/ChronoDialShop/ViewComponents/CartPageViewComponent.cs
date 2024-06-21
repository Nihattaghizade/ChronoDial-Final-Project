using ChronoDialShop.Data;
using ChronoDialShop.Models;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;

namespace ChronoDialShop.ViewComponents;

public class CartPageViewComponent : ViewComponent
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;

    public CartPageViewComponent(AppDbContext context, UserManager<AppUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    //public async Task<IViewComponentResult> InvokeAsync()
    //{
    //    List<BasketVm>? basketVm = GetBasketFromCookies();
    //    List<BasketItemVm> basketItemsVm = new List<BasketItemVm>();
    //    foreach (var basketData in basketVm)
    //    {
    //        var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == basketData.Id);
    //        basketItemsVm.Add(new BasketItemVm()
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
    //private List<BasketVm> GetBasketFromCookies()
    //{
    //    List<BasketVm> basketVms;
    //    if (Request.Cookies["basket"] != null)
    //    {
    //        basketVms = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies["basket"]);
    //    }
    //    else basketVms = new List<BasketVm>();
    //    return basketVms;
    //}

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<BasketItemVm> basketItemsVm = new List<BasketItemVm>();

        if (User.Identity.IsAuthenticated)
        {
            var user = await _userManager.GetUserAsync((ClaimsPrincipal)User);
            var basketItems = await _context.BasketItems
                .Where(b => b.UserId == user.Id)
                .Include(b => b.Product)
                .ThenInclude(p => p.ProductImages)
                .ToListAsync();

            basketItemsVm = basketItems.Select(b => new BasketItemVm
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
            List<BasketVm> basketVm = GetBasketFromCookies();
            foreach (var basketData in basketVm)
            {
                var product = await _context.Products
                    .Include(x => x.ProductImages)
                    .FirstOrDefaultAsync(x => x.Id == basketData.Id);

                if (product != null)
                {
                    basketItemsVm.Add(new BasketItemVm
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

    private List<BasketVm> GetBasketFromCookies()
    {
        List<BasketVm> basketVms;
        if (Request.Cookies["basket"] != null)
        {
            basketVms = JsonConvert.DeserializeObject<List<BasketVm>>(Request.Cookies["basket"]);
        }
        else
        {
            basketVms = new List<BasketVm>();
        }
        return basketVms;
    }
}
