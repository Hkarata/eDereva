using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Exemption;

public class ApproveExemptionEndpoint(
    IVenueExemptionService venueExemptionService,
    ILogger<ApproveExemptionEndpoint> logger)
    : EndpointWithoutRequest<Ok>
{
    public override void Configure()
    {
        Get("/venue/{venueId}/approve-exemption");
        Version(1);
        Policies("RequireAdministrator");
        Description(options =>
        {
            options.WithTags("Exemption")
                .WithSummary("Approve an exemption")
                .WithDescription("Approve an exemption");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");

        logger.LogInformation("Received request to approve exemption for venue ID: {VenueId}", venueId);

        await venueExemptionService.ApproveExemption(venueId, ct);

        logger.LogInformation("Exemption approved for venue ID: {VenueId}", venueId);

        await SendOkAsync(ct);
    }
}