using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class CardsController : Controller
{
    public IActionResult Basic() => View();
}
