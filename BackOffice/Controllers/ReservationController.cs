using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using PicnicFinder.Models;

namespace BackOffice.Controllers
{
    public class ReservationController : Controller
    {
        private readonly PicnicFinderContext _context;

        public ReservationController(PicnicFinderContext context)
        {
            _context = context;
        }

        // GET: Reservation
        [Authorize(Roles = "ADMIN,OWNER")]
        public async Task<IActionResult> Index()
        {
            var picnicFinderContext = _context.Reservations.Include(r => r.Employee).Include(r => r.Space);
            return View(await picnicFinderContext.ToListAsync());
        }

        // GET: Reservation/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Employee)
                .Include(r => r.Space)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // GET: Reservation/Create
        public IActionResult Create()
        {
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Email");
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Name");
            return View();
        }

        // POST: Reservation/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,EmployeeId,SpaceId,ReservationDate,StartDate,EndDate,Status")] Reservation reservation)
        {
            if (ModelState.IsValid)
            {
                _context.Add(reservation);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Email", reservation.EmployeeId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Name", reservation.SpaceId);
            return View(reservation);
        }

        // GET: Reservation/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null)
            {
                return NotFound();
            }
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Email", reservation.EmployeeId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Name", reservation.SpaceId);
            return View(reservation);
        }

        // POST: Reservation/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,EmployeeId,SpaceId,ReservationDate,StartDate,EndDate,Status")] Reservation reservation)
        {
            if (id != reservation.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reservation);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReservationExists(reservation.Id))
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
            ViewData["EmployeeId"] = new SelectList(_context.Users, "Id", "Email", reservation.EmployeeId);
            ViewData["SpaceId"] = new SelectList(_context.Space, "Id", "Name", reservation.SpaceId);
            return View(reservation);
        }

        // GET: Reservation/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var reservation = await _context.Reservations
                .Include(r => r.Employee)
                .Include(r => r.Space)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (reservation == null)
            {
                return NotFound();
            }

            return View(reservation);
        }

        // POST: Reservation/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation != null)
            {
                _context.Reservations.Remove(reservation);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReservationExists(long id)
        {
            return _context.Reservations.Any(e => e.Id == id);
        }
    }
}
