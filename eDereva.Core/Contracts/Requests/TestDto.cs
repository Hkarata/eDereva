namespace eDereva.Core.Contracts.Requests;

public class TestDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; } // Duration in minutes
    public int PassScore { get; set; }
    public string TestVersion { get; set; } = string.Empty;
}