using eDereva.Core.Interfaces;

namespace eDereva.Core.Entities;

public class Question : IAudit, ISoftDelete
{
    public Guid Id { get; set; }
    public string Scenario { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public string QuestionText { get; set; } = string.Empty;

    // Navigation properties
    public ICollection<Choice>? Choices { get; set; }
    public Guid QuestionBankId { get; set; }
    public QuestionBank? QuestionBank { get; set; }
    public DateTime CreatedAt { get; } = DateTime.UtcNow;
    public DateTime? ModifiedAt { get; set; }
    public bool IsDeleted { get; set; }
}