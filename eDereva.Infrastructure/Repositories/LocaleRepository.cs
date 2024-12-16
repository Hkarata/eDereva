using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class LocaleRepository(ApplicationDbContext context) : ILocaleRepository
{
    public async ValueTask<List<RegionDto>?> GetAllRegions(CancellationToken cancellationToken)
    {
        var regions = await context.Regions
            .Select(r => new RegionDto
            {
                Id = r.Id,
                Name = r.Name
            })
            .ToListAsync(cancellationToken: cancellationToken);

        return regions.Count == 0 ? null : regions;
    }

    public async ValueTask<List<DistrictDto>> GetAllDistricts(Guid? regionId, CancellationToken cancellationToken)
    {
        var districts = await context.Districts
            .Include(d => d.Region)
            .Where(d => d.RegionId == regionId)
            .Select(d => new DistrictDto
            {
                Id = d.Id,
                Name = d.Name
            })
            .ToListAsync(cancellationToken: cancellationToken);
        
        return districts;
    }
}