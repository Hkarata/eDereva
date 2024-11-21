using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Repositories;

public interface IVenueRepository
{
    Task<PaginatedResult<VenueDto>> GetVenuesPaginated(PaginationParams paginationParams,
        CancellationToken cancellationToken);

    Task<bool> AddVenue(Venue venue, CancellationToken cancellationToken);
    Task<Venue> UpdateVenue(Guid venueId, Venue venue, CancellationToken cancellationToken);
    Task DeleteVenue(Guid venueId, CancellationToken cancellationToken);
    Task UnDeleteVenue(Guid venueId, CancellationToken cancellationToken);

    Task<Venue?> GetVenueById(Guid venueId, CancellationToken cancellationToken);
    Task<List<Guid>> GetAllVenuesIds(CancellationToken cancellationToken);
}