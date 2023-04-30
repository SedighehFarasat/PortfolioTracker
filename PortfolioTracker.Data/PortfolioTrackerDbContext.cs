using System.Reflection;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.Data;

public class PortfolioTrackerDbContext : DbContext
{
    public PortfolioTrackerDbContext(DbContextOptions<PortfolioTrackerDbContext> options)
        : base(options)
    {
    }

    public DbSet<Asset> Assets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}