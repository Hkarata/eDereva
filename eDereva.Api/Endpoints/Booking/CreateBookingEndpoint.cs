using System.Threading;
using System.Threading.Tasks;
using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;

namespace eDereva.Api.Endpoints.Booking;

public class CreateBookingEndpoint(
    IBookingRepository bookingRepository,
    ILogger<CreateBookingEndpoint> logger) // Injected logger
    : Endpoint<BookingDto, Results<Ok, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/bookings");
        Version(1);
        Policies("RequireCreateBookings");
        Description(options =>
        {
            options.WithTags("Booking")
                .WithSummary("Creates a new booking")
                .WithDescription(
                    "This endpoint allows the creation of a new booking. The request should include booking details such as the user's NIN and session Id.");
        });
    }

    public override async Task<Results<Ok, BadRequest<string>>> ExecuteAsync(BookingDto req, CancellationToken ct)
    {
        logger.LogInformation("Creating a new booking for user: {Nin}", req.Nin); // Log information

        var result = await bookingRepository.CheckUsersBookingValidity(req.Nin, ct);

        if (!result)
        {
            logger.LogWarning("User {Nin} has a pending booking.", req.Nin); // Log warning
            return TypedResults.BadRequest("User has a pending booking");
        }

        var booking = req.MaptoBooking();
        await bookingRepository.AddBookingAsync(booking, ct);

        logger.LogInformation("Booking successfully created for user: {Nin}", req.Nin);

        return TypedResults.Ok();
    }
}