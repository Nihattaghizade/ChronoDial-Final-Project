using ChronoDialShop.Models;

namespace ChronoDialShop.ViewModels
{
    public class PaginateVm
    {
        public int TotalPageCount { get; set; }
        public int CurrentPage { get; set; }
        public List<Product> Products { get; set;}
    }
}
