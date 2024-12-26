using System.Text;                        // Pour 'Encoding'
using System.Threading.Tasks;
using System.IdentityModel.Tokens.Jwt;    // Pour 'JwtSecurityToken', 'JwtSecurityTokenHandler'
using System.Security.Claims;             // Pour 'Claim', 'ClaimTypes'
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;  // Pour 'AuthorizeAttribute'
using Microsoft.IdentityModel.Tokens;     // Pour 'SymmetricSecurityKey', 'SigningCredentials', etc.
using PicnicFinder.Models;

namespace MyApp.Namespace
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly AuthService _authService;
        private readonly PicnicFinderContext _dbContext;

        public AuthController(IConfiguration configuration, PicnicFinderContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _authService = new AuthService(_configuration, _dbContext);
        }

        // GET: Auth/Login
        [AllowAnonymous]
        public IActionResult Index()
        {
            ViewData["HideNavbar"] = true;  // Masquer la navbar sur cette page
            return View(); // Affiche la vue Login.cshtml
        }

        // POST: Auth/Authenticate
        [HttpPost]
        [Route("api/auth/login")]
        public async Task<IActionResult> Authenticate(string username, string password)
        {
            var token = await _authService.AuthenticateAsync(username, password);

            if (token == null)
            {
                ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
                ViewData["HideNavbar"] = true;
                return View("Index");
            }

            // Stockage du token côté client (par exemple via un cookie)
            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            return RedirectToAction("Index", "Home");
        }

        // POST: Auth/Signup
        [HttpPost]
        [Route("api/auth/signup")]
        public async Task<IActionResult> Signup([FromBody] SignupModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.Email) || string.IsNullOrEmpty(model.Password) || string.IsNullOrEmpty(model.Role) || string.IsNullOrEmpty(model.Name))
            {
                return BadRequest("Veuillez fournir toutes les informations nécessaires.");
            }

            var result = await _authService.SignupAsync(model.Email, model.Password, model.Role, model.Phone, model.Name);

            if (result)
            {
                return Ok("Inscription réussie. Un email de confirmation a été envoyé.");
            }
            else
            {
                return BadRequest("Y a un problème lors de l'inscription.");
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            ViewData["HideNavbar"] = true;
            ViewBag.logoutMessage = "Déconnecté";
            return View("Index");
        }
    }
}
