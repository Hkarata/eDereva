using eDereva.Core.Enums;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Session : ISoftDelete, IAudit
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public SessionStatus Status { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public int Capacity { get; set; }

    // Navigation properties
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }
    public Guid? ContingencyId { get; set; }
    public Contingency? Contingency { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
}