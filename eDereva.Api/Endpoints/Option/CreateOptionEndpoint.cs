using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Option;

public class CreateOptionEndpoint(IOptionRepository optionRepository) : Endpoint<OptionDto, OptionDto>
{
    public override void Configure()
    {
        Post("/options");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Option")
                .WithSummary("Create an option")
                .WithDescription("Create an option");
        });
    }

    public override async Task HandleAsync(OptionDto req, CancellationToken ct)
    {
        var option = new Core.Entities.Option
        {
            Id = Guid.NewGuid(),
            QuestionId = req.QuestionId,
            IsCorrect = req.IsCorrect,
            Text = req.Text
        };

        await optionRepository.AddAsync(option, ct);

        await SendCreatedAtAsync<CreateOptionEndpoint>(
            new { option.Id },
            new OptionDto
            {
                Id = option.Id,
                QuestionId = option.QuestionId,
                IsCorrect = option.IsCorrect,
                Text = option.Text
            },
            cancellation: ct
        );
    }
}