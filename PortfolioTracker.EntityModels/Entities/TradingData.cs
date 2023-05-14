using PortfolioTracker.EntityModels.Enums;

namespace PortfolioTracker.EntityModels.Entities;

public class TradingData
{
    public TradingData()
    {
        Date = DateTime.Now;
    }

    public string? InstrumentId { get; set; }

    public DateTime Date { get; private set; }

    public Status? Status { get; set; }

    public decimal? OpeningPrice { get; set; }

    public decimal? HighestPrice { get; set; }

    public decimal? LowestPrice { get; set; }

    public decimal? LastPrice { get; set; }

    public decimal? ClosingPrice { get; set; }

    public decimal? PreviousClosingPrice { get; set; }

    public decimal? UpperBoundPrice { get; set; }

    public decimal? LowerBoundPrice { get; set; }

    public int? NumberOfTrades { get; set; }

    public decimal? TradingValue { get; set; }

    public long? TradingVolume { get; set; }
}
