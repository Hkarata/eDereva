using Microsoft.EntityFrameworkCore;

namespace eDereva.Core.Entities;

[Keyless]
public class Answer
{
    public Guid QuestionId { get; set; }
    public Guid ChoiceId { get; set; }
}