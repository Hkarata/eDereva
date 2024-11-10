using System.ComponentModel.DataAnnotations;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Venue : ISoftDelete
{
    public int VenueId { get; set; }
    
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;
    
    public string Address { get; set; } = string.Empty;

    public List<string>? ImageUrls { get; set; }
    public int Capacity { get; set; }

    // Foreign Key to District
    public int DistrictId { get; set; }
    public District? District { get; set; }

    // Navigation property to related Sessions
    public ICollection<Session>? Sessions { get; set; }

    public bool IsDeleted { get; set; }
}