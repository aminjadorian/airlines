using Application.Dtos;
using Domain.Entities;
using Domain.Models;
using System.Collections.Concurrent;

namespace Application.Abstractions;

public interface IFlightsServices
{
    IQueryable<GetAllFlightsDto> GetAllAsync(DateTime startDate, DateTime endDate);
    Task InsertAsync(CancellationToken cancellationToken);
    Task<ConcurrentBag<FlightResult>> FlightChangeFinder(DateTime startDate, DateTime endDate, long agencyId);
}
