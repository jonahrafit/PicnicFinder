using System.Diagnostics;
using System.Threading.Tasks;
using AdminBO.Models;
using AdminBO.Models.formBody;
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
    public async Task<ActionResult<IEnumerable<ViewReservation>>> Index(
        int page = 1,
        int pageSize = 3,
        int monthFilter = 0,
        int yearFilter = 2025,
        string status = "TOUS"
    )
    {
        long ownerId = GetCurrentUserId();
        ViewData["monthFilter"] = monthFilter;
        ViewData["statusFilter"] = status;
        ViewData["yearFilter"] = yearFilter;

        // Appel de la méthode GetAllReservationsByFilter avec pagination
        var allReservations = _reservationService.GetAllReservationsByFilter(
            ownerId,
            monthFilter,
            yearFilter,
            status
        );

        // Pagination des résultats
        var totalReservations = allReservations.Count;

        // Appliquer la pagination sur la liste des réservations
        var pagedReservations = allReservations.Skip((page - 1) * pageSize).Take(pageSize).ToList();

        // Créer le modèle de pagination
        var paginationModel = new PaginationModel
        {
            CurrentPage = page,
            TotalItems = totalReservations,
            ItemsPerPage = pageSize,
            TotalPages = (int)Math.Ceiling((double)totalReservations / pageSize),
        };

        // Résultat combiné
        var result = new { Reservations = pagedReservations, Pagination = paginationModel };

        // Passer l'ownerId à la vue
        ViewData["ownerId"] = ownerId;
        ViewData["totalReservations"] = totalReservations;
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

    [Authorize(Roles = "OWNER")]
    public IActionResult UpdateStatus([FromBody] ReservationStatusUpdateRequest request)
    {
        // Rechercher la réservation par ID et vérifier l'employeeId
        var reservation = _dbContext.Reservations.FirstOrDefault(r =>
            r.Id == request.ReservationId && r.EmployeeId == request.EmployeeId
        );
        if (reservation == null)
        {
            return NotFound(
                new { Message = "Réservation non trouvée ou l'employé ne correspond pas." }
            );
        }

        // Récupérer l'employé à partir de l'EmployeeId
        var employee = _dbContext.Users.FirstOrDefault(e => e.Id == request.EmployeeId);
        if (employee == null)
        {
            return NotFound(new { Message = "Employé non trouvé." });
        }

        // Convertir le statut en énumération
        if (!Enum.TryParse(typeof(ReservationStatus), request.Status, true, out var status))
        {
            return BadRequest(new { Message = $"Statut '{request.Status}' invalide." });
        }

        // Mettre à jour le statut de la réservation
        reservation.Status = (ReservationStatus)status;

        // Sauvegarder les modifications
        try
        {
            _dbContext.SaveChanges();

            // Si l'opération réussit, envoyer un e-mail à l'employé
            bool send_mail = EmailService.SendEmail(
                employee.Email, // Utilisez l'email de l'employé
                "Mise à jour de votre réservation",
                $"Votre réservation a été mise à jour avec le statut : {reservation.Status}. Merci pour votre patience."
            );

            if (!send_mail)
            {
                return StatusCode(500, new { Message = "Erreur lors de l'envoi de l'e-mail." });
            }

            return Ok(
                new
                {
                    Message = "Statut mis à jour avec succès et e-mail envoyé.",
                    Reservation = reservation,
                }
            );
        }
        catch (Exception ex)
        {
            return StatusCode(
                500,
                new { Message = "Erreur lors de la mise à jour.", Error = ex.Message }
            );
        }
    }

    [Authorize(Roles = "OWNER")]
    public async Task<ActionResult<IEnumerable<ViewReservation>>> AllReservationToPdf(
        int monthFilter = 0,
        int yearFilter = 2025,
        string status = "TOUS"
    )
    {
        long ownerId = GetCurrentUserId();

        var allReservations = _reservationService.GetAllReservationsByFilter(
            ownerId,
            monthFilter,
            yearFilter,
            status
        );

        return allReservations;
    }

    // ________________________________________________
    // ________________________________________________
    // ________________________________________________
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
