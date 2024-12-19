using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PicnicFinder.Models;
using Microsoft.AspNetCore.Authorization;

namespace PicnicFinder.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize(Roles = "ADMIN,OWNER")]
    public IActionResult Index()
    {
        ViewData["ActiveMenu"] = "Dashboard";
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

}
