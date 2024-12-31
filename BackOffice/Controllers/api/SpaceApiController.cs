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
        private readonly UserService _userService;
        private readonly IConfiguration _configuration;
        private readonly PicnicFinderContext _dbContext;

        public SpaceApiController(IConfiguration configuration, PicnicFinderContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _userService = new UserService(_configuration, _dbContext);
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

            space.OwnerId = GetCurrentUserId();
            try
            {
                // Récupérer l'ID de l'utilisateur actuellement authentifié
                var currentUserId = GetCurrentUserId();

                // Récupérer l'utilisateur à partir de la base de données
                var owner = await _userService.GetUserByIdAsync(currentUserId);

                if (owner == null)
                {
                    return Unauthorized("Utilisateur non trouvé.");
                }

                // Assigner l'owner à l'espace
                space.Owner = owner;
                await _spaceService.CreateSpaceAsync(space);

                // Retourner l'espace créé avec un code de statut 201
                return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, space);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de l'espace : {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue.");
            }
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
