using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

namespace BackOffice.Services
{
    public class SpaceService
    {
        private readonly PicnicFinderContext _context;

        public SpaceService(PicnicFinderContext context)
        {
            _context = context;
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

        // Créer un nouvel espace
        public async Task CreateSpaceAsync(Space space)
        {
            space.CreatedAt = DateTime.UtcNow;
            space.UpdatedAt = DateTime.UtcNow;
            _context.Space.Add(space);
            await _context.SaveChangesAsync();
        }

        // Mettre à jour un espace
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

        // Vérifier si un espace existe
        public async Task<bool> SpaceExistsAsync(long id)
        {
            return await _context.Space.AnyAsync(e => e.Id == id);
        }
    }
}
