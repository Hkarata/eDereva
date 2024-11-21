namespace eDereva.Core.Contracts.Responses;

public class ChoiceDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; }
}