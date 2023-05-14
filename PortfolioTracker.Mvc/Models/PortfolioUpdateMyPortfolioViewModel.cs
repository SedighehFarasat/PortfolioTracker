namespace PortfolioTracker.Mvc.Models;

public class PortfolioUpdateMyPortfolioViewModel
{
    public string? InstrumentId { get; set; }

    public long Quantity { get; set; }

    public decimal AveragePrice { get; set; }

    public bool IsSold { get; set; }
}
