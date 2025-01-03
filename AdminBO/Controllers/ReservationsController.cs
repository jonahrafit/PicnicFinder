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

    public ReservationsController(
        IConfiguration configuration,
        ReservationService reservationService
    )
    {
        _configuration = configuration;
        _reservationService = reservationService;
    }

    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Index()
    {
        var reservations = await _reservationService.GetAllReservationsAsync();
        return View("Basic", reservations);
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

    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> Create(
        [Bind("EmployeeId,SpaceId,ReservationDate,StartDate,EndDate,Status")]
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
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "OWNER")]
    public async Task<IActionResult> DeleteConfirmed(long id)
    {
        await _reservationService.DeleteReservationAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
