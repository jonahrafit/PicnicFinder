using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using AdminBO.Models;

namespace AdminBO.Models;

public class SpaceActivity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required(ErrorMessage = "Le champ 'Name' est requis.")]
    public string Name { get; set; } = string.Empty;

     // Relation avec CategoryActivity
    [ForeignKey("CategoryActivity")]
    [Required(ErrorMessage = "Le champ 'CategoryId' est requis.")]
    public long CategoryId { get; set; }

    public CategoryActivity? CategoryActivity { get; set; }

    // Ajoutez cette propriété pour la relation avec Space
    public ICollection<SpaceActivityLink> SpaceActivityLinks { get; set; }

    public override string ToString()
    {
        return $"ID: {Id}, Name: {Name}, CategoryId: {CategoryId}, "
            + $"Category: {CategoryActivity?.Name ?? "N/A"}";
    }
}
