using System.Diagnostics;
using System.Net.Http; // Pour HttpClient et StringContent
using System.Text; // Pour Encoding
using System.Text.Json; // Pour JsonSerializer
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class ReservationController : BaseController
{
    public ReservationController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    public async Task<IActionResult> Index(int page = 1, int pageSize = 5)
    {
        var url = $"{_apiBaseUrl}/reservation?page={page}&pageSize={pageSize}"; // URL avec pagination
        string jsonData = "[]";
        string bearerToken = Request.Cookies["jwt"];

        if (!string.IsNullOrEmpty(bearerToken))
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Authorization =
                        new System.Net.Http.Headers.AuthenticationHeaderValue(
                            "Bearer",
                            bearerToken
                        );

                    // Ajouter d'autres en-têtes si nécessaire
                    httpClient.DefaultRequestHeaders.Add("Accept", "application/json");

                    var response = await httpClient.GetAsync(url);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonData = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Response Content: {jsonData}");
                    }
                    else
                    {
                        Console.WriteLine(
                            $"Response Code: {response.StatusCode}, Message: {response.ReasonPhrase}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while calling the API.");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
        var reservationsJson = result?.reservations?.ToString();
        var reservations = Newtonsoft.Json.JsonConvert.DeserializeObject<
            List<ReservationViewModel>
        >(reservationsJson);

        var paginationJson = result?.pagination?.ToString();
        var pagination = Newtonsoft.Json.JsonConvert.DeserializeObject<PaginationModel>(
            paginationJson
        );

        // Passer les données à la vue
        ViewData["JsonReservationList"] = reservations;
        ViewData["Pagination"] = result?.pagination;

        var viewModel = new ViewModelReservationList
        {
            Reservations = reservations,
            Pagination = pagination,
        };
        // Retourner la vue
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(ReservationViewModel model)
    {
        Console.WriteLine(model.ToString());
        if (!ModelState.IsValid)
        {
            RedirectToAction("Index", "Space");
        }

        string apiUrl = $"{_apiBaseUrl}/reservation";
        string bearerToken = Request.Cookies["jwt"];
        Console.WriteLine(bearerToken);
        if (string.IsNullOrEmpty(bearerToken))
        {
            ModelState.AddModelError(string.Empty, "Token non trouvé dans les cookies.");
            return View(model);
        }

        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

                var requestData = new
                {
                    SpaceId = model.SpaceId,
                    ReservationDate = DateTime.UtcNow,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                };
                Console.WriteLine(requestData);

                string jsonContent = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await httpClient.PostAsync(apiUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Reservation");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError(string.Empty, $"Erreur de l'API : {errorMessage}");
                }
            }
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"Erreur : {ex.Message}");
        }

        return RedirectToAction("Index");
    }
}
