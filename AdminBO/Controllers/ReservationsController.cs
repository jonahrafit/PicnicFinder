using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class ReservationsController : BaseController
{
    private readonly IConfiguration _configuration;
    private readonly ReservationService _reservationService;
    private readonly AdminBOContext _dbContext;

    public ReservationsController(
        ILogger<ReservationsController> logger,
        IConfiguration configuration,
        AdminBOContext dbContext
    )
        : base(logger, configuration)
    {
        _dbContext = dbContext;
        _reservationService = new ReservationService(configuration, dbContext);
    }

    [Authorize(Roles = "OWNER")]
    public async Task<ActionResult<IEnumerable<Reservation>>> Index(int page = 1, int pageSize = 10)
    {
        Console.WriteLine("-------------------'--------------------");
        long ownerId = GetCurrentUserId();
        var reservations = await _reservationService.GetReservationsPagedAsyncByOwner(
            page,
            pageSize,
            ownerId
        );

        foreach (var reserv in reservations)
        {
            Console.WriteLine(reserv.ToString());
        }

        if (reservations == null || reservations.Count == 0)
        {
            return NotFound("Aucun reservation trouvé.");
        }

        // Récupère la liste des réservations
        var all_reservations = await _reservationService.GetReservationsAsyncByOwner(ownerId);

        // Si aucune réservation n'est trouvée
        if (all_reservations == null || all_reservations.Count == 0)
        {
            return NotFound("Aucune réservation trouvée.");
        }

        var totalReservations = reservations.Count;

        var paginationModel = new PaginationModel
        {
            CurrentPage = page,
            TotalItems = totalReservations,
            ItemsPerPage = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalReservations / pageSize),
        };

        Console.WriteLine(paginationModel.ToString());
        var result = new { Reservations = reservations, Pagination = paginationModel };

        return View("Basic", result);
    }

    [Authorize(Roles = "OWNER")]
    public IActionResult GetReservationsData(string? year)
    {
        var data = _reservationService.GetReservationsByYearOrLast12Months(year);
        return Json(data);
    }

    [Authorize(Roles = "OWNER")]
    public async Task<ActionResult<double>> GetReservationGrowth(string year)
    {
        double growth = await _reservationService.CalculateReservationGrowthAsync(year);
        return Ok(growth);
    }

    // ________________________________________________
    [Authorize(Roles = "OWNER")]
    // GET: Reservation/Details/5
    public async Task<IActionResult> Details(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _reservationService.GetReservationByIdAsync(id.Value);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    // GET: Reservation/Create
    [Authorize(Roles = "OWNER")]
    public IActionResult Create()
    {
        return View();
    }

    // GET: Reservation/Edit/5
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _reservationService.GetReservationByIdAsync(id.Value);
        if (reservation == null)
        {
            return NotFound();
        }
        return View(reservation);
    }

    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(long id, Reservation reservation)
    {
        if (id != reservation.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _reservationService.UpdateReservationAsync(id, reservation);
            }
            catch
            {
                if (await _reservationService.GetReservationByIdAsync(reservation.Id) == null)
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
        return View(reservation);
    }

    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Delete(long? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var reservation = await _reservationService.GetReservationByIdAsync(id.Value);
        if (reservation == null)
        {
            return NotFound();
        }

        return View(reservation);
    }

    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
