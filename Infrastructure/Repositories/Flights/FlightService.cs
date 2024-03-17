using Application.Abstractions;
using Application.Dtos;
using Domain.Entities;
using EFCore.BulkExtensions;
using System.Formats.Asn1;
using System.Globalization;
using CsvHelper;
using Microsoft.EntityFrameworkCore;
using Domain.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Infrastructure.Repositories.Flights;

public class FlightService : IFlightsServices
{
    private readonly ApplicationDbContext _context;
    private readonly ISubscriptionServices _subscriptions;
    public FlightService(ApplicationDbContext context, ISubscriptionServices subscriptions)
    {
        _context = context;
        _subscriptions = subscriptions;
    }

    public async Task<ConcurrentBag<FlightResult>> FlightChangeFinder(DateTime startDate, DateTime endDate, long agencyId)
    {
        var subscriptions = _subscriptions.GetSubscriptionWithAgencyId(agencyId);

        if (subscriptions is null)
        {
            Console.WriteLine("there is no subscriptions");
            return null!;
        }


        var iQueryable_Flights = GetAllAsync(startDate, endDate);

        var flightsList = await iQueryable_Flights.ToListAsync();

        flightsList = flightsList.Where(
              f => subscriptions!.Contains(new ValueTuple<long, long>(f.origin_city_id, f.destination_city_id)))
              .ToList();

        var flightsByAirline = new Dictionary<long, List<GetAllFlightsDto>>();

        foreach (var flight in flightsList)
        {
            if (!flightsByAirline.ContainsKey(flight.airline_id))
            {
                flightsByAirline[flight.airline_id] = [flight];
            }
        }

        foreach (var list in flightsByAirline.Values)
        {
            list.Sort((flight1, flight2) => 
                flight1.departure_time.CompareTo(flight2.departure_time));
        }

        var results = new ConcurrentBag<FlightResult>();

        Parallel.ForEach(flightsList, flight =>
        {
            
            var previousFlight = GetFlight(flightsByAirline, flight.airline_id, flight.departure_time.AddDays(-7), 30);

            
            var nextFlight = GetFlight(flightsByAirline, flight.airline_id, flight.departure_time.AddDays(7), 30);


            if (previousFlight == null && nextFlight == null)
            {
                results.Add(new FlightResult
                (
                    flight.origin_city_id,
                    flight.destination_city_id,
                    flight.departure_time,
                    flight.arrival_time,
                    flight.airline_id,
                    "New"
                ));
            }
            else if (previousFlight != null && nextFlight == null)
            {
                results.Add(new FlightResult
                (
                    flight.origin_city_id,
                    flight.destination_city_id,
                    flight.departure_time,
                    flight.arrival_time,
                    flight.airline_id,
                    "Discontinued"
                ));
            }
        });

        return results;
    }

    public IQueryable<GetAllFlightsDto> GetAllAsync(DateTime startDate, DateTime endDate)
    {
        var currentFlights = from f in _context.Flights
                             join r in _context.Routes on f.route_id equals r.route_id
                             where f.departure_time >= startDate && f.departure_time <= endDate
                             select new GetAllFlightsDto(
                                 r.origin_city_id,
                                 r.destination_city_id,
                                 f.departure_time,
                                 f.arrival_time,
                                 f.airline_id
                             );

        return currentFlights;
    }

    public async Task InsertAsync(CancellationToken cancellationToken)
    {
        List<Domain.Entities.Flights> flights;

        var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "files/flights.csv");

        using (var reader = new StreamReader(path))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            flights = csv.GetRecords<Domain.Entities.Flights>().ToList();
        }
        if (!_context.Flights.Any())
        {
            await _context.BulkInsertAsync(flights, cancellationToken: cancellationToken);
        }
    }

    private GetAllFlightsDto GetFlight(Dictionary<long, List<GetAllFlightsDto>> flightsByAirline, long airlineId, DateTime departureTime, int tolerance)
    {
        if (!flightsByAirline.ContainsKey(airlineId))
        {
            return null!;
        }

        var flights = flightsByAirline[airlineId];

        var left = 0;
        var right = flights.Count - 1;
        while (left <= right)
        {
            var mid = left + (right - left) / 2;
            if (Math.Abs((flights[mid].departure_time - departureTime).TotalMinutes) <= tolerance)
            {
                return flights[mid];
            }
            else if (flights[mid].departure_time < departureTime)
            {
                left = mid + 1;
            }
            else
            {
                right = mid - 1;
            }
        }

        return null!;
    }
}
