using Microsoft.EntityFrameworkCore;
using PicnicFinder.Models;

namespace BackOffice.Services;

public class MenuService
{   
    private readonly PicnicFinderContext _context;
    private readonly IConfiguration _configuration;

    public MenuService(IConfiguration configuration, PicnicFinderContext dbContext)
    {
        _configuration = configuration;
        _context = dbContext;
    }

    public async Task<List<Menu>> GetMenusForUserAsync(string[] userRoles)
    {
        // var menus = await _context.Menus.Where(m => m.IsActive).ToListAsync();
        List<Menu> menus = await _context.Menus.ToListAsync();
        Console.WriteLine(menus);
        Console.WriteLine(menus);
        List<Menu> accessibleMenus = new List<Menu>();
        
        foreach (var menu in menus)
        {
            var menuRoles = menu.Roles.Split(',').Select(r => r.Trim()).ToList();
            if (menuRoles.Any(role => userRoles.Contains(role)))
            {
                accessibleMenus.Add(menu);
            }
        }

        return accessibleMenus;
    }
}
