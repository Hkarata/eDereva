using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        Task<PaginatedResult<Session>> GetSessionsByVenueIdAsync(int venueId, PaginationParams pagination);
        Task<PaginatedResult<Session>> GetActiveSessionsAsync(PaginationParams pagination);
        Task<PaginatedResult<Session>> GetSessionsWithContingencyAsync(PaginationParams pagination);
        Task<bool> HasOverlappingSessionsAsync(int venueId, DateTime startTime, DateTime endTime, Guid? excludeSessionId = null);
    }
}
