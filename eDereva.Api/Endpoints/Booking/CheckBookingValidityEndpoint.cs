using System.Threading;
using System.Threading.Tasks;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace eDereva.Api.Endpoints.Booking;

public class CheckBookingValidityEndpoint(
    IBookingRepository bookingRepository,
    ILogger<CheckBookingValidityEndpoint> logger)
    : EndpointWithoutRequest<Results<Ok, BadRequest>>
{
    public override void Configure()
    {
        Get("/booking/check-validity/{user-nin}");
        Version(1);
        Policies("RequireCreateBookings");
        Description(options =>
        {
            options.WithTags("Booking")
                .WithSummary("Check booking validity")
                .WithDescription("returns true if booking is valid, false otherwise");
        });
    }

    public override async Task<Results<Ok, BadRequest>> ExecuteAsync(CancellationToken ct)
    {
        var userNin = Route<string>("user-nin");

        if (string.IsNullOrEmpty(userNin))
        {
            logger.LogWarning("User NIN is null or empty.");
            return TypedResults.BadRequest();
        }

        logger.LogInformation("Checking booking validity for User NIN: {UserNin}", userNin);

        var result = await bookingRepository.CheckUsersBookingValidity(userNin, ct);

        if (result)
        {
            logger.LogInformation("Booking is valid for User NIN: {UserNin}", userNin);
            return TypedResults.Ok();
        }

        logger.LogWarning("Booking is not valid for User NIN: {UserNin}", userNin);
        return TypedResults.BadRequest();
    }
}