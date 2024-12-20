using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Venue : IAudit
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public int Capacity { get; set; }
    public bool IsDeleted { get; set; }

    // Navigation properties
    public Guid DistrictId { get; set; }
    public District? District { get; set; }
    public ICollection<Session>? Sessions { get; set; }
    public ICollection<Contingency>? Contingencies { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow; // Set at creation
    public DateTime? ModifiedAt { get; set; }

    public void UpdateEntity()
    {
        ModifiedAt = DateTime.UtcNow; // Update modification time
    }
}

public static class VenueExtensions
{
    public static VenueDto MapToVenueDto(this Venue venue)
    {
        return new VenueDto
        {
            Id = venue.Id,
            Name = venue.Name,
            Address = venue.Address,
            ImageUrls = venue.ImageUrls,
            Capacity = venue.Capacity,
            District = venue.District?.Name ?? string.Empty,
            Region = venue.District?.Region?.Name ?? string.Empty
        };
    }
}