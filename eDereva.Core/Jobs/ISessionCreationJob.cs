namespace eDereva.Core.Jobs;

public interface ISessionCreationJob
{
    Task RunJobAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}