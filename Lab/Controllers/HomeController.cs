using Microsoft.AspNetCore.Mvc;

namespace MyAppMVC.Controllers;

public class HomeController : Controller
{
    public IActionResult Error() => View();
}
