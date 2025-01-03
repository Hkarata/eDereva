using System;
using System.Threading;
using System.Threading.Tasks;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;
using Microsoft.Extensions.Logging;

namespace eDereva.Api.Endpoints.Session;

public class GetSessionEndpoint(
    ISessionRepository sessionRepository,
    HybridCache hybridCache,
    ILogger<GetSessionEndpoint> logger)
    : EndpointWithoutRequest<Results<Ok<SessionDto>, BadRequest>>
{
    public override void Configure()
    {
        Get("/sessions/{sessionId}");
        Version(1);
        Policies("RequireViewSessions");
        Description(options =>
        {
            options.WithTags("Session")
                .WithSummary("Get session by Id")
                .WithDescription("Get session by Id");
        });
    }

    public override async Task<Results<Ok<SessionDto>, BadRequest>> ExecuteAsync(CancellationToken ct)
    {
        var sessionId = Route<Guid>("sessionId");

        var cacheKey = $"session-{sessionId}";

        var session = await hybridCache.GetOrCreateAsync<SessionDto?>(cacheKey, async entry =>
        {
            logger.LogInformation("Cache miss for key: {CacheKey}. Fetching session from repository.", cacheKey);
            return await sessionRepository.GetByIdAsync(sessionId, entry);
        }, cancellationToken: ct);


        if (session is null)
        {
            logger.LogWarning("Session with ID: {SessionId} not found.", sessionId);
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Session with ID: {SessionId} fetched successfully.", sessionId);
        return TypedResults.Ok(session);
    }
}