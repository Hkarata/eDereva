namespace eDereva.Core.Entities;

public class TestSection
{
    public Guid Id { get; set; }

    public Guid TestId { get; set; }

    public Guid SectionTemplateId { get; set; }

    public int ContributionCount { get; set; }

    // Navigation Properties
    public Test Test { get; set; } = default!;
    public SectionTemplate SectionTemplate { get; set; } = default!;
    public ICollection<Question>? Questions { get; set; }
}