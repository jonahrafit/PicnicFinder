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
            Console.WriteLine("---------------1------------");
            Console.WriteLine(
                $"Reservation reçu : Id={reservation?.Id}, SpaceId={reservation?.SpaceId}, Dates={reservation?.StartDate} - {reservation?.EndDate}"
            );

            if (reservation == null)
            {
                return BadRequest("Les données de la réservation ne sont pas valides.");
            }

            reservation.EmployeeId = GetCurrentUserId();

            try
            {
                var currentUserId = GetCurrentUserId();
                Console.WriteLine($"Current User Id: {currentUserId}");

                var client = await _userService.GetUserByIdAsync(currentUserId);
                if (client == null)
                {
                    return Unauthorized("Utilisateur non trouvé.");
                }

                Console.WriteLine($"Client trouvé : {client}");

                var space = await _spaceService.GetSpaceByIdAsync(reservation.SpaceId);
                if (space == null)
                {
                    return NotFound("Espace non trouvé.");
                }

                Console.WriteLine($"Espace trouvé : {space}");
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
                Console.WriteLine($"Erreur lors de la création de la réservation : {ex.Message}");
                return StatusCode(500, "Une erreur interne est survenue.");
            }
        }

        private long GetCurrentUserId()
        {
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
            var totalReservations =
                await _reservationService.GetTotalReservationsCountAsyncByClient(
                    GetCurrentUserId()
                );

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

        [HttpGet("{month}/{year}")]
        [Authorize(Roles = "OWNER")]
        public async Task<
            ActionResult<IEnumerable<Reservation>>
        > GetApprovedReservationsByMonthAndYearAsync(int month, int year)
        {
            if (month < 1 || month > 12)
            {
                return BadRequest("Month must be between 1 and 12.");
            }
            var reservation = await _reservationService.GetApprovedReservationsByMonthAndYearAsync(
                month,
                year
            );
            return Ok(reservation);
        }

        [HttpGet("{day}/{month}/{year}/{spaceId}")]
        public async Task<ActionResult<bool>> GetApprovedReservationExistsAsync(
            int day,
            int month,
            int year,
            int spaceId
        )
        {
            // Validate month
            if (month < 1 || month > 12)
            {
                return BadRequest("Month must be between 1 and 12.");
            }

            // Call the service to check if any approved reservation exists
            var reservationExists = await _reservationService.CheckApprovedReservationExistsAsync(
                day,
                month,
                year,
                spaceId
            );

            // Return the result
            return Ok(reservationExists);
        }
    }
}
