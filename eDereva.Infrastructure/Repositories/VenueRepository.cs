using eDereva.Core.Contracts.Responses;
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

        public async Task<PaginatedResult<VenueDto>> GetAllVenuesAsync(PaginationParams pagination)
        {
            // Build the base query with pagination, excluding deleted venues
            var query = _context.Venues
                .AsNoTracking()
                .AsSplitQuery()
                .Where(v => !v.IsDeleted)
                .OrderBy(v => v.Name); // You can change the sorting criteria as needed

            // Get the total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var venues = await query
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .Include(v => v.District) // Include District (SplitQuery will handle this)
                .ThenInclude(d => d!.Region) // Include Region through District (SplitQuery will handle this)
                .AsSplitQuery() // Use SplitQuery to optimize loading related entities
                .Select(v => new VenueDto
                {
                    Id = v.Id,
                    Name = v.Name,
                    Address = v.Address,
                    Capacity = v.Capacity,
                    District = v.District!.Name, // District Name
                    Region = v.District!.Region!.Name, // Region Name, safely accessing with null conditional operator
                    ImageUrls = v.ImageUrls
                })
                .ToListAsync();

            // Return the paginated result
            var paginatedResult = new PaginatedResult<VenueDto>(venues, totalCount, pagination);

            return paginatedResult;
        }



        public async Task<PaginatedResult<VenueDto>> GetVenuesByDistrictIdAsync(Guid districtId, PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .AsSplitQuery()
                .Where(v => v.DistrictId == districtId && !v.IsDeleted);

            var count = await query.CountAsync();
            var items = await query
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var venues = items.Select(v => v.MapToVenueDto()).ToList();

            return new PaginatedResult<VenueDto>(venues, count, pagination);
        }

        public async Task<PaginatedResult<VenueDto>> GetVenuesWithSessionsAsync(PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .AsSplitQuery()
                .Where(v => !v.IsDeleted);

            var count = await query.CountAsync();
            var items = await query
                .Include(v => v.Sessions!.Where(s => !s.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var venues = items.Select(v => v.MapToVenueDto()).ToList();

            return new PaginatedResult<VenueDto>(venues, count, pagination);
        }

        public async Task<PaginatedResult<VenueDto>> GetAvailableVenuesAsync(
            DateTime startTime,
            DateTime endTime,
            PaginationParams pagination)
        {
            var query = _context.Venues
                .AsNoTracking()
                .AsSplitQuery()
                .Where(v => !v.IsDeleted &&
                           (!v.Sessions!.Any() || !v.Sessions!.Any(s =>
                               !s.IsDeleted &&
                               s.Status == Core.Enums.SessionStatus.Active &&
                               ((startTime >= s.StartTime && startTime < s.EndTime) ||
                                (endTime > s.StartTime && endTime <= s.EndTime) ||
                                (startTime <= s.StartTime && endTime >= s.EndTime)))));

            var count = await query.CountAsync();
            var items = await query
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Venue>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            var venues = items.Select(v => v.MapToVenueDto()).ToList();

            return new PaginatedResult<VenueDto>(venues, count, pagination);
        }
    }
}
