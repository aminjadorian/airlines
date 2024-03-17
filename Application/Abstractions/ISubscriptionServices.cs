namespace Application.Abstractions;

public interface ISubscriptionServices
{
    Task InsertAsync(CancellationToken cancellationToken = default);
    HashSet<(long, long)> GetSubscriptionWithAgencyId(long agencyId, CancellationToken cancellationToken = default);
}
