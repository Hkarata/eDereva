using eDereva.Core.Entities;

namespace eDereva.Core.Contracts.Requests;

public class QuestionBankDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public static class QuestionBankExtensions
{
    public static QuestionBank MapToQuestionBank(this QuestionBankDto dto)
    {
        return new QuestionBank
        {
            Id = Guid.CreateVersion7(),
            Name = dto.Name,
            Description = dto.Description
        };
    }
}