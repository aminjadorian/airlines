using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Flights
{
    public long flight_id { get; set; }
    public long route_id { get; set; }
    public DateTime departure_time { get; set; }
    public DateTime arrival_time { get; set; }
    public long airline_id { get; set; }
}
