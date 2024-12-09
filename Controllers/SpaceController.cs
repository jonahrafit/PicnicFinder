using Microsoft.AspNetCore.Mvc;

namespace PicnicFinder.Controllers;

public class SpaceController : Controller
{
    private readonly PicnicFinderContext _context;

    // Constructeur avec injection de dépendances
    public SpaceController(PicnicFinderContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    // Exemple d'action
    public IActionResult GetAllSpaces()
    {
        var spaces = _context.Spaces.ToList();
        return Ok(spaces);
    }

}
