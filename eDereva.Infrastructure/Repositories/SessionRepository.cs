using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories
{
    public class SessionRepository(ApplicationDbContext context, ILogger<SessionRepository> logger)
        : ISessionRepository
    {
        public async Task<SessionDto?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching session with ID: {SessionId}", id);

            var session = await context.Sessions
                .AsNoTracking()
                .AsSplitQuery()
                .Where(s => s.Id == id && !s.IsDeleted)
                .Include(s => s.Venue)
                    .ThenInclude(v => v!.District)
                        .ThenInclude(d => d!.Region)
                .Include(s => s.Contingency)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Contingency = s.Contingency!.ContingencyType,
                    ContingencyExplanation = s.Contingency.ContingencyExplanation,
                    Venue = s.Venue!.Name,
                    District = s.Venue.District!.Name,
                    Region = s.Venue.District.Region!.Name
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (session == null)
            {
                logger.LogWarning("Session with ID: {SessionId} not found.", id);
            }
            else
            {
                logger.LogInformation("Session with ID: {SessionId} retrieved successfully.", id);
            }

            return session;
        }

        public async ValueTask<PaginatedResult<SessionDto>> GetAllAsync(PaginationParams paginationParams,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching all sessions with pagination: Page {PageNumber}, Size {PageSize}",
                paginationParams.PageNumber, paginationParams.PageSize);

            var query = context.Sessions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(s => s.Contingency)
                .Include(s => s.Venue)
                .ThenInclude(v => v!.District)
                .ThenInclude(d => d!.Region)
                .Where(s => !s.IsDeleted)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Contingency = s.Contingency!.ContingencyType,
                    ContingencyExplanation = s.Contingency.ContingencyExplanation,
                    Venue = s.Venue!.Name,
                    District = s.Venue.District!.Name,
                    Region = s.Venue.District.Region!.Name
                });

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Fetched {TotalCount} sessions.", totalCount);

            return new PaginatedResult<SessionDto>(items, totalCount, paginationParams);
        }

        public async Task<PaginatedResult<SessionDto>> GetByVenueIdAsync(
            Guid venueId, 
            PaginationParams paginationParams, 
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching sessions for Venue ID: {VenueId} with pagination: Page {PageNumber}, Size {PageSize}",
                venueId, paginationParams.PageNumber, paginationParams.PageSize);

            var query = context.Sessions
                .Include(s => s.Contingency)
                .Where(s => s.VenueId == venueId && !s.IsDeleted)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Contingency = s.Contingency!.ContingencyType,
                    ContingencyExplanation = s.Contingency.ContingencyExplanation
                });

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Fetched {TotalCount} sessions for Venue ID: {VenueId}.", totalCount, venueId);

            return new PaginatedResult<SessionDto>(items, totalCount, paginationParams);
        }

        public async Task<PaginatedResult<Session>> GetByContingencyIdAsync(
            Guid contingencyId, 
            PaginationParams paginationParams, 
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching sessions for Contingency ID: {ContingencyId} with pagination: Page {PageNumber}, Size {PageSize}",
                contingencyId, paginationParams.PageNumber, paginationParams.PageSize);

            var query = context.Sessions
                .Include(s => s.Venue)
                .Include(s => s.Contingency)
                .Where(s => s.ContingencyId == contingencyId && !s.IsDeleted)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime);

            var totalCount = await query.CountAsync(cancellationToken);
            var items = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Fetched {TotalCount} sessions for Contingency ID: {ContingencyId}.", totalCount, contingencyId);

            return new PaginatedResult<Session>(items, totalCount, paginationParams);
        }

        public async Task<PaginatedResult<SessionDto>> GetSessionsByDateRangeAsync(
            DateTime startDate, 
            DateTime endDate,
            PaginationParams paginationParams,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Fetching sessions between {StartDate} and {EndDate}.", startDate, endDate);

            var query = context.Sessions
                .AsNoTracking()
                .AsSplitQuery()
                .Include(s => s.Contingency)
                .Include(s => s.Venue)
                .ThenInclude(v => v!.District)
                .ThenInclude(d => d!.Region)
                .Where(s => s.Date >= startDate && s.Date <= endDate && !s.IsDeleted)
                .OrderBy(s => s.Date)
                .ThenBy(s => s.StartTime)
                .Select(s => new SessionDto
                {
                    Id = s.Id,
                    Date = s.Date,
                    StartTime = s.StartTime,
                    EndTime = s.EndTime,
                    Contingency = s.Contingency!.ContingencyType,
                    ContingencyExplanation = s.Contingency.ContingencyExplanation,
                    Venue = s.Venue!.Name,
                    District = s.Venue.District!.Name,
                    Region = s.Venue.District.Region!.Name
                });

            var totalCount = await query.CountAsync(cancellationToken);
            var sessions = await query
                .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
                .Take(paginationParams.PageSize)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Fetched {SessionCount} sessions between {StartDate} and {EndDate}.", sessions.Count, startDate, endDate);

            return new PaginatedResult<SessionDto>(sessions, totalCount, paginationParams);
        }

        public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Checking if session with ID: {SessionId} exists.", id);

            var exists = await context.Sessions
                .AnyAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);

            logger.LogInformation("Session with ID: {SessionId} exists: {Exists}.", id, exists);

            return exists;
        }

        public async Task<Session> CreateAsync(Session session, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Creating session with ID: {SessionId}.", session.Id);

            await context.Sessions.AddAsync(session, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Session with ID: {SessionId} created successfully.", session.Id);
            return session;
        }

        public async Task<Session> UpdateAsync(Session session, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Updating session with ID: {SessionId}.", session.Id);

            session.ModifiedAt = DateTime.UtcNow;
            context.Sessions.Update(session);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Session with ID: {SessionId} updated successfully.", session.Id);
            return session;
        }

        public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Soft deleting session with ID: {SessionId}.", id);

            var session = await context.Sessions.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);
            if (session == null)
            {
                logger.LogWarning("Session with ID: {SessionId} not found for deletion.", id);
                return false;
            }

            session.IsDeleted = true;
            session.ModifiedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Session with ID: {SessionId} soft deleted successfully.", id);
            return true;
        }

        public async Task<bool> UpdateStatusAsync(
            Guid id, 
            SessionStatus newStatus, 
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("Updating status for session with ID: {SessionId} to {NewStatus}.", id, newStatus);

            var session = await context.Sessions.FindAsync([id, cancellationToken], cancellationToken: cancellationToken);
            if (session == null)
            {
                logger.LogWarning("Session with ID: {SessionId} not found for status update.", id);
                return false;
            }

            session.Status = newStatus;
            session.ModifiedAt = DateTime.UtcNow;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Status for session with ID: {SessionId} updated to {NewStatus} successfully.", id, newStatus);
            return true;
        }

        public async Task<bool> CheckSessionsByVenueAndDate(Guid pairVenueId, DateTime pairDate, CancellationToken cancellationToken)
        {
            // Check if any session exists matching the given venue and date
            var exists = await context.Sessions
                .AnyAsync(s => s.VenueId == pairVenueId && s.Date == pairDate.Date, cancellationToken);

            return exists;
        }

    }
}
