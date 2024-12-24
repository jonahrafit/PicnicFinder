using Microsoft.EntityFrameworkCore;

public class MenuService
{
    private readonly PicnicFinderContext _context;

    public MenuService(PicnicFinderContext context)
    {
        _context = context;
    }

    public async Task<List<Menu>> GetMenusForUserAsync(string[] userRoles)
    {
        var menus = await _context.Menus.Where(m => m.IsActive).ToListAsync();
        var accessibleMenus = new List<Menu>();

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
