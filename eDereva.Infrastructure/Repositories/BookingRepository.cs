using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class BookingRepository(ApplicationDbContext context) : IBookingRepository
{
    public async Task AddBookingAsync(Booking booking, CancellationToken cancellationToken)
    {
        context.Bookings.Add(booking);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<PaginatedResult<BookingDto>> GetBookingsPaginatedAsync(Guid sessionId,
        PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        // Fetch bookings that match the sessionId and are not deleted
        var query = context.Bookings
            .AsNoTracking()
            .AsSplitQuery()
            .Include(b => b.User)
            .Include(b => b.Session)
            .ThenInclude(s => s!.Venue)
            .Where(b => b.SessionId == sessionId && !b.IsDeleted);

        // Determine the total count of bookings for pagination calculation
        var totalRecords = await query.CountAsync(cancellationToken);

        // Apply sorting based on PaginationParams
        if (!string.IsNullOrEmpty(paginationParams.SortBy))
            // Assuming sort is required on property names present in BookingDto
            query = paginationParams.IsDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, paginationParams.SortBy))
                : query.OrderBy(e => EF.Property<object>(e, paginationParams.SortBy));

        // Apply pagination
        var result = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        // Map to Booking Dto
        var bookings = result.Select(b => new BookingDto
        {
            Id = b.Id,
            BookedAt = b.CreatedAt, // Assuming BookedAt should hold CreatedAt value
            UsersName = b.User!.FullName(), // Assuming a mapping from UserNin
            SessionId = b.SessionId,
            // Assuming some logic to populate StartTime, EndTime, VenueAddress, VenueName
            StartTime = b.Session?.StartTime ?? default,
            EndTime = b.Session?.EndTime ?? default,
            VenueAddress = b.Session?.Venue?.Address ?? "",
            VenueName = b.Session?.Venue?.Name ?? ""
        });

        // Create and return a paginated result
        return new PaginatedResult<BookingDto>(bookings, totalRecords, paginationParams);
    }


    public async Task<bool> CheckUsersBookingValidity(string nin, CancellationToken cancellationToken)
    {
        // Find the most recent (or any existing) booking for the user
        var existingBooking = await context.Bookings
            .AsNoTracking()
            .Include(b => b.Session)
            .Where(b => b.UserNin == nin && !b.IsDeleted)
            .OrderByDescending(b => b.Session!.Date) // Assuming you want the most recent session
            .FirstOrDefaultAsync(cancellationToken);

        // If there is no booking, the user can book
        if (existingBooking == null) 
            return true;

        // Check if the session date of the existing booking has passed
        var sessionDate = existingBooking.Session!.Date;

        return sessionDate < DateTime.UtcNow;
    }
}