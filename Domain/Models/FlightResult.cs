using Domain.Entities;

namespace Domain.Models;

public record FlightResult(
    long origin_city_id,
    long destination_city_id,
    DateTime departure_time,
    DateTime arrival_time,
    long airline_id,
    string Status
    );
