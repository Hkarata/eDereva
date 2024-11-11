using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Venue
{
    public class GetAllVenuesEndpoint(IVenueRepository venueRepository, HybridCache hybridCache)
        : Endpoint<PaginationParams, Results<Ok<PaginatedResult<VenueDto>>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/venues");
            AllowAnonymous();
            Version(1);
            Description(options =>
            {
                options.WithTags("Venue")
                    .WithSummary("Retrieve all venues")
                    .WithDescription("Returns a list of all venues with pagination support");
            });
            Throttle(
                hitLimit: 15,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok<PaginatedResult<VenueDto>>, BadRequest>> ExecuteAsync(PaginationParams req, CancellationToken ct)
        {
            var cacheKey = $"venues:{req.PageNumber}:{req.PageSize}";

            var venues = await hybridCache.GetOrCreateAsync<PaginatedResult<VenueDto>>(cacheKey, async (cancellationToken) =>
            {
                return await venueRepository.GetAllVenuesAsync(req);
            }, null, null, ct);

            if (venues == null)
            {
                return TypedResults.BadRequest();
            }

            return TypedResults.Ok(venues);
        }
    }
}
