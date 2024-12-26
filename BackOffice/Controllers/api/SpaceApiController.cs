using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;
using BackOffice.Services;
using System.Text;           
using System.IdentityModel.Tokens.Jwt;    // Pour 'JwtSecurityToken', 'JwtSecurityTokenHandler'
using System.Security.Claims;   
using Microsoft.AspNetCore.Authorization;  // Pour 'AuthorizeAttribute'
using Microsoft.IdentityModel.Tokens;     // Pour 'SymmetricSecurityKey', 'SigningCredentials', etc.

namespace BackOffice.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class SpaceApiController : Controller
    {
        private readonly SpaceService _spaceService;

        public SpaceApiController(SpaceService spaceService)
        {
            _spaceService = spaceService;
        }

        // GET: api/Space
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Space>>> GetSpaces()
        {
            var spaces = await _spaceService.GetAllSpacesAsync();
            if (spaces == null || spaces.Count == 0)
            {
                return NotFound("Aucun espace trouvé.");
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
                return NotFound($"Espace avec l'ID {id} non trouvé.");
            }
            return Ok(space);
        }

        // POST: api/Space
        [HttpPost]
        [Authorize(Roles = "OWNER")]  // Seuls les utilisateurs avec le rôle "OWNER" peuvent créer un espace
        public async Task<ActionResult<Space>> CreateSpace([FromBody] Space space)
        {
            if (space == null)
            {
                return BadRequest("Les données de l'espace ne sont pas valides.");
            }

            // Assurez-vous que l'utilisateur connecté est le propriétaire de l'espace
            var currentUserId = GetCurrentUserId(); // Méthode fictive pour récupérer l'ID de l'utilisateur connecté
            if (space.OwnerId != currentUserId)
            {
                return Forbid(); // L'utilisateur n'est pas autorisé à créer cet espace
            }

            // Enregistrez l'espace dans la base de données via le service
            await _spaceService.CreateSpaceAsync(space);

            // Retourner une réponse avec l'ID du nouvel espace créé
            return CreatedAtAction(nameof(GetSpace), new { id = space.Id }, space);
        }

        private long GetCurrentUserId()
        {
            // Cette méthode doit être implémentée pour récupérer l'ID de l'utilisateur connecté.
            // Par exemple, vous pouvez l'extraire des claims du jeton JWT.
            var userIdClaim = User?.FindFirst("UserId")?.Value;
            return userIdClaim != null ? long.Parse(userIdClaim) : 0;
        }
    }
}
