using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Locale;

public class GetDistrictsEndpoint (HybridCache hybridCache, ILocaleRepository localeRepository) : Endpoint<Guid, Results<Ok<List<DistrictDto>>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/Districts/{regionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Locale")
                .WithSummary("Retrieves a list of districts for a particular region");
        });
    }

    public override async Task<Results<Ok<List<DistrictDto>>, BadRequest<string>>> ExecuteAsync(Guid req, CancellationToken ct)
    {
        var regionId = Route<Guid>("regionId");
        
        const string cacheKey = "Region-{regionId}-Districts";

        var districts = await hybridCache.GetOrCreateAsync<List<DistrictDto>>(cacheKey, async (entry) 
            => await localeRepository.GetAllDistricts(regionId, entry), cancellationToken: ct);
        
        return TypedResults.Ok(districts);
    }
}