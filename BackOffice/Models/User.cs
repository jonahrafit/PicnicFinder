using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicnicFinder.Models;

public enum UserRole
{
    ADMIN,
    EMPLOYEE,
    OWNER
}

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public string Phone { get; set; }
    public string Name { get; set; }

    public ICollection<Space> Spaces { get; set; } // Espaces associés au propriétaire

    public User()
    {
        Spaces = [];
    }
}