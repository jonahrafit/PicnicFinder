using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

namespace BackOffice.Services;

public class SpaceService
{
    private readonly PicnicFinderContext _context;
    private readonly IConfiguration _configuration;

    public SpaceService(IConfiguration configuration, PicnicFinderContext dbContext)
    {
        _configuration = configuration;
        _context = dbContext;
    }

    // Retourner tous les espaces
    public async Task<List<Space>> GetAllSpacesAsync()
    {
        return await _context.Space.Include(s => s.Owner).ToListAsync();
    }

    // Retourner un espace par ID
    public async Task<Space?> GetSpaceByIdAsync(long id)
    {
        return await _context.Space.Include(s => s.Owner).FirstOrDefaultAsync(s => s.Id == id);
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
            _context.Space.Add(space);
            await _context.SaveChangesAsync();
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
        _context.Update(space);
        await _context.SaveChangesAsync();
    }

    // Supprimer un espace
    public async Task DeleteSpaceAsync(long id)
    {
        var space = await _context.Space.FindAsync(id);
        if (space != null)
        {
            _context.Space.Remove(space);
            await _context.SaveChangesAsync();
        }
    }

    // V�rifier si un espace existe
    public async Task<bool> SpaceExistsAsync(long id)
    {
        return await _context.Space.AnyAsync(e => e.Id == id);
    }

}
