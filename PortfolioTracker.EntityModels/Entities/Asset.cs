namespace PortfolioTracker.EntityModels.Entities;
public class Asset
{
    public int Id { get; set; }
    public string InstrumentId { get; set; }
    public long Quantity { get; set; }
    public decimal AveragePrice { get; set; }

    public Instrument Instrument { get; set; }
}
