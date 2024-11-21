using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Services;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Services;

public class VenueExemptionService(
    ApplicationDbContext context,
    ILogger<VenueExemptionService> logger
) : IVenueExemptionService
{
    public async Task Exempt(ExemptVenueDto venue, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating venue exemption for venue {VenueId}", venue.VenueId);

        try
        {
            var venueExemption = new VenueExemption
            {
                VenueId = venue.VenueId,
                ExemptionDate = DateTime.Now
            };

            context.VenueExemptions.Add(venueExemption);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully created venue exemption for venue {VenueId}", venue.VenueId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to create venue exemption for venue {VenueId}", venue.VenueId);
            throw;
        }
    }

    public async Task ApproveExemption(Guid venueId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Approving exemption for venue {VenueId}", venueId);

        try
        {
            var venueExemption = await context.VenueExemptions
                .SingleOrDefaultAsync(x => x.VenueId == venueId, cancellationToken);

            if (venueExemption == null)
            {
                logger.LogWarning("No exemption found for venue {VenueId}", venueId);
                return;
            }

            venueExemption.HasBeenApproved = true;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully approved exemption for venue {VenueId}", venueId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to approve exemption for venue {VenueId}", venueId);
            throw;
        }
    }

    public async Task<List<Guid>> GetExemptedVenuesInDateRange(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Retrieving exempted venues between {StartDate} and {EndDate}", startDate, endDate);

        try
        {
            var venueIds = await context.VenueExemptions
                .AsNoTracking()
                .AsSplitQuery()
                .Where(v => v.ExemptionDate >= startDate &&
                            v.ExemptionDate <= endDate &&
                            v.HasBeenApproved)
                .Select(v => v.VenueId)
                .ToListAsync(cancellationToken);

            logger.LogInformation("Found {Count} exempted venues in date range", venueIds.Count);
            return venueIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve exempted venues between {StartDate} and {EndDate}", startDate,
                endDate);
            throw;
        }
    }

    public async Task<PaginatedResult<Core.Contracts.Responses.ExemptVenueDto>> GetNewExemptionRequests(
        PaginationParams paginationParams,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Retrieving new exemption requests for page {PageNumber}, size {PageSize}",
            paginationParams.PageNumber,
            paginationParams.PageSize
        );

        var query = context.VenueExemptions
            .Where(v => !v.HasBeenApproved && !v.HasBeenExempted)
            .OrderByDescending(v => v.ExemptionDate)
            .Join(
                context.Venues,
                exemption => exemption.VenueId,
                venue => venue.Id,
                (exemption, venue) => new { exemption, venue }
            );

        var totalCount = await query.CountAsync(cancellationToken);

        logger.LogDebug("Total count of unprocessed exemption requests: {TotalCount}", totalCount);

        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(v => new Core.Contracts.Responses.ExemptVenueDto
            {
                VenueId = v.exemption.VenueId,
                VenueName = v.venue.Name,
                ExemptionDate = v.exemption.ExemptionDate,
                Reason = v.exemption.Reason
            })
            .ToListAsync(cancellationToken);

        var result = new PaginatedResult<Core.Contracts.Responses.ExemptVenueDto>(items, totalCount, paginationParams);

        logger.LogInformation(
            "Retrieved {ItemCount} exemption requests (page {PageNumber} of {TotalPages})",
            items.Count,
            paginationParams.PageNumber,
            result.TotalPages
        );

        return result;
    }


    public async Task<bool> IsVenueExemptedForDate(Guid pairVenueId, DateTime pairDate,
        CancellationToken cancellationToken)
    {
        return await context.VenueExemptions
            .AsNoTracking()
            .Where(v => v.VenueId == pairVenueId && v.ExemptionDate == pairDate)
            .AnyAsync(cancellationToken);
    }
}