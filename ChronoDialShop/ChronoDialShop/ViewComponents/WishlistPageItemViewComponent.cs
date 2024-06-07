using ChronoDialShop.Data;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace ChronoDialShop.ViewComponents;

public class WishlistPageItemViewComponent : ViewComponent
{
    private readonly AppDbContext _context;

    public WishlistPageItemViewComponent(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IViewComponentResult> InvokeAsync()
    {
        List<WishlistVm>? basketVm = GetWishlist();
        List<WishlisItemVm> basketItemsVm = new List<WishlisItemVm>();
        foreach (var basketData in basketVm)
        {
            var product = await _context.Products.Include(x => x.ProductImages).FirstOrDefaultAsync(x => x.Id == basketData.Id);
            basketItemsVm.Add(new WishlisItemVm()
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
    private List<WishlistVm> GetWishlist()
    {
        List<WishlistVm> basketVms;
        if (Request.Cookies["basket"] != null)
        {
            basketVms = JsonConvert.DeserializeObject<List<WishlistVm>>(Request.Cookies["basket"]);
        }
        else basketVms = new List<WishlistVm>();
        return basketVms;
    }
}
