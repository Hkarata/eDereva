using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Test;

public class DeleteTestEndpoint(ITestRepository testRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/tests/{testId}");
        Version(1);
        Description(options =>
        {
            options.WithTags("Test")
                .WithSummary("Delete a test")
                .WithDescription(
                    "Delete a test");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var testId = Route<Guid>("testId");
        await testRepository.DeleteAsync(testId, ct);
        await SendOkAsync(ct);
    }
}