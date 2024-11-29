using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Repositories;

public interface IBookingRepository
{
    Task AddBookingAsync(Booking booking, CancellationToken cancellationToken);

    Task<PaginatedResult<BookingDto>> GetBookingsPaginatedAsync(Guid sessionId, PaginationParams paginationParams,
        CancellationToken cancellationToken);

    Task<bool> CheckUsersBookingValidity(string nin, CancellationToken cancellationToken);
}