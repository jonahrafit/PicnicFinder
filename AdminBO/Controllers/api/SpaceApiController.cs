using System.Collections.Generic;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace AdminBO.Controllers.Api
{
    [Route("api/space")]
    [ApiController]
    public class SpaceApiController : Controller
    {
        private readonly SpaceService _spaceService;
        private readonly SpaceActivityService _spaceActivityService;
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly AdminBOContext _dbContext;

        public SpaceApiController(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _userService = new UserService(_configuration, _dbContext);
            _spaceService = new SpaceService(_configuration, _dbContext);
            _spaceActivityService = new SpaceActivityService(_configuration, _dbContext);
        }

        // GET: api/Space
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Space>>> GetSpaces(
            int page = 1,
            int pageSize = 10
        )
        {
            var spaces = await _spaceService.GetSpacesPagedAsync(page, pageSize);

            if (spaces == null || spaces.Count == 0)
            {
                return NotFound("Aucun espace trouvé.");
            }

            // LIST ACTIVITY
            var viewSpaceWithActivities = new List<ViewSpaceWithActivities>();
            foreach (var space in spaces)
            {
                var spaceWithActivities = new ViewSpaceWithActivities
                {
                    Space = space,
                    SpaceActivities =
                        await _spaceActivityService.GetAllSpaceActivityNameBySpaceIdAsync(space.Id),
                };

                viewSpaceWithActivities.Add(spaceWithActivities);
            }

            // PAGINATION
            // Nombre total d'espaces
            var totalSpaces = await _spaceService.GetTotalSpacesCountAsync();

            // Création d'un objet de pagination
            var paginationModel = new PaginationModel
            {
                CurrentPage = page,
                TotalItems = totalSpaces,
                ItemsPerPage = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalSpaces / pageSize),
            };

            var result = new { Spaces = viewSpaceWithActivities, Pagination = paginationModel };
            return Ok(result);
        }

        // GET: api/Space/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Space>> GetSpace(long id)
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
                SpaceActivities =
                    await _spaceActivityService.GetAllSpaceActivityLinksBySpaceIdAsync(space.Id),
            };

            return Ok(spaceDetailsWithActivities);
        }

        private long GetCurrentUserId()
        {
            var userIdClaim = User?.FindFirst("UserId")?.Value;
            return userIdClaim != null ? long.Parse(userIdClaim) : 0;
        }
    }
}
