using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Session;

public class GetSessionsEndpoint(ISessionRepository sessionRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<SessionDto>>, BadRequest>>
{
    public override void Configure()
    {
        Get("/sessions");
        Version(1);
        Policies("RequireViewSessions");
        Description(options =>
        {
            options.WithTags("Session")
                .WithSummary("Retrieves a paginated list of sessions")
                .WithDescription(
                    "This endpoint retrieves a paginated list of sessions based on the provided pagination parameters. It supports filtering and sorting where applicable.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<SessionDto>>, BadRequest>> ExecuteAsync(PaginationParams req,
        CancellationToken ct)
    {
        var cachekey = $"sessions-pageNumber-{req.PageNumber}-pageSize-{req.PageSize}";

        var sessions = await hybridCache.GetOrCreateAsync<PaginatedResult<SessionDto>>(cachekey, async _
            => await sessionRepository.GetAllAsync(req, ct), cancellationToken: ct);

        if (sessions.TotalCount == 0) return TypedResults.BadRequest();

        return TypedResults.Ok(sessions);
    }
}