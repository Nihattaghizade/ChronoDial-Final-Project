using ChronoDialShop.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChronoDialShop.Data;

	public class AppDbContext : IdentityDbContext<AppUser>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


		public DbSet<Product> Products { get; set; }
		public DbSet<ProductImage> ProductImages { get; set; }
		public DbSet<Vendor> Vendors { get; set; }
    public DbSet<Brand> Brands { get; set; }
    public DbSet<Size> Sizes { get; set; }
    public DbSet<ProductSize> ProductSizes { get; set; }
    public DbSet<Visualization> Visualizations { get; set; }
    public DbSet<BandType> BandTypes { get; set; }
    public DbSet<InnerColor> InnerColors { get; set; }
    public DbSet<BasketItem> BasketItems { get; set; }
     public DbSet<AppUser> Users { get; set; }



    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Brand>().HasQueryFilter(x => !x.SoftDelete);
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<BasketItem>()
                .HasOne(b => b.Product)
                .WithMany(p => p.BasketItems)
                .HasForeignKey(b => b.ProductId)
                .OnDelete(DeleteBehavior.Restrict); // or DeleteBehavior.NoAction

        modelBuilder.Entity<BasketItem>()
            .HasOne(b => b.User)
            .WithMany(u => u.BasketItems)
            .HasForeignKey(b => b.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
