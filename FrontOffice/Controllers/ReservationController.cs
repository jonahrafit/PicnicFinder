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
        Console.WriteLine(bearerToken);

        if (!string.IsNullOrEmpty(bearerToken))
        {
            Console.WriteLine("------3-------");
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

        // Désérialisation de la réponse
        var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData);
        Console.WriteLine(result);
        // Récupérer les espaces et la pagination
        var reservations = result?.reservations ?? new List<dynamic>();
        var pagination = result?.pagination;

        // Passer les données à la vue
        ViewData["JsonReservationList"] = Newtonsoft.Json.JsonConvert.SerializeObject(reservations);
        ViewData["Pagination"] = pagination;

        return View(reservations);
    }

    [HttpPost]
    public async Task<IActionResult> CreateReservation(ReservationViewModel model)
    {
        Console.WriteLine("---------------------");
        if (!ModelState.IsValid)
        {
            RedirectToAction("Index", "Space");
        }

        string apiUrl = $"{_apiBaseUrl}/api/reservation";
        string bearerToken = Request.Cookies["jwt"];
        Console.WriteLine(bearerToken);
        // if (string.IsNullOrEmpty(bearerToken))
        // {
        //     ModelState.AddModelError(string.Empty, "Token non trouvé dans les cookies.");
        //     return View(model);
        // }

        // try
        // {
        //     using (var httpClient = new HttpClient())
        //     {
        //         httpClient.DefaultRequestHeaders.Authorization =
        //             new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", bearerToken);

        //         var requestData = new
        //         {
        //             SpaceId = model.SpaceId,
        //             ReservationDate = DateTime.UtcNow,
        //             StartDate = model.StartDate,
        //             EndDate = model.EndDate,
        //             Status = "PENDING",
        //         };

        //         string jsonContent = JsonSerializer.Serialize(requestData);
        //         var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

        //         var response = await httpClient.PostAsync(apiUrl, content);

        //         if (response.IsSuccessStatusCode)
        //         {
        //             return RedirectToAction("Index", "Reservation"); // Redirection vers l'Index
        //         }
        //         else
        //         {
        //             string errorMessage = await response.Content.ReadAsStringAsync();
        //             ModelState.AddModelError(string.Empty, $"Erreur de l'API : {errorMessage}");
        //         }
        //     }
        // }
        // catch (Exception ex)
        // {
        //     ModelState.AddModelError(string.Empty, $"Erreur : {ex.Message}");
        // }

        return RedirectToAction("Index");
    }
}
