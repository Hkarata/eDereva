using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities;

public class Region
{
    public Guid Id { get; set; }

    [MaxLength(100)] public string Name { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<District>? Districts { get; set; }
}