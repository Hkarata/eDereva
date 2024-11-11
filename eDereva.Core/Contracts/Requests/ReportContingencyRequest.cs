using eDereva.Core.Enums;

namespace eDereva.Core.Contracts.Requests
{
    public class ReportContingencyRequest
    {
        public Guid SessionId { get; set; }
        public ContingencyType Type { get; set; }
        public DateTime? ContingencyTime { get; set; } // Time the contingency occurred, if applicable
        public string? Description { get; set; }
    }
}
