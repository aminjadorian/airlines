using Application;
using Application.Abstractions;
using Infrastructure;
using Infrastructure.Repositories.Flights;
using Infrastructure.Repositories.Routes;
using Infrastructure.Repositories.Subscriptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices(services =>
    {

        services.AddTransient<AppService>();
        services.AddScoped<IFlightsServices, FlightService>();
        services.AddScoped<IRouteServices, RouteServices>();
        services.AddScoped<ISubscriptionServices, SubscriptionServices>();

        services.AddApplication()
        .AddInfrastructure();

        services.AddDbContext<ApplicationDbContext>(options =>
        {
            options.UseSqlServer("Data Source=.; Initial Catalog=AirLines;Integrated Security=True;Encrypt=False");
        });
    })
    .Build();

var my = host.Services.GetRequiredService<AppService>();

await my.ExecuteAsync();
