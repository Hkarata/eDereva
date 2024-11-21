using System.ComponentModel.DataAnnotations;
using eDereva.Core.Enums;

namespace eDereva.Core.Entities;

public class Contingency
{
    public Guid Id { get; set; }
    public DateTime ReportedAt { get; } = DateTime.UtcNow;
    public ContingencyType ContingencyType { get; set; }

    [MaxLength(500)] public string? ContingencyExplanation { get; set; }

    public ICollection<Session>? AffectedSessions { get; set; }
}