namespace eDereva.Core.Contracts.Responses;

public class TestDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } // Test duration in minutes
    public int PassScore { get; set; }
    public string TestVersion { get; set; } = string.Empty;

    public List<TestSectionDto>? TestSections { get; set; }

    // Computed Property for Total Questions
    public int TotalQuestions => TestSections?.Sum(ts => ts.ContributionCount) ?? 0;
}