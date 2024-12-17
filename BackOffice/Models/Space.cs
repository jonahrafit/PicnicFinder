using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using PicnicFinder.Models;

public enum SpaceStatus
{
    PENDING,
    APPROVED,
    REJECTED
}

public class Space
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [ForeignKey("Owner")]
    public long OwnerId { get; set; }
    public required User Owner { get; set; }  // Propriétaire de l'espace

    [Required]
    public required string Name { get; set; }

    [Required]
    public double Latitude { get; set; }

    [Required]
    public double Longitude { get; set; }

    [Required]
    public int Capacity { get; set; }

    public ICollection<string> Photos { get; set; } = new List<string>();  // Liste des URL des photos

    public string? Description { get; set; }

    [Required]
    public SpaceStatus Status { get; set; }  // Statut de l'espace

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Space()
    {
        Photos = new List<string>();
    }
}
