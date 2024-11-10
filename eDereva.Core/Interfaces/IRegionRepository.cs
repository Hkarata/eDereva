using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IRegionRepository : IGenericRepository<Region>
    {
        Task<PaginatedResult<Region>> GetRegionsWithDistrictsAsync(PaginationParams pagination);
        Task<IEnumerable<Region>> GetAllRegionsLightAsync(); // For dropdowns/lists without related data
    }
}
