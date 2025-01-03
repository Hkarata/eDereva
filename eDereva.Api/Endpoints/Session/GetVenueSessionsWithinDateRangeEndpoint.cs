using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Session;

public class GetVenueSessionsWithinDateRangeEndpoint(ISessionRepository sessionRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<SessionDto>>, BadRequest>>
{
    public override void Configure()
    {
        Post("/venue/{venueId}/sessions/within-date-range/{startDate}/{endDate}");
        Version(1);
        Policies("RequireViewSessions");
        Description(options =>
        {
            options.WithTags("Session")
                .WithSummary("Get sessions within date range");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<SessionDto>>, BadRequest>> ExecuteAsync(PaginationParams req,
        CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");
        var startDate = Route<DateTime>("startDate");
        var endDate = Route<DateTime>("endDate");
        var cacheKey = $"sessions-within-date-range-{startDate}-{endDate}";

        var sessions = await hybridCache.GetOrCreateAsync<PaginatedResult<SessionDto>>(cacheKey, async entry
                => await sessionRepository.GetVenueSessionsByDateRangeAsync(venueId, startDate, endDate, req, entry),
            cancellationToken: ct);

        if (sessions.TotalCount == 0) return TypedResults.BadRequest();

        return TypedResults.Ok(sessions);
    }
}