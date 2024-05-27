using System.ComponentModel.DataAnnotations.Schema;

namespace ChronoDialShop.Models;

public class Visualization
{
    public int Id { get; set; }
    public bool SoftDelete { get; set; }
    public string Name { get; set; } = null!;
    public string Icon { get; set; } = null!;
    [NotMapped]
    public IFormFile File { get; set; }
    public List<Product>? Products { get; set; }
}
