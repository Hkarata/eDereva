using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class DistrictRepository : GenericRepository<District>, IDistrictRepository
    {
        private readonly ApplicationDbContext _context;

        public DistrictRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<District>> GetAllDistrictsLightAsync()
        {
            return await _context.Districts
                .AsNoTracking()
                .Where(d => !d.IsDeleted)
                .Select(d => new District
                {
                    DistrictId = d.DistrictId,
                    Name = d.Name,
                })
                .ToListAsync();
        }

        public async Task<PaginatedResult<District>> GetDistrictsByRegionIdAsync(Guid regionId, PaginationParams pagination)
        {
            var query = _context.Districts
                .AsNoTracking()
                .Where(d => d.RegionId == regionId && !d.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .OrderBy(SortPropertyHelper<District>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<District>(items, count, pagination);
        }

        public async Task<PaginatedResult<District>> GetDistrictsWithVenuesAsync(PaginationParams pagination)
        {
            var query = _context.Districts
                .AsNoTracking()
                .Where(d => !d.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(d => d.Venues!.Where(v => !v.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<District>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<District>(items, count, pagination);
        }
    }
}