using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AdminBO.Models;

public enum UserRole
{
    ADMIN,
    OWNER,
    CLIENT,
}

public enum UserStatus
{
    PENDING_APPROVAL,
    APPROVED,
    REJECTED,
}

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [EmailAddress]
    public required string Email { get; set; }

    [Required]
    public required string Password { get; set; }

    [Required]
    public UserRole Role { get; set; }

    public required string Phone { get; set; }
    public required string Name { get; set; }

    [Required]
    public UserStatus Status { get; set; } = UserStatus.PENDING_APPROVAL;

    public override string ToString()
    {
        return $"User [Id={Id}, Email={Email}, Role={Role}, Phone={Phone}, Name={Name}, Status={Status}]";
    }
}
