using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Booking;

public class GetSessionBookingsEndpoint(IBookingRepository bookingRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<BookingDto>>, BadRequest>>
{
    public override void Configure()
    {
        Get("/sessions/{sessionId}/bookings");
        Version(1);
        Policies("RequireViewBookings");
        Description(options =>
        {
            options.WithTags("Booking")
                .WithSummary("Retrieves a list of bookings")
                .WithDescription(
                    "Retrieves a list of bookings for a session. The list is paginated and can be filtered by date range, status and venue.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<BookingDto>>, BadRequest>> ExecuteAsync(PaginationParams req,
        CancellationToken ct)
    {
        var sessionId = Route<Guid>("sessionId");

        var cacheKey = $"SessionBookings-{sessionId}-{req.PageNumber}-{req.PageSize}";


        var sessions = await hybridCache.GetOrCreateAsync<PaginatedResult<BookingDto>>(cacheKey, async entry
            => await bookingRepository.GetBookingsPaginatedAsync(sessionId, req, entry), cancellationToken: ct);

        if (sessions.TotalCount == 0)
            return TypedResults.BadRequest();

        return TypedResults.Ok(sessions);
    }
}