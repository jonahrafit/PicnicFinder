using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class ExtendedUiController : Controller
{
    public IActionResult PerfectScrollbar() => View();

    public IActionResult TextDivider() => View();
}
