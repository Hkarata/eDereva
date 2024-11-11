using eDereva.Core.Contracts.Requests;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.VenueManager
{
    public class ReportContingencyEndpoint(IVenueManagerRepository venueManagerRepository)
        : Endpoint<ReportContingencyRequest, Results<Ok, BadRequest>>
    {
        public override void Configure()
        {
            Post("/venue-manager/report-contingency/{sessionId}");
            AllowAnonymous();
            Version(1);
            Description(options =>
            {
                options.WithTags("Venue Manager");
                options.WithSummary("Report Session Contingency");
                options.WithDescription("Reports a contingency event for a specific session");
            });
            Throttle(
                hitLimit: 10,
                durationSeconds: 60,
                headerName: "X-Client-Id"
            );
        }

        public override async Task<Results<Ok, BadRequest>> ExecuteAsync(ReportContingencyRequest req, CancellationToken ct)
        {
            var sessionId = Route<Guid>("SessionId");
            if (sessionId == Guid.Empty)
            {
                return TypedResults.BadRequest();
            }

            var success = await venueManagerRepository.ReportContingency(
                sessionId,
                req.Type,
                req.ContingencyTime ?? DateTime.UtcNow,
                req.Description ?? string.Empty);

            return success ? TypedResults.Ok() : TypedResults.BadRequest();
        }
    }
}