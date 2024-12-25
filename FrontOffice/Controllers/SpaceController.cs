using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FrontOffice.Models;

namespace FrontOffice.Controllers;

public class SpaceController : Controller
{
    private readonly ILogger<SpaceController> _logger;

    public SpaceController(ILogger<SpaceController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
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
