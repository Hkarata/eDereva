using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class BookingRepository : GenericRepository<Booking>, IBookingRepository
    {
        private readonly ApplicationDbContext _context;

        public BookingRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Booking>> GetBookingsByUserIdAsync(Guid userId, PaginationParams pagination)
        {
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.UserId == userId && !b.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(b => b.Session)
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Booking>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Booking>(items, count, pagination);
        }

        public async Task<PaginatedResult<Booking>> GetBookingsBySessionIdAsync(Guid sessionId, PaginationParams pagination)
        {
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => b.SessionId == sessionId && !b.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(b => b.User)
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Booking>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Booking>(items, count, pagination);
        }

        public async Task<int> GetBookingsCountBySessionIdAsync(Guid sessionId)
        {
            return await _context.Bookings
                .CountAsync(b => b.SessionId == sessionId && !b.IsDeleted);
        }

        public async Task<PaginatedResult<Booking>> GetBookingsAffectedByContingencyAsync(PaginationParams pagination)
        {
            var query = _context.Bookings
                .AsNoTracking()
                .Where(b => !b.IsDeleted && b.Session != null &&
                            b.Session.Contingency != Core.Enums.ContingencyType.None);

            var count = await query.CountAsync();

            var items = await query
                .Include(b => b.Session)
                .Include(b => b.User)
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Booking>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Booking>(items, count, pagination);
        }
    }
}
