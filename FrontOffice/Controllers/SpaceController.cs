using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrontOffice.Controllers;

public class SpaceController : BaseController
{
    public SpaceController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 3)
    {
        var url = $"{_apiBaseUrl}/Space?page={page}&pageSize={pageSize}";
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

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
        // Récupérer les espaces et la pagination
        var spaces = result?.spaces ?? new List<dynamic>();
        var pagination = result?.pagination;
        var spaceActivities = result?.spaceActivityList ?? new List<dynamic>();
        var SpaceActivityListJson = JsonConvert.SerializeObject(spaceActivities);

        // Échapper les apostrophes et autres caractères spéciaux
        SpaceActivityListJson = SpaceActivityListJson.Replace("'", "\\'");

        // Passer les données à la vue
        ViewData["SpaceActivityListJson"] = SpaceActivityListJson;
        ViewData["JsonSpaceList"] = Newtonsoft.Json.JsonConvert.SerializeObject(spaces);
        ViewData["Pagination"] = pagination;
        ViewData["SpaceActivityListJson"] = SpaceActivityListJson;
        ViewData["ApiBaseUrl"] = GetApiBaseUrl();
        ViewData["ImageBaseUrl"] = GetImageBaseUrl();
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

        ViewData["JsonSpace"] = jsonData;
        ViewData["ImageBaseUrl"] = GetImageBaseUrl();
        ViewData["ApiBaseUrl"] = GetApiBaseUrl();
        ViewBag.SpaceDetails = space;

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
