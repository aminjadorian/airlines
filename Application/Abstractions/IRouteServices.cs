namespace Application.Abstractions;

public interface IRouteServices
{
    Task InsertAsync(CancellationToken cancellationToken);
}
