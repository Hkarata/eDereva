namespace eDereva.Core.Contracts.Requests;

public class ExemptVenueDto
{
    public Guid VenueId { get; set; }
    public DateTime ExemptionDate { get; set; }
    public string Reason { get; set; } = string.Empty;
}