namespace eDereva.Core.Contracts.Responses;

public class QuestionDto
{
    public Guid Id { get; set; }
    public string Scenario { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<OptionDto>? Choices { get; set; }
    public Guid SectionTemplateId { get; set; }
}