using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.Data.TypeConfigurations;
public class AssetTypeConfiguration : IEntityTypeConfiguration<Asset>
{
    private const string _tableName = "Assets";

    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.InstrumentId)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(x => x.Quantity)
            .IsRequired();
        
        builder.Property(x => x.AveragePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasOne(x => x.Instrument)
                .WithMany(x => x.Assets)
                .HasForeignKey(x => x.InstrumentId)
                .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(x => x.InstrumentId);
    }
}
