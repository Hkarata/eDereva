using System.ComponentModel.DataAnnotations;

namespace eDereva.Core.Entities;

public class VenueManager
{
    [Key]
    public Guid UserId { get; set; }
    public ICollection<Venue>? Venues { get; set; }
}