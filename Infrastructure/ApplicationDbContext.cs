using Microsoft.EntityFrameworkCore;
using Domain.Entities;

namespace Infrastructure;

public sealed class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options)
        : base(options)
    {
        Database.EnsureCreated();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
      
        modelBuilder.Entity<Flights>()
            .HasNoKey();
        modelBuilder.Entity<Routes>()
            .HasNoKey();
        modelBuilder.Entity<Subscriptions>()
            .HasNoKey();


        modelBuilder.Entity<Flights>()
             .HasIndex(f => f.departure_time)
             .HasDatabaseName("idx_flights_departure_time");

        modelBuilder.Entity<Flights>()
            .HasIndex(f => f.airline_id)
            .HasDatabaseName("idx_flights_airline_id");

        modelBuilder.Entity<Routes>()
            .HasIndex(r => r.route_id)
            .HasDatabaseName("idx_routes_route_id");

        modelBuilder.Entity<Subscriptions>()
        .HasIndex(s => new { s.origin_city_id, s.destination_city_id, s.agency_id })
        .HasDatabaseName("idx_subscriptions_origin_destination_agency");
    }

    public DbSet<Flights> Flights { get; set; }
    public DbSet<Routes> Routes { get; set; }
    public DbSet<Subscriptions> Subscriptions { get; set; }
}
