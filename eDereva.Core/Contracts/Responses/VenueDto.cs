namespace eDereva.Core.Contracts.Responses;

public class VenueDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public string Address { get; set; } = string.Empty;

    public List<string>? ImageUrls { get; set; }
    public int Capacity { get; set; }
    public string District { get; set; } = string.Empty;
    public string? Region { get; set; } = string.Empty;
}