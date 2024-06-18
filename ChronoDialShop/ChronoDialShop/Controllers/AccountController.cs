using ChronoDialShop.Data;
using ChronoDialShop.Enums;
using ChronoDialShop.Models;
using ChronoDialShop.Services;
using ChronoDialShop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mail;

namespace ChronoDialShop.Controllers;

public class AccountController : Controller
{
    private readonly AppDbContext _context;
    private readonly UserManager<AppUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly SignInManager<AppUser> _signInManager;
    private readonly IEmailService _emailService;

    public AccountController(UserManager<AppUser> userManager, 
                             RoleManager<IdentityRole> roleManager,
                             SignInManager<AppUser> signInManager,
                             AppDbContext context,
                             IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _signInManager = signInManager;
        _context = context;
        _emailService = emailService;
    }









    [HttpPost]
    public async Task<IActionResult> Search(string searchTerm)
    {
        if (string.IsNullOrEmpty(searchTerm) || searchTerm.Length < 3)
        {
            return BadRequest("Search term must be at least 3 characters long.");
        }

        var searchResults = await _context.Products
            .Where(p => p.Name.Contains(searchTerm) && !p.SoftDelete)
            .Select(p => new
            {
                p.Id,
                p.Name,
                p.SellPrice,
                ImageUrl = "/client/image/product/" + p.ProductImages.FirstOrDefault(x => x.IsMain).Url
            })
            .ToListAsync();

        return Json(searchResults);
    }










    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginVm loginVm)
    {
        if (!ModelState.IsValid) return View(loginVm);

        var existUser = await _userManager.FindByEmailAsync(loginVm.Email);
        if (existUser == null)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        var result = await _signInManager.PasswordSignInAsync(existUser, loginVm.Password, loginVm.RememberMe, true);
        if (!result.Succeeded)
        {
            ModelState.AddModelError("", "Invalid Credentials");
            return View();
        }

        // Clear the basket cookies upon successful login
        if (Request.Cookies["basket"] != null)
        {
            Response.Cookies.Delete("basket");
        }

        var role = await _userManager.IsInRoleAsync(existUser, Roles.Admin.ToString());
        if (role)
            return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });
        var vendorRole = await _userManager.IsInRoleAsync(existUser, Roles.Vendor.ToString());
        if (vendorRole)
            return RedirectToAction("Index", "Product", new { Area = "Admin" });
        return RedirectToAction("Index", "Home");
    }

    public async Task<IActionResult> LogOut()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    private async Task MigrateBasketItemsFromCookiesToDatabase(AppUser user)
    {
        var basketCookie = Request.Cookies["basket"];
        if (basketCookie != null)
        {
            var basketItems = JsonConvert.DeserializeObject<List<BasketVm>>(basketCookie);
            foreach (var item in basketItems)
            {
                var basketItem = await _context.BasketItems
                    .FirstOrDefaultAsync(x => x.ProductId == item.Id && x.UserId == user.Id);

                if (basketItem != null)
                {
                    basketItem.Count += item.Count;
                }
                else
                {
                    _context.BasketItems.Add(new BasketItem
                    {
                        UserId = user.Id,
                        ProductId = item.Id,
                        Count = item.Count
                    });
                }
            }
            await _context.SaveChangesAsync();
            Response.Cookies.Delete("basket");
        }
    }
































    public IActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Register(RegisterVm registerVm)
    {
        if (!ModelState.IsValid) return View(registerVm);

        var existUser = await _userManager.FindByNameAsync(registerVm.Username);
        if (existUser != null)
        {
            ModelState.AddModelError("", "User already exist");
            return View(registerVm);
        }

        AppUser newUser = new AppUser
        {
            Name = registerVm.Name,
            Surname = registerVm.Surname,
            Email = registerVm.Email,
            UserName = registerVm.Username,
        };
        var result = await _userManager.CreateAsync(newUser, registerVm.Password);
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", $"{error.Code} - {error.Description}");
            }
            return View(registerVm);
        }


        if (registerVm.IsVendor)
        {
            var resultVendor = await _userManager.AddToRoleAsync(newUser, Roles.Vendor.ToString());
            if (!resultVendor.Succeeded)
            {
                foreach (var error in resultVendor.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
                return View(registerVm);
            }

            Vendor newVendor = new Vendor
            {
                Name = $"{registerVm.Name} {registerVm.Surname}",
                User = newUser
            };
            _context.Vendors.Add(newVendor);
            await _context.SaveChangesAsync();
        }
        else
        {
            var resultCustomer = await _userManager.AddToRoleAsync(newUser, Roles.Customer.ToString());
            if (!resultCustomer.Succeeded)
            {
                foreach (var error in resultCustomer.Errors)
                {
                    ModelState.AddModelError("", $"{error.Code} - {error.Description}");
                }
                return View(registerVm);
            }
        }

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
        var link = Url.Action("VerifyEmail", "Account", new { email = newUser.Email, token = token });
        _emailService.SendEmail(new EmailDto(body: link, subject: "Email Verification", to: registerVm.Email));
        TempData["VerifyEmail"] = "Confirmation mail sent!";

        return RedirectToAction("Login", "Account");
    }


    



    public async Task<IActionResult> VerifyEmail(string? email, string? token)
    {
        if (token == null || email == null) return NotFound();
        var user = await _context.Users.FirstOrDefaultAsync(e => e.Email == email);

        var verify = await _userManager.ConfirmEmailAsync(user,token);

        if (verify.Succeeded)
        {
            user.EmailConfirmed = true;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }
        return RedirectToAction("Index", "Home");
    }


    #region(CreateRoles
    public async Task<IActionResult> CreateRole()
    {
        foreach (var role in Enum.GetValues(typeof(Roles)))
        {
            await _roleManager.CreateAsync(new IdentityRole
            {
                Id = Guid.NewGuid().ToString(),
                Name = role.ToString(),
            });
        }
        return RedirectToAction("Index", "Home");
    }
    #endregion
}









