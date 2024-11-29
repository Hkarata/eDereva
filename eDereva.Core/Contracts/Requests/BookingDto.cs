using eDereva.Core.Entities;

namespace eDereva.Core.Contracts.Requests;

public class BookingDto
{
    public string Nin { get; set; } = string.Empty;
    public Guid SessionId { get; set; }
}

public static class BookingExtensions
{
    public static Booking MaptoBooking(this BookingDto bookingDto)
    {
        return new Booking
        {
            Id = Guid.CreateVersion7(),
            UserNin = bookingDto.Nin,
            SessionId = bookingDto.SessionId
        };
    }
}