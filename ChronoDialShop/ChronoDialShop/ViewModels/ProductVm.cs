using ChronoDialShop.Models;

namespace ChronoDialShop.ViewModels
{
	public class ProductVm
	{
        public Product Product { get; set; }
        public List<Product> Products { get; set; }
        public List<Brand> Brands { get; set; }
        public List<InnerColor> InnerColors { get; set; }
    }
}
