using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Option;

public class PatchOptionEndpoint(IOptionRepository optionRepository) : Endpoint<OptionDto, OptionDto>
{
    public override void Configure()
    {
        Patch("/options/{optionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Option")
                .WithSummary("Update an existing option")
                .WithDescription(
                    "This endpoint updates an existing option. The option name or ID should be provided in the request to identify the option to be updated.");
        });
    }

    public override async Task HandleAsync(OptionDto req, CancellationToken ct)
    {
        var optionId = Route<Guid>("optionId");

        var option = new Core.Entities.Option
        {
            Id = optionId,
            QuestionId = req.QuestionId,
            IsCorrect = req.IsCorrect,
            Text = req.Text
        };

        await optionRepository.UpdateAsync(option, ct);

        await SendOkAsync(ct);
    }
}