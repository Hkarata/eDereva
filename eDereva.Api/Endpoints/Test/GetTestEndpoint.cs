using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Test;

public class GetTestEndpoint(ITestRepository testRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/tests/{testId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Test")
                .WithSummary("Get a test")
                .WithDescription(
                    "Get a test");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var testId = Route<Guid>("testId");

        var test = await testRepository.GetByIdAsync(testId, ct);

        if (test != null)
            await SendOkAsync(test, ct);
        else
            await SendNotFoundAsync(ct);
    }
}