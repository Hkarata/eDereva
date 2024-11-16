using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class VenueRepository(ApplicationDbContext context) : IVenueRepository
{
    public async Task<PaginatedResult<VenueDto>> GetVenuesPaginated(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        // Ensure pagination parameters are valid, defaulting if necessary
        paginationParams.PageNumber = paginationParams.PageNumber < 1 ? 1 : paginationParams.PageNumber;
        paginationParams.PageSize = paginationParams.PageSize < 1 ? 10 : paginationParams.PageSize;

        // Querying only active (non-deleted) venues
        var query = context.Venues
            .AsNoTracking()
            .Include(v => v.District)
            .ThenInclude(d => d!.Region)
            .Where(v => !v.IsDeleted);

        // Calculate the total number of venues (for pagination purposes)
        var totalCount = await query.CountAsync(cancellationToken);

        // Retrieve the paginated list of venues, ordered by name
        var venues = await query
            .OrderBy(v => v.Name)  // Ensure it's ordered consistently
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)  // Apply paging logic
            .Take(paginationParams.PageSize)  // Take only the required number of records
            .Select(v => new VenueDto
            {
                Id = v.Id,
                Name = v.Name,
                Address = v.Address,
                ImageUrls = v.ImageUrls,
                Capacity = v.Capacity,
                District = v.District!.Name,
                Region = v.District!.Region!.Name,
            })
            .ToListAsync(cancellationToken);  // Fetch the data asynchronously

        // Return the paginated result
        return new PaginatedResult<VenueDto>(venues, totalCount, paginationParams);
    }


    public async Task<bool> AddVenue(Venue venue, CancellationToken cancellationToken)
    {
        var venueExists = await IsVenueUniqueAsync(venue.Name, cancellationToken);

        if (venueExists)
            return false;
        
        context.Venues.Add(venue);
        await context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<Venue> UpdateVenue(Guid venueId, Venue updated, CancellationToken cancellationToken)
    {
        var venue = await GetVenueById(venueId, cancellationToken);
        
        if (venue == null)
            return null!;
        
        venue.Name = updated.Name;
        venue.Address = updated.Address;
        venue.ImageUrls = updated.ImageUrls;
        venue.Capacity = updated.Capacity;
        venue.DistrictId = updated.DistrictId;
        
        await context.SaveChangesAsync(cancellationToken);
        return venue;
    }

    public async Task DeleteVenue(Guid venueId, CancellationToken cancellationToken)
    {
        var venue = await GetVenueById(venueId, cancellationToken);
        if (venue != null) 
            venue.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UnDeleteVenue(Guid venueId, CancellationToken cancellationToken)
    {
        var venue = await GetVenueById(venueId, cancellationToken);
        if (venue != null) 
            venue.IsDeleted = true;
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<Venue?> GetVenueById(Guid venueId, CancellationToken cancellationToken)
    {
        return await context.Venues
            .Where(v => !v.IsDeleted && v.Id == venueId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    private async Task<bool> IsVenueUniqueAsync(string venueName, CancellationToken cancellationToken)
    {
        return await context.Venues
            .AnyAsync(v => v.Name == venueName, cancellationToken);
    }
}