using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChronoDialShop.ViewComponents;

public class CartPageViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public CartPageViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<BasketVm>? basketVm = GetBasketFromCookies();
        List<BasketItemVm> basketItemsVm = new List<BasketItemVm>();
        foreach (var basketData in basketVm)
        {
            var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == basketData.Id);
            basketItemsVm.Add(new BasketItemVm()
            {
                Count = basketData.Count,
                Id = basketData.Id,
                Image = product.ProductImages.FirstOrDefault(x => x.IsMain).Url,
                Price = product.SellPrice,
                Name = product.Name,
            });
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
        else basketVms = new List<BasketVm>();
        return basketVms;
    }
}
