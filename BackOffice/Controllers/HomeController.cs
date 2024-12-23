﻿using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PicnicFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;  // Pour UserManager et ApplicationUser
using System.Threading.Tasks;
using System.Security.Claims;

namespace PicnicFinder.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly MenuService _menuService;

        public HomeController(ILogger<HomeController> logger, MenuService menuService)
        {
            _menuService = menuService;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var username = User.Identity?.Name;
    if (string.IsNullOrEmpty(username))
    {
        // Gérer le cas où le nom d'utilisateur est absent (par exemple, rediriger vers la page de connexion)
        // Gérer le cas où le nom d'utilisateur est absent (par exemple, rediriger vers la page de connexion)
        return RedirectToAction("Login", "Auth");
    }

    // Récupérer le rôle à partir du JWT (claims)
    var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

    // Vérifier si le rôle est null
    if (string.IsNullOrEmpty(userRole))
    {
        // Gérer le cas où le rôle est absent
        return RedirectToAction("Login", "Auth");
    }

    // Optionnel : afficher les informations utilisateur dans le journal ou pour d'autres traitements
    _logger.LogInformation($"Utilisateur connecté : {username} avec rôle : {userRole}");

    // Récupérer les menus accessibles pour cet utilisateur (selon son rôle)
    var menus = await _menuService.GetMenusForUserAsync(new[] { userRole });

    ViewData["ActiveMenu"] = "Dashboard";
    return View(menus);
}


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
