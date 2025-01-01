using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services;

public class ReservationService
{
    private readonly IConfiguration _configuration;
    private readonly AdminBOContext _dbContext;

    public ReservationService(IConfiguration configuration, AdminBOContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    // Retourner tous les espaces
    public async Task<List<Reservation>> GetReservationsAsync()
    {
        return await _dbContext.Reservations // Sauter les éléments précédents
        .ToListAsync();
    } // Retourner tous les espaces

    // ___________________________
    // ___________________________
    // ___________________________
    public async Task<List<Reservation>> GetReservationsPagedAsync(int page, int pageSize)
    {
        return await _dbContext
            .Reservations.Skip((page - 1) * pageSize) // Sauter les éléments précédents
            .Take(pageSize) // Prendre seulement la page actuelle
            .ToListAsync();
    }

    public async Task<int> GetTotalReservationsCountAsync()
    {
        return await _dbContext.Reservations.CountAsync();
    }

    public async Task<List<Reservation>> GetAllReservationsAsyncByOwner(long employeeId)
    {
        Console.WriteLine(employeeId);
        try
        {
            if (_dbContext == null)
            {
                throw new NullReferenceException(
                    "Le contexte de la base de données (_dbContext) est null."
                );
            }

            if (_dbContext.Reservations == null)
            {
                throw new NullReferenceException("La table Reservation dans le contexte est null.");
            }

            // Récupérer les espaces associés au propriétaire spécifié
            return await _dbContext
                .Reservations.Where(s => s.EmployeeId == employeeId) // Utiliser l'ID du propriétaire
                .ToListAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");
            throw;
        }
    }

    // Retourner un espace par ID
    public async Task<Reservation?> GetReservationByIdAsync(long id)
    {
        return await _dbContext.Reservations.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Reservation?> GetReservationByIdAsyncByOwner(long id)
    {
        return await _dbContext.Reservations.FirstOrDefaultAsync(s => s.Id == id);
    }

    // Cr�er un nouvel espace
    public async Task CreateReservationAsync(Reservation space)
    {
        if (space == null)
        {
            throw new ArgumentNullException(nameof(space), "Reservation cannot be null.");
        }

        try
        {
            // Add the space to the context and save changes
            _dbContext.Reservations.Add(space);
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error occurred while creating space: {ex.Message}");
            // Rethrow the exception if needed
            throw;
        }
    }

    // Mettre � jour un espace
    public async Task UpdateReservationAsync(Reservation space)
    {
        _dbContext.Update(space);
        await _dbContext.SaveChangesAsync();
    }

    // Supprimer un espace
    public async Task DeleteReservationAsync(long id)
    {
        var space = await _dbContext.Reservations.FindAsync(id);
        if (space != null)
        {
            _dbContext.Reservations.Remove(space);
            await _dbContext.SaveChangesAsync();
        }
    }

    // V�rifier si un espace existe
    public async Task<bool> ReservationExistsAsync(long id)
    {
        return await _dbContext.Reservations.AnyAsync(e => e.Id == id);
    }
}
