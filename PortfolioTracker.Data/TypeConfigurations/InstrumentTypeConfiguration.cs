using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PortfolioTracker.EntityModels.Entities;

namespace PortfolioTracker.Data.TypeConfigurations;
public class InstrumentTypeConfiguration : IEntityTypeConfiguration<Instrument>
{
    private const string _tableName = "Instruments";
    private const string _file = @"F:\Algorithmic Trading\Portfolio Tracker\Documents\ISIN.csv";

    public void Configure(EntityTypeBuilder<Instrument> builder)
    {
        builder.ToTable(_tableName);

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(x => x.InsCode)
            .HasMaxLength(32);
        
        builder.Property(x => x.Ticker)
            .IsRequired()
            .HasMaxLength(32);
        
        builder.Property(x => x.Name)
            .HasMaxLength(64);
        
        builder.Property(x => x.Board);
        
        builder.Property(x => x.Industry);

        using (StreamReader textReader = File.OpenText(_file))
        {
            while (!textReader.EndOfStream)
            {
                var line = textReader.ReadLine()!;
                if (line.Contains("IR"))
                {
                    builder.HasData(new Instrument { Id = line.Split(',')[1], Ticker = line.Split(',')[0] });
                }
            }
        }
    }
}
