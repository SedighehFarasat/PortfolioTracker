using Microsoft.EntityFrameworkCore;
using PortfolioTracker.BackgroundTasks;
using PortfolioTracker.Data;

IHost host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((builder, services) =>
    {
        services.AddDbContext<PortfolioTrackerDbContext>(option =>
        {
            option.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
        });
        services.AddHostedService<Worker>();
    })
    .Build();

host.Run();
