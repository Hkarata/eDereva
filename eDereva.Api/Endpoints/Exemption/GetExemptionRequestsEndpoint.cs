using eDereva.Core.Contracts.Responses;
using eDereva.Core.Services;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Exemption;

public class GetExemptionRequestsEndpoint(
    IVenueExemptionService venueExemptionService,
    ILogger<GetExemptionRequestsEndpoint> logger)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<ExemptVenueDto>>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/exemption-requests");
        Version(1);
        Policies("RequireManageVenues");
        Description(options =>
        {
            options.WithTags("Exemption")
                .WithSummary("Retrieves a list of exemption requests")
                .WithDescription(
                    "Retrieves a list of exemption requests. The list is paginated. The page size is 10. The page number is 1 by default.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<ExemptVenueDto>>, BadRequest<string>>> ExecuteAsync(
        PaginationParams req, CancellationToken ct)
    {
        logger.LogInformation("Received request to get exemption requests with parameters: {PaginationParams}", req);

        var exemptionRequests = await venueExemptionService.GetNewExemptionRequests(req, ct);

        if (exemptionRequests.TotalCount == 0)
        {
            logger.LogWarning("No exemption requests found for the provided parameters: {PaginationParams}", req);
            return TypedResults.BadRequest("No exemption requests found");
        }

        logger.LogInformation("Exemption requests retrieved successfully. Total count: {TotalCount}",
            exemptionRequests.TotalCount);
        return TypedResults.Ok(exemptionRequests);
    }
}