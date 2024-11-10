namespace eDereva.Core.Entities;

public class VenueManager
{
    public Guid Id { get; set; }

    public ICollection<Venue>? Venues { get; set; }
}