namespace eDereva.Core.Contracts.Responses;

public class OptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }

    // Foreign Key
    public Guid QuestionId { get; set; }
}