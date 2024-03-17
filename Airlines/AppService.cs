using Application.Abstractions;
using Domain.Models;
using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

partial class AppService
{
    private readonly ILogger<AppService> _logger;
    private readonly IFlightsServices _flightServices;
    private readonly IRouteServices _routeServices;
    private readonly ISubscriptionServices _subscriptionServices;
    public AppService(
        ILogger<AppService> logger,
        IFlightsServices flightServices,
        ISubscriptionServices subscriptionServices,
        IRouteServices routeServices)
    {
        _logger = logger;
        _flightServices = flightServices;
        _subscriptionServices = subscriptionServices;
        _routeServices = routeServices;
    }

    public async Task ExecuteAsync(CancellationToken stoppingToken = default)
    {

        try
        {
            await _flightServices.InsertAsync(stoppingToken);
            await _routeServices.InsertAsync(stoppingToken);
            await _subscriptionServices.InsertAsync(stoppingToken);
        }
        catch (Exception e)
        {
            _logger.LogInformation(e.Message);

        }


        Console.WriteLine("Insert Start date with format of: yyyy-mm-dd");
        var value = Console.ReadLine();
        if (value is null)
            value = Console.ReadLine();
        var startDate = DateTime.Parse(value!);


        Console.WriteLine("Insert End date with format of: yyyy-mm-dd");
        value = Console.ReadLine();
        if (value is null)
            value = Console.ReadLine();

        var endDate = DateTime.Parse(value!);

        Console.WriteLine("Insert Agency Id");
        value = Console.ReadLine();
        if (value is null)
            value = Console.ReadLine();

        var agencyId = long.Parse(value!);

        Stopwatch stopwatch = new();
        stopwatch.Start();

        var flights = await _flightServices.FlightChangeFinder(startDate, endDate, agencyId);
        stopwatch.Stop();
        var ts = stopwatch.Elapsed;
        var elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
    ts.Hours, ts.Minutes, ts.Seconds,
    ts.Milliseconds / 10);
        Console.WriteLine("RunTime " + elapsedTime);

        WriteToCsv(flights);
        Console.WriteLine("result.csv has been saved in wwwroot/files");
        Console.ReadKey();
    }

    private void WriteToCsv(ConcurrentBag<FlightResult> results)
    {
        // Convert the ConcurrentBag to a List
        var resultList = results.ToList();

        // Specify the file to write to
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/results.csv");
        // Create a new StreamWriter and open the file
        using (var writer = new StreamWriter(filePath))
        {
            // Write the header line
            writer.WriteLine("OriginCityId,DestinationCityId,DepartureTime,ArrivalTime,AirlineId,Status");

            // Write each result
            foreach (var result in resultList)
            {
                writer.WriteLine(
                    $",{result.origin_city_id}," +
                    $"{result.destination_city_id}," +
                    $"{result.destination_city_id}," +
                    $"{result.arrival_time}," +
                    $"{result.airline_id}," +
                    $"{result.Status}");
            }
        }
    }
}
