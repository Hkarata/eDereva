namespace eDereva.Core.Entities;

public class Question
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public string Scenario { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public ICollection<Option>? Options { get; set; }

    // Foreign Key to Section Template
    public Guid SectionTemplateId { get; set; }

    // Navigation Properties
    public SectionTemplate SectionTemplate { get; set; } = default!;
}