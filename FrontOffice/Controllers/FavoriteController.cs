using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class FavoriteController : BaseController
{
    public FavoriteController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    public IActionResult Index()
    {
        return View();
    }
}
