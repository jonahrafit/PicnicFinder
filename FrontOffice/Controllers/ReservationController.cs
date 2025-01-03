using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class ReservationController : Controller
{
    private readonly ILogger<ReservationController> _logger;
    private readonly string _apiBaseUrl;

    private readonly IConfiguration _configuration;

    public ReservationController(
        ILogger<ReservationController> logger,
        IConfiguration configuration
    )
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
