using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class FormsController : Controller
{
    public IActionResult BasicInputs() => View();

    public IActionResult InputGroups() => View();
}
