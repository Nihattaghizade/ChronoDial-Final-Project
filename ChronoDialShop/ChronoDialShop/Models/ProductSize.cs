using ChronoDialShop.Areas.Admin.ViewModels;

namespace ChronoDialShop.Models;

public class ProductSize
{
    public int Id { get; set; }
    public bool SoftDelete { get; set; }
    public Product Product { get; set; } = null!;
    public int ProductId { get; set; }
    public Size Size { get; set; } = null!;
    public int SizeId { get; set; }
    public int Count { get; set; }

    public static explicit operator ProductSize(ProductSizeVm productSizeVm)
    {
        return new ProductSize
        {
            ProductId = productSizeVm.ProductId,
            Count = productSizeVm.Count,
            SizeId = productSizeVm.SizeId
        };
    }
}
