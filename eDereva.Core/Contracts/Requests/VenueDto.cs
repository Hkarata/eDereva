using eDereva.Core.Entities;

namespace eDereva.Core.Contracts.Requests;

public class VenueDto
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public int Capacity { get; set; }
    public Guid DistrictId { get; set; }
}

public static class VenueDtoExtensions
{
    public static Venue MapToVenue(this VenueDto dto)
    {
        return new Venue
        {
            Id = Guid.CreateVersion7(),
            Name = dto.Name,
            Address = dto.Address,
            ImageUrls = dto.ImageUrls,
            Capacity = dto.Capacity,
            DistrictId = dto.DistrictId,
        };
    }
}