using System;
using System.Threading;
using System.Threading.Tasks;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;

namespace eDereva.Api.Endpoints.Venue;

public class UnDeleteVenueEndpoint(IVenueRepository venueRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Put("/venues/{venueId}/undelete");
        Version(1);
        Policies("RequireManageVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Restores a deleted venue")
                .WithDescription(
                    "This endpoint allows users to restore a previously deleted venue identified by its unique ID. Restoring a venue makes it active again in the system, allowing it to be used for future operations such as booking or listing.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");

        await venueRepository.UnDeleteVenue(venueId, ct);

        await SendOkAsync(ct);
    }
}