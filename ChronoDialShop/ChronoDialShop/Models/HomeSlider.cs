using System.ComponentModel.DataAnnotations.Schema;

namespace ChronoDialShop.Models;

public class HomeSlider
{
    public int Id { get; set; }
    public bool SoftDelete { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;
    [NotMapped]
    public List<IFormFile>? Files { get; set; }
    [NotMapped]
    public IFormFile MainFile { get; set; }
    public List<SliderImage>? SliderImages { get; set; }
}
