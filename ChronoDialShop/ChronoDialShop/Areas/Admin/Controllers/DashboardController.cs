using ChronoDialShop.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChronoDialShop.Areas.Admin.Controllers;
[Area("Admin")]
[Authorize(Roles = "Admin,Vendor")]
public class DashboardController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
