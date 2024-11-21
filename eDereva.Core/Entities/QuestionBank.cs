namespace eDereva.Core.Entities;

public class QuestionBank
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    
    // Navigation properties
    public ICollection<Question>? Questions { get; set; }
}