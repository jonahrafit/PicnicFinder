using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace FrontOffice.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    private readonly string _apiBaseUrl;

    private readonly IConfiguration _configuration;

    public IActionResult Index()
    {
        return View();
    }

    public AuthController(ILogger<AuthController> logger, IConfiguration configuration)
    {
        _configuration = configuration;
        _logger = logger;
        _apiBaseUrl =
            _configuration["ServerSettings:ApiBaseUrl"]
            ?? throw new ArgumentNullException(nameof(configuration));
    }

    // POST: Auth/Authenticate
    public async Task<IActionResult> Authenticate(string username, string password)
    {
        string token = null; // Pour stocker le jeton si l'authentification réussit
        var url = $"{_apiBaseUrl}/auth/login"; // Remplacez par l'URL de votre API d'authentification

        try
        {
            // Préparer les données à envoyer
            var loginData = new { Username = username, Password = password };

            // Sérialiser les données en JSON
            var jsonContent = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(loginData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            // Envoyer la requête POST
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Récupérer le jeton depuis la réponse
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(
                        responseContent
                    );
                    token = result?.token;
                }
                else
                {
                    _logger.LogWarning(
                        $"Authentication failed with status code: {response.StatusCode}"
                    );
                    ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
                    ViewData["HideNavbar"] = true;
                    return View("Index");
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calling the authentication API.");
            ViewBag.ErrorMessage = "Une erreur est survenue. Veuillez réessayer plus tard.";
            ViewData["HideNavbar"] = true;
            return View("Index");
        }

        if (string.IsNullOrEmpty(token))
        {
            ViewBag.ErrorMessage = "Impossible de récupérer le jeton.";
            ViewData["HideNavbar"] = true;
            return View("Index");
        }

        // Stockage du token côté client (par exemple via un cookie)
        Response.Cookies.Append(
            "jwt",
            token,
            new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddHours(1),
            }
        );

        return RedirectToAction("Index", "Space");
    }
}
