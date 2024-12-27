using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PicnicFinder.Models;

namespace PicnicFinder.Models;
public enum ReservationStatus
{
    PENDING,
    CONFIRMED,
    CANCELLED
}

public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public required User Employee { get; set; }  // L'employé qui fait la réservation

    [ForeignKey("Space")]
    public long SpaceId { get; set; }
    public required Space Space { get; set; }  // L'espace réservé

    [Required]
    public DateTime ReservationDate { get; set; }  // Date et heure de la réservation

    public DateTime StartDate { get; set; }  // Date de début de la réservation

    public DateTime EndDate { get; set; }  // Date de fin de la réservation

    [Required]
    public ReservationStatus Status { get; set; }  // Statut de la réservation
}
