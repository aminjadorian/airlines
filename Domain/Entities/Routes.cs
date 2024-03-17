using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Routes
{
    public long route_id { get; set; } 
    public long origin_city_id { get; set; }
    public long destination_city_id { get; set; }
    public DateTime departure_date { get; set; }
}
