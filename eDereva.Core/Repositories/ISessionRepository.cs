using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Repositories;

public interface ISessionRepository
{
    Task<SessionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    ValueTask<PaginatedResult<SessionDto>> GetAllAsync(PaginationParams paginationParams,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<SessionDto>> GetByVenueIdAsync(Guid venueId, PaginationParams paginationParams,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<Session>> GetByContingencyIdAsync(Guid contingencyId, PaginationParams paginationParams,
        CancellationToken cancellationToken = default);

    Task<PaginatedResult<SessionDto>> GetVenueSessionsByDateRangeAsync(
        Guid venueId, DateTime startDate, DateTime endDate, PaginationParams paginationParams,
        CancellationToken cancellationToken);

    Task<PaginatedResult<SessionDto>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate,
        PaginationParams paginationParams, CancellationToken cancellationToken = default);

    Task<Session> CreateAsync(Session session, CancellationToken cancellationToken = default);
    Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> UpdateStatusAsync(Guid id, SessionStatus newStatus, CancellationToken cancellationToken = default);
    Task<bool> CheckSessionsByVenueAndDate(Guid pairVenueId, DateTime pairDate, CancellationToken cancellationToken);
}