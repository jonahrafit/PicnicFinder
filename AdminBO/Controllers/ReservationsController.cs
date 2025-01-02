using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AdminBO.Controllers;

public class ReservationsController : Controller
{
    private readonly IConfiguration _configuration;
    private readonly ReservationService _reservationService;
    private readonly AdminBOContext _dbContext;

    public ReservationsController(
        IConfiguration configuration,
        ILogger<HomeController> logger,
        AdminBOContext dbContext
    )
    {
        _configuration = configuration;
        _dbContext = dbContext;
        _reservationService = new ReservationService(_configuration, _dbContext);
    }

    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Index()
    {
        // var reservations = await _reservationService.GetReservationsAsync();
        // return View("Basic", reservations);
        return View("Basic");
    }

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

    // POST: Reservation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Create(
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            Reservation reservation
    )
    {
        if (ModelState.IsValid)
        {
            await _reservationService.CreateReservationAsync(reservation);
            return RedirectToAction(nameof(Index));
        }
        return View(reservation);
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

    // POST: Reservation/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Edit(
        long id,
        [Bind(
            "Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt"
        )]
            Reservation reservation
    )
    {
        if (id != reservation.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _reservationService.UpdateReservationAsync(reservation);
            }
            catch
            {
                if (!await _reservationService.ReservationExistsAsync(reservation.Id))
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

    // GET: Reservation/Delete/5
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

    // POST: Reservation/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
