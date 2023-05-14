namespace PortfolioTracker.Mvc.Models;

public class PortfolioCommonViewModel
{
    public PortfolioCommonViewModel()
    {
        GetVM = new List<PortfolioGetMyPortfolioViewModel>();
    }

    public List<PortfolioGetMyPortfolioViewModel> GetVM { get; set; }
    public PortfolioUpdateMyPortfolioViewModel PostVM { get; set; }
}
