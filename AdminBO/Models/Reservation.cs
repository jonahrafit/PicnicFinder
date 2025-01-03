using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AdminBO.Models;

namespace AdminBO.Models;

public enum ReservationStatus
{
    PENDING,
    CONFIRMED,
    CANCELLED,
}

public class Reservation
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    [ForeignKey("Employee")]
    public long EmployeeId { get; set; }
    public User? Employee { get; set; } // L'employé qui fait la réservation

    [Required]
    [ForeignKey("Space")]
    public long SpaceId { get; set; }
    public Space? Space { get; set; } // L'espace réservé

    [Required]
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow; // Date et heure de la réservation

    [Required]
    public DateTime StartDate { get; set; } // Date de début de la réservation

    [Required]
    public DateTime EndDate { get; set; } // Date de fin de la réservation

    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public ReservationStatus Status { get; set; } = ReservationStatus.PENDING; // Statut de la réservation

    // Surcharge de ToString()
    public override string ToString()
    {
        return $"Reservation [Id={Id}, EmployeeId={EmployeeId}, SpaceId={SpaceId}, "
            + $"ReservationDate={ReservationDate}, StartDate={StartDate}, "
            + $"EndDate={EndDate}, Status={Status}]";
    }
}
