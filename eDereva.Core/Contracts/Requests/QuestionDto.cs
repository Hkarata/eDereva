using eDereva.Core.Entities;

namespace eDereva.Core.Contracts.Requests;

public class QuestionDto
{
    public string Scenario { get; set; } = string.Empty;
    public List<string>? ImageUrls { get; set; }
    public string QuestionText { get; set; } = string.Empty;
    public List<ChoiceDto>? Choices { get; set; }
}

public static class QuestionExtensions
{
    public static (Question question, Guid questionId, Guid choiceId) MapToQuestion(this QuestionDto dto,
        Guid questionId = default)
    {
        var question = new Question
        {
            Id = questionId != Guid.Empty ? questionId : Guid.CreateVersion7(),
            Scenario = dto.Scenario,
            ImageUrls = dto.ImageUrls,
            QuestionText = dto.QuestionText,
            Choices = dto.Choices?.Select(choiceDto => new Choice
            {
                Id = Guid.CreateVersion7(),
                Text = choiceDto.Text
            }).ToList()
        };

        var correctChoiceId = question.Choices!
            .Where(choice => choice.Text == dto.Choices?.FirstOrDefault(c => c.IsCorrect)?.Text)
            .Select(choice => choice.Id)
            .FirstOrDefault();

        foreach (var choice in question.Choices!) choice.QuestionId = question.Id;

        return (question, question.Id, correctChoiceId!);
    }
}