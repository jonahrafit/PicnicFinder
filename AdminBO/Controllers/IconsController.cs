using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class IconsController : Controller
{
    public IActionResult Boxicons() => View();
}
