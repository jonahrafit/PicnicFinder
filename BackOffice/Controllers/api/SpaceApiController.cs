using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;
using System.Text;
using System.IdentityModel.Tokens.Jwt;    // Pour 'JwtSecurityToken', 'JwtSecurityTokenHandler'
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;  // Pour 'AuthorizeAttribute'
using Microsoft.IdentityModel.Tokens;    // Pour 'SymmetricSecurityKey', 'SigningCredentials', etc.
using BackOffice.Services;

namespace BackOffice.Controllers.Api
{
    [Route("api/space")]
    [ApiController]
    public class SpaceApiController : Controller
    {
        private readonly SpaceService _spaceService;
        private readonly IConfiguration _configuration;
        private readonly PicnicFinderContext _dbContext;

        public SpaceApiController(IConfiguration configuration, PicnicFinderContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _spaceService = new SpaceService(_configuration, _dbContext);
        }

        // GET: api/Space
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Space>>> GetSpaces()
        {
            var spaces = await _spaceService.GetAllSpacesAsync();
            if (spaces == null || spaces.Count == 0)
            {
                return NotFound("Aucun espace trouv�.");
            }
            return Ok(spaces);
        }

        // GET: api/Space/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Space>> GetSpace(long id)
        {
            var space = await _spaceService.GetSpaceByIdAsync(id);
            if (space == null)
            {
                return NotFound($"Espace avec l'ID {id} non trouv�.");
            }
            return Ok(space);
        }

        // POST: api/Space
        [HttpPost]
        [Authorize(Roles = "OWNER")]  // Seuls les utilisateurs avec le r�le "OWNER" peuvent cr�er un espace
        public async Task<ActionResult<Space>> CreateSpace([FromBody] Space space)
        {
            if (space == null)
            {
                return BadRequest("Les donn�es de l'espace ne sont pas valides.");
            }

            Console.WriteLine($"____________________");
            space.OwnerId = GetCurrentUserId();
            Console.WriteLine($"Creating space: {space.ToString()}");
            Console.WriteLine($"____________________");
            await _spaceService.CreateSpaceAsync(space);

            // Retourner une r�ponse avec l'ID du nouvel espace cr��
            return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, space);
        }

        private long GetCurrentUserId()
        {
            // Cette m�thode doit �tre impl�ment�e pour r�cup�rer l'ID de l'utilisateur connect�.
            // Par exemple, vous pouvez l'extraire des claims du jeton JWT.
            var userIdClaim = User?.FindFirst("UserId")?.Value;
            return userIdClaim != null ? long.Parse(userIdClaim) : 0;
        }
    }
}
