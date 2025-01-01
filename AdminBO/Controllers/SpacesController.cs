using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class SpacesController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly SpaceService _spaceService;
    private readonly AdminBOContext _dbContext;

    public SpacesController(
        IConfiguration configuration,
        ILogger<HomeController> logger,
        AdminBOContext dbContext
    )
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _spaceService = new SpaceService(_configuration, _dbContext);
    }

    [Authorize(Roles = "ADMIN, OWNER")]
    public async Task<IActionResult> Index(long? ownerId = null)
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("Utilisateur non authentifié ou ID d'utilisateur manquant.");
        }
        long owner_Id = long.Parse(userIdClaim);
        var spaces = await _spaceService.GetAllSpacesAsyncByOwner(owner_Id);
        return View("Basic", spaces);
    }

    [Authorize(Roles = "ADMIN, OWNER")]
    // GET: Space/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var space = await _spaceService.GetSpaceByIdAsync(id.Value);
        if (space == null)
        {
            return NotFound();
        }

        return View(space);
    }

    // GET: Space/Create
    [Authorize(Roles = "OWNER")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: Space/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            Space space
    )
    {
        if (ModelState.IsValid)
        {
            await _spaceService.CreateSpaceAsync(space);
            return RedirectToAction(nameof(Index));
        }
        return View(space);
    }

    // GET: Space/Edit/5
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var space = await _spaceService.GetSpaceByIdAsync(id.Value);
        if (space == null)
        {
            return NotFound();
        }
        return View(space);
    }

    // POST: Space/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(
        long id,
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            Space space
    )
    {
        if (id != space.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _spaceService.UpdateSpaceAsync(space);
            }
            catch
            {
                if (!await _spaceService.SpaceExistsAsync(space.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(space);
    }

    // GET: Space/Delete/5
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var space = await _spaceService.GetSpaceByIdAsync(id.Value);
        if (space == null)
        {
            return NotFound();
        }

        return View(space);
    }

    // POST: Space/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _spaceService.DeleteSpaceAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
