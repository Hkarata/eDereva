using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IBookingRepository : IGenericRepository<Booking>
    {
        Task<PaginatedResult<Booking>> GetBookingsByUserIdAsync(Guid userId, PaginationParams pagination);
        Task<PaginatedResult<Booking>> GetBookingsBySessionIdAsync(Guid sessionId, PaginationParams pagination);
        Task<PaginatedResult<Booking>> GetBookingsAffectedByContingencyAsync(PaginationParams pagination);
        Task<int> GetBookingsCountBySessionIdAsync(Guid sessionId);
    }
}
