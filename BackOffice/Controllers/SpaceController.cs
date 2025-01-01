using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PicnicFinder.Models;
using Microsoft.AspNetCore.Authorization;
using BackOffice.Services;

namespace BackOffice.Controllers
{
    public class SpaceController : Controller
    {
        private readonly SpaceService _spaceService;
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        private readonly PicnicFinderContext _dbContext;

        public SpaceController(SpaceService spaceService, ILogger<HomeController> logger, MenuService menuService,IConfiguration configuration, PicnicFinderContext dbContext)
        {
            _spaceService = new SpaceService(_configuration, _dbContext);
            _logger = logger;
        }

        // GET: Space
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "GestionDesEspaces";
            var spaces = await _spaceService.GetAllSpacesAsyncByOwner();
            return View(spaces);
        }

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
        public async Task<IActionResult> Create([Bind("Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt")] Space space)
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
        public async Task<IActionResult> Edit(long id, [Bind("Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt")] Space space)
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
}
