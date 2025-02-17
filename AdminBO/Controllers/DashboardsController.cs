using System.Diagnostics;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class DashboardsController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly SpaceService _spaceService;
    private readonly AdminBOContext _dbContext;

    public DashboardsController(
        ILogger<DashboardsController> logger,
        IConfiguration configuration,
        AdminBOContext dbContext
    )
        : base(logger, configuration)
    {
        _dbContext = dbContext;
        _spaceService = new SpaceService(base._configuration, _dbContext);
    }
    public async Task<IActionResult> Index()
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("Utilisateur non authentifié ou ID d'utilisateur manquant");
        }
        long owner_Id = long.Parse(userIdClaim);
        var userRole = HttpContext.User.FindFirst("Role")?.Value;
        if (userRole == "ADMIN")
        {
            Console.WriteLine("ato ooo -------------");
            return View(new List<Space>());
        }
        var spaces = await _spaceService.GetAllSpacesAsyncByOwner(owner_Id);
        if (spaces == null || spaces.Count == 0)
        {
            Console.WriteLine("------2-------");
            return View(new List<Space>());
        }
        return View(spaces);
    }
}
