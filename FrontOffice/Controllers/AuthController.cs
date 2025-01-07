using System.Diagnostics;
using FrontOffice.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace FrontOffice.Controllers;

public class AuthController : BaseController
{
    public AuthController(ILogger<SpaceController> logger, IConfiguration configuration)
        : base(logger, configuration) { }

    public IActionResult Index()
    {
        return View();
    }

    public async Task<IActionResult> Authenticate([FromBody] SigninModel request)
    {
        var url = $"{_apiBaseUrl}/auth/login";
        try
        {
            var loginData = new { Username = request.Username, Password = request.Password };

            _logger.LogInformation(
                "Sending authentication request for user: {Username}",
                request.Username
            );

            var jsonContent = new StringContent(
                Newtonsoft.Json.JsonConvert.SerializeObject(loginData),
                System.Text.Encoding.UTF8,
                "application/json"
            );

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(url, jsonContent);
                if (response.IsSuccessStatusCode)
                {
                    var responseContent = await response.Content.ReadAsStringAsync();
                    var result = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(
                        responseContent
                    );
                    var token = result?.token?.ToString();

                    if (!string.IsNullOrEmpty(token))
                    {
                        // Stockage du token côté client
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
                        return Ok(new { token });
                    }
                }
                else
                {
                    _logger.LogWarning(
                        $"Authentication failed with status code: {response.StatusCode}"
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while calling the authentication API.");
            // Log the exception
            Console.WriteLine($"_________________ : {ex.Message}");
            // Rethrow the exception if needed
            throw;
        }

        return null;
    }

    public async Task<IActionResult> UserInfo(String token)
    {
        var url = $"{_apiBaseUrl}/auth/user-info";
        try
        {
            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var response = await httpClient.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var jsonResponse = await response.Content.ReadAsStringAsync();
                    // Désérialisation en type dynamique pour accéder directement aux données JSON
                    dynamic userInfo = JsonConvert.DeserializeObject(jsonResponse);

                    // Retourner directement la réponse avec l'objet utilisateur extrait
                    return Ok(
                        new
                        {
                            success = true,
                            user = new { name = userInfo.name, role = userInfo.role },
                        }
                    );
                }
                else
                {
                    return StatusCode(
                        (int)response.StatusCode,
                        new
                        {
                            success = false,
                            message = "Échec de l'authentification.",
                            details = await response.Content.ReadAsStringAsync(),
                        }
                    );
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            return StatusCode(
                500,
                new
                {
                    success = false,
                    message = "Une erreur interne est survenue.",
                    details = ex.Message,
                }
            );
        }
    }
}
