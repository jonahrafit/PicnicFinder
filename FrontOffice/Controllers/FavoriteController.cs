using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class FavoriteController : Controller
{
    private readonly ILogger<FavoriteController> _logger;
    private readonly string _apiBaseUrl;

    private readonly IConfiguration _configuration;

    public FavoriteController(ILogger<FavoriteController> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
        _apiBaseUrl =
            _configuration["ServerSettings:ApiBaseUrl"]
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IActionResult Index()
    {
        return View();
    }
}
