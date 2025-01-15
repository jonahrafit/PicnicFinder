using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services;

public class SpaceService
{
    private readonly IConfiguration _configuration;
    private readonly AdminBOContext _dbContext;

    public SpaceService(IConfiguration configuration, AdminBOContext dbContext)
    {
        _configuration = configuration;
        _dbContext = dbContext;
    }

    // Retourner tous les espaces
    public async Task<List<Space>> GetSpacesPagedAsync(int page, int pageSize)
    {
        return await _dbContext
            .Spaces.Skip((page - 1) * pageSize)
            .Take(pageSize) // Prendre seulement la page actuelle
            .ToListAsync();
    }

    public async Task<int> GetTotalSpacesCountAsync()
    {
        return await _dbContext.Spaces.CountAsync();
    }

    public async Task<List<Space>> GetAllSpacesAsyncByOwner(long ownerId)
    {
        Console.WriteLine(ownerId);
        try
        {
            if (_dbContext == null)
            {
                throw new NullReferenceException(
                    "Le contexte de la base de données (_dbContext) est null."
                );
            }

            if (_dbContext.Spaces == null)
            {
                throw new NullReferenceException("La table Space dans le contexte est null.");
            }

            // Récupérer les espaces associés au propriétaire spécifié
            return await _dbContext
                .Spaces.Where(s => s.OwnerId == ownerId) // Utiliser l'ID du propriétaire
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
    public async Task<Space?> GetSpaceByIdAsync(long id)
    {
        return await _dbContext.Spaces.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Space?> GetSpaceByIdAsyncByOwner(long id)
    {
        return await _dbContext.Spaces.FirstOrDefaultAsync(s => s.Id == id);
    }

    // Cr�er un nouvel espace
    public async Task CreateSpaceAsync(Space space)
    {
        if (space == null)
        {
            throw new ArgumentNullException(nameof(space), "Space cannot be null.");
        }

        try
        {
            // Set the timestamps
            space.CreatedAt = DateTime.UtcNow;
            space.UpdatedAt = DateTime.UtcNow;

            // Add the space to the context and save changes
            _dbContext.Spaces.Add(space);
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
    public async Task UpdateSpaceAsync(Space space)
    {
        _dbContext.Update(space);
        await _dbContext.SaveChangesAsync();
    }

    // Supprimer un espace
    public async Task DeleteSpaceAsync(long id)
    {
        var space = await _dbContext.Spaces.FindAsync(id);
        if (space != null)
        {
            _dbContext.Spaces.Remove(space);
            await _dbContext.SaveChangesAsync();
        }
    }

    // V�rifier si un espace existe
    public async Task<bool> SpaceExistsAsync(long id)
    {
        return await _dbContext.Spaces.AnyAsync(e => e.Id == id);
    }
}
