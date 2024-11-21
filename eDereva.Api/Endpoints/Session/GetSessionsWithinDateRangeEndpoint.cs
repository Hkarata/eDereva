using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Session;

public class GetSessionsWithinDateRangeEndpoint(ISessionRepository sessionRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<SessionDto>>, BadRequest>>
{
    public override void Configure()
    {
        Get("/sessions/within-date-range/{startDate}/{endDate}");
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
        var startDate = Route<DateTime>("startDate");
        var endDate = Route<DateTime>("endDate");
        var cacheKey = $"sessions-within-date-range-{startDate}-{endDate}";

        var sessions = await hybridCache.GetOrCreateAsync<PaginatedResult<SessionDto>>(cacheKey, async entry
                => await sessionRepository.GetSessionsByDateRangeAsync(startDate, endDate, req, entry),
            cancellationToken: ct);

        if (sessions.TotalCount == 0) return TypedResults.BadRequest();

        return TypedResults.Ok(sessions);
    }
}