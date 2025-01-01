using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class DashboardsController : Controller
{
    public IActionResult Index() => View();
}
