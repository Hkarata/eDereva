using eDereva.Core.Contracts.Requests;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Exemption;

public class CreateVenueExemptionEndpoint(
    IVenueExemptionService venueExemptionService,
    ILogger<CreateVenueExemptionEndpoint> logger)
    : Endpoint<ExemptVenueDto, Results<Ok, BadRequest>>
{
    public override void Configure()
    {
        Post("/venue/exemptions");
        Version(1);
        Policies("RequireEditVenues");
        Description(options =>
        {
            options.WithTags("Exemption")
                .WithSummary("Create a new exemption for a venue")
                .WithDescription("Create a new exemption for a venue");
        });
    }

    public override async Task HandleAsync(ExemptVenueDto req, CancellationToken ct)
    {
        logger.LogInformation("Handling CreateVenueExemption request for venue {VenueId} on {ExemptionDate}",
            req.VenueId, req.ExemptionDate);

        await venueExemptionService.Exempt(req, ct);
        await SendOkAsync(ct);
        logger.LogInformation("Successfully created exemption for venue {VenueId}", req.VenueId);
    }
}