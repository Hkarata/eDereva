using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Option;

public class DeleteOptionEndpoint(IOptionRepository optionRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/options/{optionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Option")
                .WithSummary("Delete an existing option")
                .WithDescription(
                    "This endpoint deletes an existing option. The option name or ID should be provided in the request to identify the option to be deleted.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var optionId = Route<Guid>("optionId");

        await optionRepository.DeleteAsync(optionId, ct);

        await SendOkAsync(ct);
    }
}