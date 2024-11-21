using eDereva.Core.Enums;

namespace eDereva.Core.Contracts.Responses;

public class SessionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Active;
    public TimeOnly StartTime { get; set; }
    public TimeOnly EndTime { get; set; }
    public ContingencyType Contingency { get; set; } = ContingencyType.None; // Default is no contingency
    public string? ContingencyExplanation { get; set; } // Explanation for "Other" contingencies

    public string Venue { get; set; } = string.Empty;
    public string District { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
}