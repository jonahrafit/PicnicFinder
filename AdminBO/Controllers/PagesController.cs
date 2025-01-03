using System.Diagnostics;
using AdminBO.Models;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class PagesController : Controller
{
    public IActionResult AccountSettings() => View();

    public IActionResult AccountSettingsConnections() => View();

    public IActionResult AccountSettingsNotifications() => View();

    public IActionResult MiscError() => View();

    public IActionResult MiscUnderMaintenance() => View();
}
