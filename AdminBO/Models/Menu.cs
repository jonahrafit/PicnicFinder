using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminBO.Models;

public class Menu
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required string Url { get; set; }
    public required string Roles { get; set; } // Liste des r�les s�par�s par des virgules
    public bool IsActive { get; set; }
}
