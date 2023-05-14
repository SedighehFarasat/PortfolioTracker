using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.Data.TypeConfigurations;

public class AssetTypeConfiguration : IEntityTypeConfiguration<Asset>
{
    private const string TableName = "Assets";

    public void Configure(EntityTypeBuilder<Asset> builder)
    {
        builder.ToTable(TableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.UserId)
            .IsRequired()
            .HasMaxLength(36);

        builder.Property(x => x.InstrumentId)
            .IsRequired()
            .HasMaxLength(32);

        builder.Property(x => x.Quantity)
            .IsRequired();

        builder.Property(x => x.AveragePrice)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.HasIndex(x => x.InstrumentId);
    }
}
