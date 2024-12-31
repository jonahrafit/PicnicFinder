using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
namespace PicnicFinder.Models;
public class Favorite
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [ForeignKey("User")]
    [Required(ErrorMessage = "Le champ 'UserId' est requis.")]
    public long UserId { get; set; }

    public User? User { get; set; }

    [ForeignKey("Space")]
    [Required(ErrorMessage = "Le champ 'SpaceId' est requis.")]
    public long SpaceId { get; set; }

    public Space? Space { get; set; }

    [Required]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public override string ToString()
    {
        return $"Id: {Id}, UserId: {UserId}, SpaceId: {SpaceId}, CreatedAt: {CreatedAt}";
    }
}
