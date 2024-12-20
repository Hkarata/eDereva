using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Locale;

public class GetRegionsEndpoint(
    HybridCache hybridCache,
    ILogger<GetRegionsEndpoint> logger,
    ILocaleRepository localeRepository)
    : EndpointWithoutRequest<Results<Ok<List<RegionDto>>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/regions");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Locale")
                .WithSummary("Retrieves a list of regions");
        });
    }

    public override async Task<Results<Ok<List<RegionDto>>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        const string cacheKey = "regions";

        var regions = await hybridCache.GetOrCreateAsync<List<RegionDto>>(cacheKey, async entry =>
        {
            logger.LogInformation("Cache miss for key: {CacheKey}. Fetching regions from repository.", cacheKey);
            return (await localeRepository.GetAllRegions(entry))!;
        }, cancellationToken: ct);

        return TypedResults.Ok(regions);
    }
}