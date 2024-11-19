using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities;

public class VenueExemption
{
    [Key]
    public Guid VenueId { get; set; }
    public DateTime ExemptionDate { get; set; }
    
    [MaxLength(500)]
    public string Reason { get; set; } = string.Empty;
    
    public bool HasBeenApproved { get; set; } = false;
    public bool HasBeenExempted { get; set; } = false;
}