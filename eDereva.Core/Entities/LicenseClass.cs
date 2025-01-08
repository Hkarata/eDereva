using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities;

public class LicenseClass
{
    public Guid Id { get; set; }

    [MaxLength(20)] public string Class { get; set; } = string.Empty;

    public ICollection<User>? Users { get; set; }
}