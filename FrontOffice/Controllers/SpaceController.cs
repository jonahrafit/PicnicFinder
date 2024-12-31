using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using FrontOffice.Models;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class SpaceController : Controller
{
    private readonly ILogger<SpaceController> _logger;
    private readonly string _apiBaseUrl;

    private readonly IConfiguration _configuration;


    public SpaceController(ILogger<SpaceController> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
        _apiBaseUrl = _configuration["ServerSettings:ApiBaseUrl"] ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<IActionResult> Index()
    {
        var url = $"{_apiBaseUrl}/Space"; // URL complète
        string jsonData = "[]";
        try
        {
            var response = await new HttpClient().GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                jsonData = await response.Content.ReadAsStringAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calling the API.");
        }

        ViewData["JsonSpaceList"] = jsonData;
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
