using Application.Abstractions;
using EFCore.BulkExtensions;
using System.Globalization;
using CsvHelper;

namespace Infrastructure.Repositories.Routes;

public class RouteServices : IRouteServices
{
    private readonly ApplicationDbContext _context;

    public RouteServices(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task InsertAsync(CancellationToken cancellationToken)
    {
        var routes = new List<Domain.Entities.Routes>();

        string path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/routes.csv");

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            routes = csv.GetRecords<Domain.Entities.Routes>().ToList();
        }
        if (!_context.Routes.Any())
        {
            await _context.BulkInsertAsync(routes, cancellationToken: cancellationToken);
        }
    }
}
