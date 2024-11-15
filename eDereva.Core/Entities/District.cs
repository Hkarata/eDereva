using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities;

public class District
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Venue>? Venues { get; set; }
    
    public Guid? RegionId { get; set; }
    public Region? Region { get; set; }
}