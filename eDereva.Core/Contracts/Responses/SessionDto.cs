using System.ComponentModel.DataAnnotations;
using eDereva.Core.Enums;

namespace eDereva.Core.Contracts.Responses
{
    public class SessionDto
    {
        public Guid Id { get; set; }

        public SessionStatus Status { get; set; } = SessionStatus.Active;
        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        public DateTime? InitiationTime { get; set; }
        public ContingencyType Contingency { get; set; } = ContingencyType.None; // Default is no contingency
        public DateTime? ContingencyTime { get; set; } // Time the contingency occurred, if applicable

        // If the contingency is "Other", a VenueManager must specify a reason
        [MaxLength(500)]
        public string? ContingencyExplanation { get; set; } // Explanation for "Other" contingencies
    }
}
