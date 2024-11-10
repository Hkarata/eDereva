using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly ApplicationDbContext _context;

        public SessionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<PaginatedResult<Session>> GetSessionsByVenueIdAsync(int venueId, PaginationParams pagination)
        {
            var query = _context.Sessions
                .AsNoTracking()
                .Where(s => s.VenueId == venueId && !s.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(s => s.Bookings!.Where(b => !b.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Session>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Session>(items, count, pagination);
        }

        public async Task<bool> HasOverlappingSessionsAsync(int venueId, DateTime startTime, DateTime endTime, Guid? excludeSessionId = null)
        {
            return await _context.Sessions
                .AsNoTracking()
                .AnyAsync(s =>
                    s.VenueId == venueId &&
                    !s.IsDeleted &&
                    s.Id != excludeSessionId &&
                    s.Status == Core.Enums.SessionStatus.Active &&
                    ((startTime >= s.StartTime && startTime < s.EndTime) ||
                     (endTime > s.StartTime && endTime <= s.EndTime) ||
                     (startTime <= s.StartTime && endTime >= s.EndTime)));
        }

        public async Task<PaginatedResult<Session>> GetActiveSessionsAsync(PaginationParams pagination)
        {
            var query = _context.Sessions
                .AsNoTracking()
                .Where(s => s.Status == Core.Enums.SessionStatus.Active && !s.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(s => s.Venue)
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Session>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Session>(items, count, pagination);
        }

        public async Task<PaginatedResult<Session>> GetSessionsWithContingencyAsync(PaginationParams pagination)
        {
            var query = _context.Sessions
                .AsNoTracking()
                .Where(s => s.Contingency != Core.Enums.ContingencyType.None && !s.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(s => s.Venue)
                .Include(s => s.Bookings!.Where(b => !b.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Session>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Session>(items, count, pagination);
        }
    }
}
