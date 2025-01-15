using System.Collections.Generic;
using System.Threading.Tasks;
using AdminBO.Models;
using Microsoft.EntityFrameworkCore;

namespace AdminBO.Services;

public class UserService
{
    private readonly AdminBOContext _context;
    private readonly IConfiguration _configuration;

    public UserService(IConfiguration configuration, AdminBOContext dbContext)
    {
        _configuration = configuration;
        _context = dbContext;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<User?> GetUserByIdAsync(long id)
    {
        return await _context.Users.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task CreateUserAsync(User user)
    {
        if (user == null)
        {
            throw new ArgumentNullException(nameof(user), "User cannot be null.");
        }
        try
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error occurred while creating user: {ex.Message}");
            throw;
        }
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(long id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> UserExistsAsync(long id)
    {
        return await _context.Users.AnyAsync(e => e.Id == id);
    }
}
