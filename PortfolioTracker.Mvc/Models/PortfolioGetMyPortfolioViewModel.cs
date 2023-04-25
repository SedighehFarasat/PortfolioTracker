namespace PortfolioTracker.Mvc.Models;

public class PortfolioGetMyPortfolioViewModel
{
    public string? InstrumentId { get; set; }

    public string? Ticker { get; set; }

    public long Quantity { get; set; }

    public decimal AveragePrice { get; set; }

    public decimal? ClosingPrice { get; set; }

    public decimal? ProfitOrLoss { get; set; }
}
