using eDereva.Core.Enums;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Booking : ISoftDelete
{
    public Guid Id { get; set; }

    // Foreign Key to User and Session
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public Guid SessionId { get; set; }
    public Session? Session { get; set; }

    public BookingStatus Status { get; set; } = BookingStatus.Pending; // Status of booking

    public DateTime? ExamStartTime { get; set; }
    public DateTime? ExamEndTime { get; set; }

    public bool IsDeleted { get; set; }

    // Check if booking is affected by a contingency
    public bool IsAffectedByContingency()
    {
        return Session?.Contingency != ContingencyType.None;
    }

}