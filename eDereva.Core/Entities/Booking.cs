using eDereva.Core.Enums;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Booking : IAudit, ISoftDelete
{
    public Guid Id { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending;

    // Navigation properties
    public Guid SessionId { get; set; }
    public Session? Session { get; set; }
    public string UserNin { get; set; } = string.Empty;
    public User? User { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
}