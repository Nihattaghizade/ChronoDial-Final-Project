using System.ComponentModel.DataAnnotations.Schema;

namespace ChronoDialShop.Models;

public class SliderImage
{
    public int Id { get; set; }
    public bool SoftDelete { get; set; }
    public string Url { get; set; } = null!;
    [NotMapped]
    public IFormFile File { get; set; } = null!;
    public bool IsMain { get; set; }
    public int HomeSliderId { get; set; }
    public HomeSlider HomeSlider { get; set; } = null!;
}
