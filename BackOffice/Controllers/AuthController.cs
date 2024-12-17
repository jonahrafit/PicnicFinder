using Microsoft.AspNetCore.Mvc;

namespace MyApp.Namespace
{
    public class AuthController : Controller
    {
        // GET: Auth/Login
        public IActionResult Index()
        {
            ViewData["HideNavbar"] = true;  // Masquer la navbar sur cette page
            return View(); // Affiche la vue Login.cshtml
        }

        // POST: Auth/Authenticate
        [HttpPost]
        public IActionResult Authenticate(string username, string password)
        {
            // Simule une vérification d'authentification
            if (username == "admin" && password == "password")
            {
                // Redirige vers une page d'accueil ou autre
                return RedirectToAction("Index", "Home");
            }

            // Ajoute un message d'erreur et reste sur la page Login
            ViewBag.ErrorMessage = "Nom d'utilisateur ou mot de passe incorrect.";
            return View("Index");
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
