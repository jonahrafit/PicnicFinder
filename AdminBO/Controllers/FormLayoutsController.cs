using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class FormLayoutsController : Controller
{
    public IActionResult Horizontal() => View();

    public IActionResult Vertical() => View();
}
