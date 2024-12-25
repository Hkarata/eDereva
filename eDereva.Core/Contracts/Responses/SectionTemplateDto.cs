namespace eDereva.Core.Contracts.Responses;

public class SectionTemplateDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<QuestionDto>? Questions { get; set; }
}