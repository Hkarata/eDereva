namespace eDereva.Core.Entities;

public class Question
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public ICollection<Option>? Options { get; set; }

    // Foreign Key to Section Template
    public Guid SectionTemplateId { get; set; }

    // Navigation Properties
    public SectionTemplate SectionTemplate { get; set; } = default!;
}