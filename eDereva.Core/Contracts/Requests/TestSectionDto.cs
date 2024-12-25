namespace eDereva.Core.Contracts.Requests;

public class TestSectionDto
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid SectionTemplateId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public int ContributionCount { get; set; }
}