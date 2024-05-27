namespace ChronoDialShop.Models
{
    public class InnerColor
    {
        public int Id { get; set; }
        public bool SoftDelete { get; set; }
        public string Name { get; set; } = null!;
        public List<Product>? Products { get; set; }
    }
}
