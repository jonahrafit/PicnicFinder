using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class AuthController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;
    private readonly AdminBOContext _dbContext;

    public AuthController(
        ILogger<AuthController> logger,
        IConfiguration configuration,
        AdminBOContext dbContext
    )
        : base(logger, configuration)
    {
        _dbContext = dbContext;
        _authService = new AuthService(configuration, dbContext);
    }

    public IActionResult ForgotPasswordBasic() => View();

    public IActionResult LoginBasic() => View();

    public IActionResult RegisterBasic() => View();

    // POST: Auth/Authenticate
    public async Task<IActionResult> Authenticate(string username, string password)
    {
        var token = await _authService.AuthenticateAsync(username, password);

        if (token == null)
        {
            ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
            return View("LoginBasic");
        }

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

        return RedirectToAction("Index", "Dashboards");
    }

    // POST: Auth/Logout
    public IActionResult Logout()
    {
        // Suppression du cookie jwt
        Response.Cookies.Delete("jwt");

        // Message de déconnexion
        ViewBag.logoutMessage = "Vous avez été déconnecté avec succès.";

        return View("LoginBasic");
    }
}
