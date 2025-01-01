using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace AdminBO.Models;

public enum SpaceStatus
{
    PENDING,
    APPROVED,
    REJECTED,
}

public class Space
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [ForeignKey("Owner")]
    [Required(ErrorMessage = "Le champ 'OwnerId' est requis.")]
    public long OwnerId { get; set; }

    public User? Owner { get; set; } // Optionnel côté client, mais pourra être chargé via EF Core si nécessaire

    [Required(ErrorMessage = "Le champ 'Name' est requis.")]
    [MaxLength(100, ErrorMessage = "Le nom ne peut pas dépasser 100 caractères.")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Le champ 'Latitude' est requis.")]
    [Range(-90, 90, ErrorMessage = "La latitude doit être comprise entre -90 et 90.")]
    public double Latitude { get; set; }

    [Required(ErrorMessage = "Le champ 'Longitude' est requis.")]
    [Range(-180, 180, ErrorMessage = "La longitude doit être comprise entre -180 et 180.")]
    public double Longitude { get; set; }

    [Required(ErrorMessage = "Le champ 'Capacity' est requis.")]
    [Range(1, int.MaxValue, ErrorMessage = "La capacité doit être au moins de 1.")]
    public int Capacity { get; set; }

    [JsonPropertyName("photos")]
    public List<string> Photos { get; set; } = new List<string>();

    public string? Description { get; set; }

    [Required(ErrorMessage = "Le champ 'Status' est requis.")]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SpaceStatus Status { get; set; } = SpaceStatus.PENDING;

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Required]
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"Id: {Id}, "
            + $"OwnerId: {OwnerId}, "
            + $"Name: {Name}, "
            + $"Latitude: {Latitude}, "
            + $"Longitude: {Longitude}, "
            + $"Capacity: {Capacity}, "
            + $"Status: {Status}, "
            + $"CreatedAt: {CreatedAt}, "
            + $"UpdatedAt: {UpdatedAt}, "
            + $"Photos: [{string.Join(", ", Photos)}]"
            + (Owner != null ? $", Owner: [{Owner.ToString()}]" : ", Owner: null");
    }
}
