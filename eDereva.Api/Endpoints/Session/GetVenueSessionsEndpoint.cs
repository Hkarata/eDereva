using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Session;

public class GetVenueSessionsEndpoint(ISessionRepository sessionRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<SessionDto>>, BadRequest>>
{
    public override void Configure()
    {
        Post("/venue/{venueId}/sessions");
        Version(1);
        Policies("RequireViewSessions");
        Description(options =>
        {
            options.WithTags("Session")
                .WithSummary("Get venue sessions")
                .WithDescription("Fetches a paginated list of sessions for a specific venue.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<SessionDto>>, BadRequest>> ExecuteAsync(PaginationParams req,
        CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");

        var cacheKey = $"venue-{venueId}-sessions";

        var sessions = await hybridCache.GetOrCreateAsync<PaginatedResult<SessionDto>>(cacheKey, async entry
            => await sessionRepository.GetByVenueIdAsync(venueId, req, entry), cancellationToken: ct);

        if (sessions.TotalCount == 0) return TypedResults.BadRequest();

        return TypedResults.Ok(sessions);
    }
}