using eDereva.Core.Interfaces;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Venue;

public class DeleteVenueEndpoint(IVenueRepository venueRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/venues/{venueId}");
        Version(1);
        Policies("RequireDeleteVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Deletes a specific venue")
                .WithDescription(
                    "Deletes an existing venue identified by its unique ID. This operation is irreversible.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");
        
        await venueRepository.DeleteVenue(venueId, ct);
        
        await SendOkAsync(ct);
    }
}
