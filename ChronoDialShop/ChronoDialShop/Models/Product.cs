using ChronoDialShop.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChronoDialShop.Models
{
	public class Product
	{
		public int Id { get; set; }
		public bool SoftDelete { get; set; }
		public string Name { get; set; } = null!;
        public string ItemCode { get; set; } = null!;
		public string Description { get; set; } = null!;
        public double? Rating { get; set; } = null!;
		public decimal SellPrice { get; set; } = default!;
		public decimal? DiscountPrice { get; set; }
        [NotMapped]
        public List<IFormFile>? Files { get; set; }
        [NotMapped]
        public IFormFile MainFile { get; set; }
        [NotMapped]
        public IFormFile HoverFile { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
		public int VendorId { get; set; }
		public Vendor Vendor { get; set; } = null!;
        public int BrandId { get; set; }
        public Brand Brand { get; set; } = null!;
        public int VisualizationId { get; set; }
        public Visualization Visualization { get; set; }
        public int BandTypeId { get; set; }
        public BandType BandType { get; set; }
        public int InnerColorId { get; set; }
        public InnerColor InnerColor { get; set; }
        //public Gender Gender { get; set; }
        public ICollection<ProductSize>? ProductSize { get; set; }
        public Product()
        {
            ProductSize = new HashSet<ProductSize>();
        }
    }
}