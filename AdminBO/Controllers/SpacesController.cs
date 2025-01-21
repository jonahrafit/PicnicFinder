using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Controllers;

public class SpacesController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly SpaceService _spaceService;
    private readonly UserService _userService;
    private readonly SpaceActivityService _spaceActivityService;
    private readonly SpaceServiceAdoNet _spaceServiceAdoNet;
    private readonly AdminBOContext _dbContext;

    public SpacesController(
        ILogger<SpacesController> logger,
        IConfiguration configuration,
        AdminBOContext dbContext
    )
        : base(logger, configuration)
    {
        _dbContext = dbContext;
        _spaceService = new SpaceService(base._configuration, _dbContext);
        _userService = new UserService(base._configuration, _dbContext);
        _spaceActivityService = new SpaceActivityService(base._configuration, _dbContext);
        _spaceServiceAdoNet = new SpaceServiceAdoNet(base._configuration);
    }

    [Authorize(Roles = "ADMIN, OWNER")]
    public async Task<IActionResult> Index(long? ownerId = null)
    {
        var userIdClaim = HttpContext.User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userIdClaim))
        {
            throw new Exception("Utilisateur non authentifié ou ID d'utilisateur manquant");
        }
        long owner_Id = long.Parse(userIdClaim);
        var spaces = await _spaceService.GetAllSpacesAsyncByOwner(owner_Id);
        return View("Basic", spaces);
    }

    public async Task<ActionResult<Space>> Details(long id)
    {
        var space = await _spaceService.GetSpaceByIdAsyncByOwner(id);

        if (space == null)
        {
            return NotFound($"Espace avec l'ID {id} non trouv�.");
        }

        List<SpaceActivity> spaceActivities = new List<SpaceActivity>();
        var spaceDetailsWithActivities = new ViewDetailsSpaceWithActivities
        {
            Space = space,
            SpaceActivities = _spaceActivityService.GetAllSpaceActivityLinksBySpaceIdAsync(
                space.Id
            ),
        };

        return View(spaceDetailsWithActivities);
    }

    // GET: Space/Create
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> CreateForm()
    {
        var spaceActivityList = await _spaceActivityService.GetAllSpaceActivitysAsync();
        return View(spaceActivityList);
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

    public async Task<IActionResult> GetSpaceById(int id)
    {
        var space = await Task.Run(() => _spaceServiceAdoNet.GetSpaceById(id));
        Console.WriteLine(space);

        if (space == null)
        {
            return NotFound();
        }

        // Passer l'objet space dans ViewData
        ViewData["JsonSpace"] = Newtonsoft.Json.JsonConvert.SerializeObject(space);

        Console.WriteLine(ViewData["JsonSpace"]);

        return View();
    }

    [IgnoreAntiforgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> CreateSpace([FromForm] SpaceFormDTO spaceForm)
    {
        try
        {
            spaceForm.Latitude = Convert.ToDouble(spaceForm.Latitude, CultureInfo.InvariantCulture);
            spaceForm.Longitude = Convert.ToDouble(
                spaceForm.Longitude,
                CultureInfo.InvariantCulture
            );
        }
        catch (FormatException ex)
        {
            return BadRequest(new { error = "Invalid format for Latitude or Longitude." });
        }

        long[] activityIdsArray = new long[0];

        if (!string.IsNullOrEmpty(spaceForm.ActivityIds))
        {
            try
            {
                activityIdsArray = spaceForm
                    .ActivityIds.Split(',')
                    .Select(id => Convert.ToInt64(id))
                    .ToArray();
            }
            catch (FormatException ex)
            {
                return BadRequest(new { error = "Certains IDs d'activités sont invalides." });
            }
        }

        Console.WriteLine("------------1----------------");
        Console.WriteLine(spaceForm.ToString());
        Console.WriteLine("----------------------------");

        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        try
        {
            var activities = await _dbContext
                .SpaceActivities.Where(activity => activityIdsArray.Contains((long)activity.Id)) // Assurez-vous que types sont compatibles
                .ToListAsync();

            Console.WriteLine("-------------2---------------");
            Console.WriteLine(activities.Count());
            foreach (var activity in activities)
            {
                Console.WriteLine(activity.ToString());
            }
            Console.WriteLine("----------------------------");

            // Vérifiez si tous les IDs fournis existent dans la base de données
            if (activities.Count != activityIdsArray.Length)
            {
                return BadRequest("Certains IDs d'activités ne sont pas valides.");
            }

            // Vous pouvez aussi vérifier si tous les IDs sont présents
            if (activityIdsArray.Except(activities.Select(a => a.Id)).Any())
            {
                return BadRequest("Certains IDs d'activités ne sont pas valides.");
            }

            var currentUserId = GetCurrentUserId();
            var owner = await _userService.GetUserByIdAsync(currentUserId);
            if (owner == null)
            {
                return Unauthorized("Utilisateur non trouvé.");
            }

            Console.WriteLine("-------------3---------------");
            List<string> uploadedFileNames = new List<string>();
            foreach (var file in spaceForm.Photos)
            {
                var fileName = Path.GetFileName(file.FileName);
                var filePath = Path.Combine("wwwroot/img", fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uploadedFileNames.Add(fileName);
            }

            Console.WriteLine("-------------4---------------");
            var space = new Space
            {
                OwnerId = spaceForm.OwnerId,
                Name = spaceForm.Name,
                Latitude = spaceForm.Latitude,
                Longitude = spaceForm.Longitude,
                Capacity = spaceForm.Capacity,
                Photos = uploadedFileNames,
                Description = spaceForm.Description,
                Status = Enum.Parse<SpaceStatus>("PENDING", true),
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Owner = owner,
            };

            Console.WriteLine("-------------5---------------");
            Console.WriteLine(space.ToString());
            Console.WriteLine("----------------------------");

            // Ajout des liens entre Space et SpaceActivity (relation many-to-many)
            var spaceActivityLinks = activities
                .Select(activity => new SpaceActivityLink
                {
                    Space = space,
                    SpaceActivity = activity,
                })
                .ToList();

            Console.WriteLine(
                $"Number of SpaceActivityLinks to be added: {spaceActivityLinks.Count}"
            );
            foreach (var link in spaceActivityLinks)
            {
                Console.WriteLine(
                    $"SpaceId: {link.SpaceId}, SpaceActivityId: {link.SpaceActivityId}"
                );
            }

            _dbContext.SpaceActivityLinks.AddRange(spaceActivityLinks);
            _dbContext.Spaces.Add(space);
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error saving changes: {ex.Message}");
            }

            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur lors de la création de l'espace : {ex.Message}");
            return StatusCode(500, "Une erreur interne est survenue.");
        }
    }
}
