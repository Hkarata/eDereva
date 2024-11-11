using System.ComponentModel.DataAnnotations;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class District : ISoftDelete
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Foreign Key to Region
    public Guid RegionId { get; set; }
    public Region? Region { get; set; }

    // Navigation property to related Venues
    public ICollection<Venue>? Venues { get; set; }
    public bool IsDeleted { get; set; }
}