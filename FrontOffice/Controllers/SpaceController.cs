using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class SpaceController : BaseController
{
    public SpaceController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
    {
        var url = $"{_apiBaseUrl}/Space?page={page}&pageSize={pageSize}"; // URL avec pagination
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

        // Désérialisation de la réponse
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);

        // Récupérer les espaces et la pagination
        var spaces = result?.spaces ?? new List<dynamic>();
        var pagination = result?.pagination;

        // Passer les données à la vue
        ViewData["JsonSpaceList"] = Newtonsoft.Json.JsonConvert.SerializeObject(spaces);
        ViewData["Pagination"] = pagination;
        ViewData["ApiBaseUrl"] = GetApiBaseUrl();
        return View();
    }

    public async Task<IActionResult> Details(int id)
    {
        var url = $"{_apiBaseUrl}/Space/{id}"; // URL pour obtenir les détails de l'espace par ID
        string jsonData = "{}";
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

        // Désérialisation de la réponse
        var space = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
        if (space == null)
        {
            return NotFound();
        }

        // Passer le JSON brut dans ViewData
        ViewData["JsonSpace"] = Newtonsoft.Json.JsonConvert.SerializeObject(space);

        Console.WriteLine(ViewData["JsonSpace"]);

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
