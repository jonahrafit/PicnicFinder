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

        public AuthController(IConfiguration configuration)
        {
            _configuration = configuration;
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
        public IActionResult Authenticate(string username, string password)
        {
            var secretKey = _configuration["Jwt:SecretKey"];
            // Vérification si la clé secrète est null ou vide
            if (string.IsNullOrEmpty(secretKey))
            {
                // Si la clé est manquante, lever une exception ou utiliser une valeur par défaut
                throw new InvalidOperationException("La clé secrète JWT n'est pas configurée.");
            }

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Vérification des identifiants
            string role;
            if (username == "admin" && password == "password")
            {
                role = "ADMIN";
            }
            else if (username == "jonah" && password == "jonah")
            {
                role = "OWNER";
            }
            else
            {
                // Nom d'utilisateur ou mot de passe incorrect
                ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
                ViewData["HideNavbar"] = true;
                return View("Index");
            }

            // Création des claims (incluant le rôle)
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role)
            };

            // Génération du token JWT
            var token = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                claims: claims,
                expires: DateTime.Now.AddHours(1), // Durée de validité du token
                signingCredentials: credentials
            );

            var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

            // Stockage du token côté client (par exemple via un cookie)
            Response.Cookies.Append("jwt", jwtToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = DateTimeOffset.Now.AddHours(1)
            });

            // Redirection vers la page d'accueil ou autre
            return RedirectToAction("Index", "Home");
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
