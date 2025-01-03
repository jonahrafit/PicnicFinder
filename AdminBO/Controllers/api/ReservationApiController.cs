using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt; // Pour 'JwtSecurityToken', 'JwtSecurityTokenHandler'
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization; // Pour 'AuthorizeAttribute'
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens; // Pour 'SymmetricSecurityKey', 'SigningCredentials', etc.

namespace AdminBO.Controllers.Api
{
    [Route("api/reservation")]
    [ApiController]
    public class ReservationApiController : Controller
    {
        private readonly UserService _userService;
        private readonly ReservationService _reservationService;
        private readonly SpaceService _spaceService;
        private readonly IConfiguration _configuration;
        private readonly AdminBOContext _dbContext;

        public ReservationApiController(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _dbContext = dbContext;
            _userService = new UserService(_configuration, _dbContext);
            _spaceService = new SpaceService(_configuration, _dbContext);
            _reservationService = new ReservationService(_configuration, _dbContext);
        }

        // POST: api/Reservation
        [HttpPost]
        [Authorize(Roles = "CLIENT")]
        public async Task<ActionResult<Reservation>> CreateReservation(
            [FromBody] Reservation reservation
        )
        {
            if (reservation == null)
            {
                return BadRequest("Les donn�es de l'ereservation ne sont pas valides.");
            }
            reservation.EmployeeId = GetCurrentUserId();
            Console.WriteLine("------------------------");
            try
            {
                var currentUserId = GetCurrentUserId();
                Console.WriteLine(currentUserId);
                var client = await _userService.GetUserByIdAsync(currentUserId);

                Console.WriteLine(client.ToString());
                if (client == null)
                {
                    return Unauthorized("Utilisateur non trouvé.");
                }

                var space = await _spaceService.GetSpaceByIdAsync(reservation.SpaceId);
                Console.WriteLine(space.ToString());
                reservation.Space = space;

                await _reservationService.CreateReservationAsync(reservation);
                return CreatedAtAction(
                    nameof(GetReservation),
                    new { id = reservation.Id },
                    reservation
                );
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la création de l'ereservation : {ex.Message}");
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

        // GET: api/Reservation
        [HttpGet]
        [Authorize(Roles = "CLIENT")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations(
            int page = 1,
            int pageSize = 10
        )
        {
            var reservations = await _reservationService.GetReservationsPagedAsyncByClient(
                page,
                pageSize,
                GetCurrentUserId()
            );

            if (reservations == null || reservations.Count == 0)
            {
                return NotFound("Aucun ereservation trouvé.");
            }

            // Nombre total d'ereservations
            var totalReservations = await _reservationService.GetTotalReservationsCountAsync();

            // Création d'un objet de pagination
            var paginationModel = new PaginationModel
            {
                CurrentPage = page,
                TotalItems = totalReservations,
                ItemsPerPage = pageSize,
                TotalPages = (int)Math.Ceiling((double)totalReservations / pageSize),
            };

            // Retourner les données paginées et la pagination dans les en-têtes ou le corps de la réponse
            var result = new { Reservations = reservations, Pagination = paginationModel };

            return Ok(result);
        }

        // ________________________________________________

        // GET: api/Reservation/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(long id)
        {
            var reservation = await _reservationService.GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return NotFound($"Ereservation avec l'ID {id} non trouv�.");
            }
            return Ok(reservation);
        }
    }
}
