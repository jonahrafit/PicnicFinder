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
        private readonly SpaceService _spaceService;

        public ReservationService(IConfiguration configuration, AdminBOContext dbContext)
        {
            _configuration = configuration;
            _context = dbContext;
            _spaceService = new SpaceService(configuration, dbContext);
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
                .Reservations.Where(r => r.EmployeeId == idUser)
                .Include(r => r.Space)
                .OrderBy(r => r.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsAsyncByOwner(long ownerId)
        {
            var spaces = await _spaceService.GetAllSpacesAsyncByOwner(ownerId);

            if (!spaces.Any())
            {
                Console.WriteLine("No spaces found for this owner.");
                return new List<Reservation>();
            }

            var reservationsQuery = _context.Reservations.Where(reservation =>
                spaces.Select(space => space.Id).Contains(reservation.SpaceId)
            );

            return await reservationsQuery.ToListAsync();
        }

        public async Task<List<Reservation>> GetReservationsPagedAsyncByOwner(
            int page,
            int pageSize,
            long ownerId
        )
        {
            if (page < 1)
                page = 1;
            if (pageSize < 1)
                pageSize = 10;

            // Attendre que la liste des réservations soit obtenue
            var reservations = await GetReservationsAsyncByOwner(ownerId);

            // Appliquer la pagination sur la liste obtenue
            var pagedReservations = reservations
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            Console.WriteLine(pagedReservations);
            return pagedReservations;
        }

        public async Task<int> GetTotalReservationsCountAsync()
        {
            return await _context.Reservations.CountAsync();
        }

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
