using System.ComponentModel.DataAnnotations;

public class SpaceFormDTO
{
    public required long OwnerId { get; set; }
    public required string Name { get; set; }

    // [Range(-90, 90, ErrorMessage = "Latitude doit être entre -90 et 90.")]
    public required double Latitude { get; set; }

    // [Range(-180, 180, ErrorMessage = "Longitude doit être entre -180 et 180.")]
    public required double Longitude { get; set; }

    public required int Capacity { get; set; }

    public List<IFormFile> Photos { get; set; } = new List<IFormFile>();
    public string? Description { get; set; }
    public required string ActivityIds { get; set; }
}
