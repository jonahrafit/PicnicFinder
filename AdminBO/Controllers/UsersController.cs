using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class UsersController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly UserService _userService;
    private readonly CsvImportService _csvImportService;
    private readonly AdminBOContext _dbContext;

    public UsersController(
        CsvImportService csvImportService,
        ILogger<UsersController> logger,
        IConfiguration configuration,
        AdminBOContext dbContext
    )
        : base(logger, configuration)
    {
        _dbContext = dbContext;
        _userService = new UserService(_configuration, _dbContext);
        _csvImportService = csvImportService;
    }

    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> ImportCsv([FromForm] IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("Le fichier CSV est requis.");
        }
        // Vérification de l'extension
        var fileExtension = Path.GetExtension(file.FileName);
        if (!string.Equals(fileExtension, ".csv", StringComparison.OrdinalIgnoreCase))
        {
            return BadRequest("Seuls les fichiers avec l'extension .csv sont autorisés.");
        }
        try
        {
            using var stream = file.OpenReadStream();
            await _csvImportService.ImportUsersFromCsvAsync(stream);

            // Ajouter un message de succès dans TempData
            TempData["SuccessMessage"] = "Importation réussie.";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Une erreur est survenue : {ex.Message}");
        }
    }

    [Authorize(Roles = "ADMIN")]
    public async Task<IActionResult> Index()
    {
        var users = await _userService.GetAllUsersAsync();
        foreach (var user in users)
        {
            Console.WriteLine(user.ToString());
        }
        return View("Basic", users);
    }

    [Authorize(Roles = "ADMIN")]
    // GET: User/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userService.GetUserByIdAsync(id.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // GET: User/Create
    [Authorize(Roles = "OWNER")]
    public IActionResult Create()
    {
        return View();
    }

    // POST: User/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            User user
    )
    {
        if (ModelState.IsValid)
        {
            await _userService.CreateUserAsync(user);
            return RedirectToAction(nameof(Index));
        }
        return View(user);
    }

    // GET: User/Edit/5
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userService.GetUserByIdAsync(id.Value);
        if (user == null)
        {
            return NotFound();
        }
        return View(user);
    }

    // POST: User/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(
        long id,
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            User user
    )
    {
        if (id != user.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _userService.UpdateUserAsync(user);
            }
            catch
            {
                if (!await _userService.UserExistsAsync(user.Id))
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
        return View(user);
    }

    // GET: User/Delete/5
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userService.GetUserByIdAsync(id.Value);
        if (user == null)
        {
            return NotFound();
        }

        return View(user);
    }

    // POST: User/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _userService.DeleteUserAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
