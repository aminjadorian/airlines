using System.ComponentModel.DataAnnotations;

namespace Domain.Entities;

public class Subscriptions
{
    public long agency_id { get; set; }
    public long origin_city_id { get; set; }
    public long destination_city_id { get; set; }
}
