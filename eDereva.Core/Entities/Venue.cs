using System.ComponentModel.DataAnnotations;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Venue : ISoftDelete
{
    public Guid Id { get; set; }

    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public List<string>? ImageUrls { get; set; }
    public int Capacity { get; set; }

    // Foreign Key to District
    public Guid DistrictId { get; set; }
    public District? District { get; set; }

    // Navigation property to related Sessions
    public ICollection<Session>? Sessions { get; set; }

    public bool IsDeleted { get; set; }
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