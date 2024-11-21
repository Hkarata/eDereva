namespace eDereva.Core.Contracts.Responses;

public class ExemptVenueDto
{
    public Guid VenueId { get; set; }
    public string VenueName { get; set; } = string.Empty;
    public DateTime ExemptionDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}