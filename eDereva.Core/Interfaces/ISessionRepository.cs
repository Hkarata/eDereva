using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces;


public interface ISessionRepository
{
    Task<Session?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Session>> GetAllAsync(PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Session>> GetByVenueIdAsync(Guid venueId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<PaginatedResult<Session>> GetByContingencyIdAsync(Guid contingencyId, PaginationParams paginationParams, CancellationToken cancellationToken = default);
    Task<IEnumerable<Session>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
    Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Session> CreateAsync(Session session, CancellationToken cancellationToken = default);
    Task<Session> UpdateAsync(Session session, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, SessionStatus newStatus, CancellationToken cancellationToken = default);
    Task<bool> CheckSessionsByVenueAndDate(Guid pairVenueId, DateTime pairDate, CancellationToken cancellationToken);
}