using PortfolioTracker.EntityModels.Enums;

namespace PortfolioTracker.EntityModels.Entities;

public class Instrument
{
    public Instrument()
    {
        TradingData = new HashSet<TradingData>();
        Assets = new HashSet<Asset>();
    }

    /// <summary>
    /// Gets or sets the instrument's ISIN code.
    /// </summary>
    public string? Id { get; set; }

    /// <summary>
    /// Gets or sets INS code of the instrument. INS code is a unique code of an instrument in tsetmc.com.
    /// </summary>
    public string? InsCode { get; set; }

    public string? Ticker { get; set; }

    /// <summary>
    /// Gets or sets the full name of the instrument.
    /// </summary>
    public string? Name { get; set; }

    public Board? Board { get; set; }

    public Industry? Industry { get; set; }

    public ICollection<TradingData> TradingData { get; set; }

    public ICollection<Asset> Assets { get; set; }
}
