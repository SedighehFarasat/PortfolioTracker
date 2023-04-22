namespace PortfolioTracker.EntityModels.Entities;

public class Asset
{
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets asset's ISIN code.
    /// </summary>
    public string? InstrumentId { get; set; }

    /// <summary>
    ///  Gets or sets the number of asset.
    /// </summary>
    public long Quantity { get; set; }

    /// <summary>
    /// Gets or sets the average buy price.
    /// </summary>
    public decimal AveragePrice { get; set; }

    public Instrument? Instrument { get; set; }
}
