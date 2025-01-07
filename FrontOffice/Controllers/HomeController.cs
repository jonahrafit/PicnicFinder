using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;

namespace FrontOffice.Controllers;

public class HomeController : BaseController
{
    public HomeController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

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
        return View(
            new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier }
        );
    }
}
