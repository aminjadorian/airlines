using Application.Abstractions;
using Application.Dtos;
using EFCore.BulkExtensions;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories.Subscriptions;

public class SubscriptionServices : ISubscriptionServices
{
    private readonly ApplicationDbContext _context;

    public SubscriptionServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public HashSet<(long, long)> GetSubscriptionWithAgencyId(long agencyId, CancellationToken cancellationToken = default)
    {
        var subscriptions = new HashSet<(long, long)>(
             _context.Subscriptions
             .Where(s => s.agency_id == agencyId)
              .Select(s => new
               {
                s.origin_city_id,
                s.destination_city_id
                }).ToList()
             .Select(s => (s.origin_city_id, s.destination_city_id)));

        return subscriptions;
    }

    public async Task InsertAsync(CancellationToken cancellationToken)
    {
        var subscriptions = new List<Domain.Entities.Subscriptions>();

        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/subscriptions.csv");

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            subscriptions = csv.GetRecords<Domain.Entities.Subscriptions>().ToList();
        }
        if (!_context.Subscriptions.Any())
        {
            await _context.BulkInsertAsync(subscriptions, cancellationToken: cancellationToken);
        }
    }
}
