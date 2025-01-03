using System.Collections.Generic;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services
{
    public class ReservationService
    {
        private readonly IConfiguration _configuration;
        private readonly AdminBOContext _context;

        public ReservationService(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _context = dbContext;
        }

        // Create a new reservation
        public async Task<Reservation> CreateReservationAsync(Reservation reservation)
        {
            _context.Set<Reservation>().Add(reservation);
            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<List<Reservation>> GetReservationsPagedAsyncByClient(
            int page,
            int pageSize,
            long idUser
        )
        {
            return await _context
                .Reservations.Where(r => r.EmployeeId == idUser) // Filtrer par EmployeeId
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        //____________________________________________________

        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _context.Reservations.CountAsync();
        }

        // Retrieve all reservations
        public async Task<List<Reservation>> GetAllReservationsAsync()
        {
            return await _context.Set<Reservation>().Include(r => r.Employee).ToListAsync();
        }

        // Retrieve a single reservation by ID
        public async Task<Reservation?> GetReservationByIdAsync(long id)
        {
            return await _context
                .Set<Reservation>()
                .Include(r => r.Employee)
                .Include(r => r.Space)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        // Update an existing reservation
        public async Task<Reservation?> UpdateReservationAsync(
            long id,
            Reservation updatedReservation
        )
        {
            var existingReservation = await GetReservationByIdAsync(id);
            if (existingReservation == null)
            {
                return null;
            }

            existingReservation.EmployeeId = updatedReservation.EmployeeId;
            existingReservation.SpaceId = updatedReservation.SpaceId;
            existingReservation.ReservationDate = updatedReservation.ReservationDate;
            existingReservation.StartDate = updatedReservation.StartDate;
            existingReservation.EndDate = updatedReservation.EndDate;
            existingReservation.Status = updatedReservation.Status;

            _context.Set<Reservation>().Update(existingReservation);
            await _context.SaveChangesAsync();

            return existingReservation;
        }

        // Delete a reservation
        public async Task<bool> DeleteReservationAsync(long id)
        {
            var reservation = await GetReservationByIdAsync(id);
            if (reservation == null)
            {
                return false;
            }

            _context.Set<Reservation>().Remove(reservation);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
