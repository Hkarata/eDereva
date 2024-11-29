namespace eDereva.Core.Contracts.Responses;

public class BookingDto
{
    public Guid Id { get; set; }
    public DateTime BookedAt { get; set; }
    public string UsersName { get; set; } = string.Empty;
    public Guid SessionId { get; set; }
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public string VenueAddress { get; set; } = string.Empty;
    public string VenueName { get; set; } = string.Empty;
}