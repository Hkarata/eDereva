using System.ComponentModel.DataAnnotations;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;


public class Region : ISoftDelete
{
    public Guid RegionId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    // Navigation property to related Districts
    public ICollection<District>? Districts { get; set; }

    public bool IsDeleted { get; set; }
}
