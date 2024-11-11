using System.ComponentModel.DataAnnotations;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Enums;
using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Session : ISoftDelete
{
    public Guid Id { get; set; }

    public SessionStatus Status { get; set; } = SessionStatus.Active;
    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    public DateTime? InitiationTime { get; set; }

    // Contingency management
    public ContingencyType Contingency { get; set; } = ContingencyType.None; // Default is no contingency
    public DateTime? ContingencyTime { get; set; } // Time the contingency occurred, if applicable

    // If the contingency is "Other", a VenueManager must specify a reason
    [MaxLength(500)]
    public string? ContingencyExplanation { get; set; } // Explanation for "Other" contingencies

    // Foreign Key to Venue
    public Guid VenueId { get; set; }
    public Venue? Venue { get; set; }

    // Navigation property to related Bookings
    public ICollection<Booking>? Bookings { get; set; }

    public bool IsDeleted { get; set; }

    // Method to validate contingency
    public bool ValidateContingency()
    {
        // If the contingency is "Other", the venue manager must provide an explanation
        if (Contingency == ContingencyType.Other && string.IsNullOrEmpty(ContingencyExplanation))
        {
            throw new InvalidOperationException("A contingency explanation must be provided when the contingency type is 'Other'.");
        }
        return true;
    }
}

public static class SessionExtensions
{
    public static SessionDto MapToSessionDto(this Session session)
    {
        return new SessionDto
        {
            Id = session.Id,
            Status = session.Status,
            StartTime = session.StartTime,
            EndTime = session.EndTime,
            InitiationTime = session.InitiationTime,
            Contingency = session.Contingency,
            ContingencyTime = session.ContingencyTime,
            ContingencyExplanation = session.ContingencyExplanation
        };
    }
}