#region(AdminCreate
//var roleResult = await _userManager.AddToRoleAsync(newUser, Roles.Admin.ToString());
//if (!roleResult.Succeeded)
//{
//    foreach (var error in roleResult.Errors)
//    {
//        ModelState.AddModelError("", $"{error.Code} - {error.Description}");
//    }
//    return View(registerVm);
//}
#endregion

//[HttpPost]
//public async Task<IActionResult> Register(RegisterVm registerVm)
//{
//    if (!ModelState.IsValid) return View(registerVm);

//    var existUser = await _userManager.FindByNameAsync(registerVm.Username);
//    if (existUser != null)
//    {
//        ModelState.AddModelError("", "User already exists");
//        return View(registerVm);
//    }

//    AppUser newUser = new AppUser
//    {
//        Name = registerVm.Name,
//        Surname = registerVm.Surname,
//        Email = registerVm.Email,
//        UserName = registerVm.Username,
//    };

//    var result = await _userManager.CreateAsync(newUser, registerVm.Password);
//    if (!result.Succeeded)
//    {
//        foreach (var error in result.Errors)
//        {
//            ModelState.AddModelError("", $"{error.Code} - {error.Description}");
//        }
//        return View(registerVm);
//    }

//    var roleName = registerVm.IsVendor ? Roles.Vendor.ToString() : Roles.Customer.ToString();
//    var roleResult = await _userManager.AddToRoleAsync(newUser, roleName);
//    if (!roleResult.Succeeded)
//    {
//        foreach (var error in roleResult.Errors)
//        {
//            ModelState.AddModelError("", $"{error.Code} - {error.Description}");
//        }
//        return View(registerVm);
//    }

//    if (registerVm.IsVendor)
//    {
//        Vendor newVendor = new Vendor
//        {
//            Name = $"{registerVm.Name} {registerVm.Surname}",
//            User = newUser
//        };
//        _context.Vendors.Add(newVendor);
//        await _context.SaveChangesAsync();
//    }

//    var token = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);
//    var link = Url.Action("VerifyEmail", "Account", new { email = newUser.Email, token = token }, protocol: HttpContext.Request.Scheme);
//    _emailService.SendEmail(new EmailDto(body: link, subject: "Email Verification", to: registerVm.Email));
//    TempData["VerifyEmail"] = "Confirmation mail sent!";

//    return RedirectToAction("Login", "Account");
//}


#region InitialCodes
//public async Task<IActionResult> LogOut()
//{
//    await _signInManager.SignOutAsync();
//    return RedirectToAction("Index", "Home");
//}



//public IActionResult Login()
//{
//    return View();
//}
//[HttpPost]
//public async Task<IActionResult> Login(LoginVm loginVm)
//{
//    if (!ModelState.IsValid) return View(loginVm);

//    var existUser = await _userManager.FindByEmailAsync(loginVm.Email);
//    if (existUser == null)
//    {
//        ModelState.AddModelError("", "Invalid Credentials");
//        return View();
//    }

//    var result = await _signInManager.PasswordSignInAsync(existUser, loginVm.Password, loginVm.RememberMe, true);
//    if (!result.Succeeded)
//    {
//        ModelState.AddModelError("", "Invalid Credentials");
//        return View();
//    }
//    var role = await _userManager.IsInRoleAsync(existUser, Roles.Admin.ToString());
//    if (role)
//        return RedirectToAction("Index", "Dashboard", new { Area = "Admin" });
//    var vendorRole = await _userManager.IsInRoleAsync(existUser, Roles.Vendor.ToString());
//    if (vendorRole)
//        return RedirectToAction("Index", "Product", new { Area = "Admin" });
//    return RedirectToAction("Index", "Home");
//}



#endregion
