using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories;

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
            logger.LogWarning("Session with ID: {SessionId} not found.", id);
        else
            logger.LogInformation("Session with ID: {SessionId} retrieved successfully.", id);

        return session;
    }

    public async ValueTask<PaginatedResult<SessionDto>> GetAllAsync(PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Fetching all sessions with pagination: Page {PageNumber}, Size {PageSize}",
            paginationParams.PageNumber, paginationParams.PageSize);

        var query = context.Sessions
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.Date >= DateTime.UtcNow)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Select(s => new
            {
                s.Id,
                s.Date,
                s.StartTime,
                s.EndTime,
                ContingencyType = s.Contingency != null ? s.Contingency.ContingencyType : ContingencyType.None,
                ContingencyExplanation =
                    s.Contingency != null ? s.Contingency.ContingencyExplanation : "No explanation",
                VenueName = s.Venue != null ? s.Venue.Name : "Unknown",
                DistrictName = s.Venue != null && s.Venue.District != null ? s.Venue.District.Name : "Unknown",
                RegionName = s.Venue != null && s.Venue.District != null && s.Venue.District.Region != null
                    ? s.Venue.District.Region.Name
                    : "Unknown"
            });

        // Perform the operations sequentially on the DbContext to avoid concurrency issues
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        var result = items.Select(s => new SessionDto
        {
            Id = s.Id,
            Date = s.Date,
            StartTime = s.StartTime,
            EndTime = s.EndTime,
            Contingency = s.ContingencyType,
            ContingencyExplanation = s.ContingencyExplanation,
            Venue = s.VenueName,
            District = s.DistrictName,
            Region = s.RegionName
        }).ToList();

        logger.LogInformation("Fetched {TotalCount} sessions.", totalCount);

        return new PaginatedResult<SessionDto>(result, totalCount, paginationParams);
    }

    public async Task<PaginatedResult<SessionDto>> GetByVenueIdAsync(
        Guid venueId,
        PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation(
            "Fetching sessions for venue {VenueId} with pagination: Page {PageNumber}, Size {PageSize}",
            venueId, paginationParams.PageNumber, paginationParams.PageSize);

        // Calculate total count separately for better performance
        var totalCount = await context.Sessions
            .Where(s => s.VenueId == venueId)
            .CountAsync(cancellationToken);

        var utcNow = DateTime.UtcNow;

        var sessions = await context.Sessions
            .AsNoTracking()
            .Where(s => s.VenueId == venueId)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Venue = s.Venue!.Name,
                Status = s.Contingency != null
                    ? SessionStatus.Canceled
                    : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= utcNow
                      && utcNow <= new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second)
                        ? SessionStatus.Active
                        : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                            s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) < utcNow
                            ? SessionStatus.Completed
                            : SessionStatus.Scheduled,
                SessionCapacity = s.Venue!.Capacity - s.Capacity,
                District = s.Venue.District != null ? s.Venue.District.Name : "Unknown",
                Region = s.Venue.District!.Region != null ? s.Venue.District.Region.Name : "Unknown",
                Contingency = s.Contingency != null ? s.Contingency.ContingencyType : ContingencyType.None,
                ContingencyExplanation = s.Contingency != null
                    ? s.Contingency.ContingencyExplanation ?? "No explanation provided"
                    : string.Empty
            })
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<SessionDto>(sessions, totalCount, paginationParams);
    }

    public Task<PaginatedResult<Session>> GetByContingencyIdAsync(Guid contingencyId, PaginationParams paginationParams,
        CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<PaginatedResult<SessionDto>> GetVenueSessionsByDateRangeAsync(Guid venueId, DateTime startDate,
        DateTime endDate,
        PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var sessions = await context.Sessions
            .AsNoTracking()
            .AsSplitQuery()
            .Where(s => s.VenueId == venueId && startDate <= s.Date && s.Date <= endDate)
            .Include(s => s.Contingency)
            .Include(s => s.Venue)
            .ThenInclude(s => s!.District)
            .ThenInclude(s => s!.Region)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Venue = s.Venue!.Name,
                Status = s.Contingency != null
                    ? SessionStatus.Canceled
                    : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= utcNow
                      && utcNow <= new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second)
                        ? SessionStatus.Active
                        : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                            s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) < utcNow
                            ? SessionStatus.Completed
                            : SessionStatus.Scheduled,
                SessionCapacity = s.Venue!.Capacity - s.Capacity,
                District = s.Venue.District != null ? s.Venue.District.Name : "Unknown",
                Region = s.Venue.District!.Region != null ? s.Venue.District.Region.Name : "Unknown",
                Contingency = s.Contingency != null ? s.Contingency.ContingencyType : ContingencyType.None,
                ContingencyExplanation = s.Contingency != null
                    ? s.Contingency.ContingencyExplanation ?? "No explanation provided"
                    : string.Empty
            })
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize) // Skip records of previous pages
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedResult<SessionDto>(sessions, sessions.Count, paginationParams);
    }

    public async Task<PaginatedResult<SessionDto>> GetSessionsByDateRangeAsync(DateTime startDate, DateTime endDate,
        PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        var utcNow = DateTime.UtcNow;

        var sessions = await context.Sessions
            .AsNoTracking()
            .AsSplitQuery()
            .Where(s => startDate <= s.Date && s.Date <= endDate)
            .Include(s => s.Contingency)
            .Include(s => s.Venue)
            .ThenInclude(v => v!.District)
            .ThenInclude(d => d!.Region)
            .OrderBy(s => s.Date)
            .ThenBy(s => s.StartTime)
            .Select(s => new SessionDto
            {
                Id = s.Id,
                Date = s.Date,
                StartTime = s.StartTime,
                EndTime = s.EndTime,
                Venue = s.Venue!.Name,
                Status = s.Contingency != null
                    ? SessionStatus.Canceled
                    : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.StartTime.Hour, s.StartTime.Minute, s.StartTime.Second) <= utcNow
                      && utcNow <= new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                          s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second)
                        ? SessionStatus.Active
                        : new DateTime(s.Date.Year, s.Date.Month, s.Date.Day,
                            s.EndTime.Hour, s.EndTime.Minute, s.EndTime.Second) < utcNow
                            ? SessionStatus.Completed
                            : SessionStatus.Scheduled,
                SessionCapacity = s.Venue!.Capacity - s.Capacity,
                District = s.Venue.District != null ? s.Venue.District.Name : "Unknown",
                Region = s.Venue.District!.Region != null ? s.Venue.District.Region.Name : "Unknown",
                Contingency = s.Contingency != null ? s.Contingency.ContingencyType : ContingencyType.None,
                ContingencyExplanation = s.Contingency != null
                    ? s.Contingency.ContingencyExplanation ?? "No explanation provided"
                    : string.Empty
            })
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize) // Skip records for previous pages
            .Take(paginationParams.PageSize) // Take the requested number of records
            .ToListAsync(cancellationToken);


        return new PaginatedResult<SessionDto>(sessions, sessions.Count, paginationParams);
    }

    public async Task<Session> CreateAsync(Session session, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Creating session with ID: {SessionId}.", session.Id);

        await context.Sessions.AddAsync(session, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Session with ID: {SessionId} created successfully.", session.Id);
        return session;
    }

    public async Task<bool> SoftDeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Soft deleting session with ID: {SessionId}.", id);

        var session = await context.Sessions.FindAsync([id], cancellationToken);
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

    public async Task<bool> UpdateStatusAsync(Guid id, SessionStatus newStatus,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Updating status for session with ID: {SessionId} to {NewStatus}.", id, newStatus);

        var session = await context.Sessions.FindAsync([id], cancellationToken);
        if (session == null)
        {
            logger.LogWarning("Session with ID: {SessionId} not found for status update.", id);
            return false;
        }

        session.Status = newStatus;
        session.ModifiedAt = DateTime.UtcNow;
        await context.SaveChangesAsync(cancellationToken);

        logger.LogInformation("Status for session with ID: {SessionId} updated to {NewStatus} successfully.", id,
            newStatus);
        return true;
    }

    public Task<bool> CheckSessionsByVenueAndDate(Guid pairVenueId, DateTime pairDate,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Checking if session with ID: {SessionId} exists.", id);

        return await context.Sessions
            .AsNoTracking()
            .AnyAsync(s => s.Id == id && !s.IsDeleted, cancellationToken);
    }
}