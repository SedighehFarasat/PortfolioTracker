using PortfolioTracker.EntityModels.Enums;

namespace PortfolioTracker.EntityModels.Entities;
public class Instrument
{
    public Instrument()
    {
        TradingData = new HashSet<TradingData>();
        Assets = new HashSet<Asset>();
    }

    public string Id { get; set; }
    public string InsCode { get; set; }
    public string Ticker { get; set; }
    public string Name { get; set; }
    public Board? Board { get; set; }
    public Industry? Industry { get; set; }

    public ICollection<TradingData> TradingData { get; set; }
    public ICollection<Asset> Assets { get; set; }
}
