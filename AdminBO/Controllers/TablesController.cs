using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class TablesController : Controller
{
    public IActionResult Basic() => View();
}
