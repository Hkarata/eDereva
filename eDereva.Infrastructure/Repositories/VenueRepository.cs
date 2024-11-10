using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class VenueRepository : GenericRepository<Venue>, IVenueRepository
    {
        private readonly ApplicationDbContext _context;

        public VenueRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Venue>> GetAllVenuesLightAsync()
        {
            return await _context.Venues
                .AsNoTracking()
                .Where(v => !v.IsDeleted)
                .Select(v => new Venue
                {
                    VenueId = v.VenueId,
                    Name = v.Name,
                    Capacity = v.Capacity
                })
                .ToListAsync();
        }

        public async Task<PaginatedResult<Venue>> GetVenuesByDistrictIdAsync(int districtId, PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .Where(v => v.DistrictId == districtId && !v.IsDeleted);

            var count = await query.CountAsync();
            var items = await query
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Venue>(items, count, pagination);
        }

        public async Task<PaginatedResult<Venue>> GetVenuesWithSessionsAsync(PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .Where(v => !v.IsDeleted);

            var count = await query.CountAsync();
            var items = await query
                .Include(v => v.Sessions!.Where(s => !s.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Venue>(items, count, pagination);
        }

        public async Task<PaginatedResult<Venue>> GetAvailableVenuesAsync(
            DateTime startTime,
            DateTime endTime,
            PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .Where(v => !v.IsDeleted &&
                           (!v.Sessions!.Any() || !v.Sessions!.Any(s =>
                               !s.IsDeleted &&
                               s.Status == Core.Enums.SessionStatus.Active &&
                               ((startTime >= s.StartTime && startTime < s.EndTime) ||
                                (endTime > s.StartTime && endTime <= s.EndTime) ||
                                (startTime <= s.StartTime && endTime >= s.EndTime)))));

            var count = await query.CountAsync();
            var items = await query
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Venue>(items, count, pagination);
        }
    }
}
