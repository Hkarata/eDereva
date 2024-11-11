using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Venue
{
    public class GetVenueEndpoint(IVenueRepository venueRepository, HybridCache hybridCache)
        : EndpointWithoutRequest<VenueDto>
    {
        public override void Configure()
        {
            Get("venues/{venueId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("Venue")
                    .WithSummary("Get a specific venue")
                    .WithDescription("Retrieves details of a specific venue based on the provided identifier");
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var venueId = Route<Guid>("venueId");
            var cacheKey = $"venue_{venueId}";

            var venue = await hybridCache.GetOrCreateAsync<VenueDto>(
                cacheKey,
                async (cancellationToken) =>
                {
                    var result = await venueRepository.GetByIdAsync(venueId);
                    if (result == null)
                    {
                        return null!;
                    }
                    return result.MapToVenueDto();
                },
                cancellationToken: ct);

            if (venue == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await SendOkAsync(venue, ct);
            return;
        }
    }
}