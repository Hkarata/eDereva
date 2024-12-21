namespace eDereva.Core.Entities;

public class Option
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    // Foreign Key
    public Guid QuestionId { get; set; }

    // Navigation Properties
    public Question Question { get; set; } = default!;
}