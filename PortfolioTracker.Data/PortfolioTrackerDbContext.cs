using Microsoft.EntityFrameworkCore;
using PortfolioTracker.EntityModels.Entities;
using System.Reflection;

namespace PortfolioTracker.Data;
public class PortfolioTrackerDbContext : DbContext
{
    public PortfolioTrackerDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }
    public DbSet<Instrument> Instruments { get; set; }
    public DbSet<TradingData> TradingData { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}