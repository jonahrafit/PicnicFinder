using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

namespace BackOffice.Controllers
{
    public class SpaceController : Controller
    {
        private readonly PicnicFinderContext _context;

        public SpaceController(PicnicFinderContext context)
        {
            _context = context;
        }

        // GET: Space
        public async Task<IActionResult> Index()
        {
            ViewData["ActiveMenu"] = "GestionDesEspaces";
            var picnicFinderContext = _context.Space.Include(s => s.Owner);
            return View(await picnicFinderContext.ToListAsync());
        }

        // GET: Space/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space
                .Include(s => s.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        // GET: Space/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.Set<User>(), "Id", "Email");
            return View();
        }

        // POST: Space/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt")] Space space)
        {
            if (ModelState.IsValid)
            {
                _context.Add(space);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.Set<User>(), "Id", "Email", space.OwnerId);
            return View(space);
        }

        // GET: Space/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space.FindAsync(id);
            if (space == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.Set<User>(), "Id", "Email", space.OwnerId);
            return View(space);
        }

        // POST: Space/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("Id,OwnerId,Name,Latitude,Longitude,Capacity,Photos,Description,Status,CreatedAt,UpdatedAt")] Space space)
        {
            if (id != space.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(space);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SpaceExists(space.Id))
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
            ViewData["OwnerId"] = new SelectList(_context.Set<User>(), "Id", "Email", space.OwnerId);
            return View(space);
        }

        // GET: Space/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var space = await _context.Space
                .Include(s => s.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (space == null)
            {
                return NotFound();
            }

            return View(space);
        }

        // POST: Space/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var space = await _context.Space.FindAsync(id);
            if (space != null)
            {
                _context.Space.Remove(space);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SpaceExists(long id)
        {
            return _context.Space.Any(e => e.Id == id);
        }
    }
}
