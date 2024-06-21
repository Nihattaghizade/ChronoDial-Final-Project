using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChronoDialShop.Models;

public class AppUser : IdentityUser
{
    public string Name { get; set; } = null!;
    public string Surname { get; set; } = null!;
    [NotMapped]
    public string FullName { get => $"{Name} {Surname}"; }




    public ICollection<BasketItem> BasketItems { get; set; } = new List<BasketItem>();
    public ICollection<WishlistItem> WishlistItems { get; set; } = new List<WishlistItem>();
}
