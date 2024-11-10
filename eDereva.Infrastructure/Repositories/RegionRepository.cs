using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class RegionRepository : GenericRepository<Region>, IRegionRepository
    {
        private readonly ApplicationDbContext _context;

        public RegionRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Region>> GetAllRegionsLightAsync()
        {
            return await _context.Regions
                .AsNoTracking()
                .Where(r => !r.IsDeleted)
                .Select(r => new Region
                {
                    RegionId = r.RegionId,
                    Name = r.Name,
                })
                .ToListAsync();
        }

        public async Task<PaginatedResult<Region>> GetRegionsWithDistrictsAsync(PaginationParams pagination)
        {
            var query = _context.Regions
                .AsNoTracking()
                .Where(r => !r.IsDeleted);

            var count = await query.CountAsync();

            var items = await query
                .Include(r => r.Districts!.Where(d => !d.IsDeleted))
                .AsSplitQuery()
                .OrderBy(SortPropertyHelper<Region>.GetSortProperty(pagination))
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PaginatedResult<Region>(items, count, pagination);
        }
    }
}