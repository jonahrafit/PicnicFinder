using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PicnicFinder.Models;

public class Report
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required]
    public DateTime Date { get; set; }  // Date du rapport

    public long TotalReservations { get; set; }  // Total des réservations pour le mois

    public long PopularSpaces { get; set; }  // Nombre d'espaces les plus réservés

    public long OwnerPerformance { get; set; }  // Performance des propriétaires
}