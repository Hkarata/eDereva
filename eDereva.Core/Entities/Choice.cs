using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Choice : IAudit
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;

    // Navigation properties
    public Guid QuestionId { get; set; }
    public Question? Question { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
}