namespace ChronoDialShop.Models
{
	public class Vendor
	{
		public string Name { get; set; } = null!;
		public List<Product>? Products { get; set; }
		public int Id { get; set; }
		public bool SoftDelete { get; set; }
	}
}
