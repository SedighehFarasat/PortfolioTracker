using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PortfolioTracker.Data;
using PortfolioTracker.EntityModels.Entities;
using PortfolioTracker.Mvc.Models;
using System.Security.Claims;

namespace PortfolioTracker.Mvc.Controllers
{
    [Authorize]
    public class PortfolioController : Controller
    {
        private readonly ILogger<PortfolioController> _logger;
        private readonly PortfolioTrackerDbContext _db;

        public PortfolioController(ILogger<PortfolioController> logger, PortfolioTrackerDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        [HttpGet]
        public IActionResult GetMyPortfolio()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var assetList = _db.Assets.Include(x => x.Instrument)
                .Join(_db.TradingData,
                    a => a.InstrumentId,
                    t => t.InstrumentId,
                    (a, t) => new { a.UserId, a.InstrumentId, a.Instrument.Ticker, a.AveragePrice, a.Quantity, t.ClosingPrice, t.Date })
                .Where(x => x.UserId == userId && x.Date.Date == DateTime.Now.Date)
                .ToList();

            PortfolioCommonViewModel commonViewModel = new ();
            foreach (var asset in assetList)
            {
                commonViewModel.GetVM.Add(new PortfolioGetMyPortfolioViewModel()
                {
                    InstrumentId = asset.InstrumentId,
                    Ticker = asset.Ticker,
                    Quantity = asset.Quantity,
                    AveragePrice = asset.AveragePrice,
                    ClosingPrice = asset.ClosingPrice,
                    ProfitOrLoss = (asset.ClosingPrice - asset.AveragePrice) * asset.Quantity,
                });
            }

            return View(commonViewModel);
        }

        [HttpPost]
        public IActionResult UpdateMyPortfolio(PortfolioCommonViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if (model.PostVM.IsSold)
                {
                    var asset = _db.Assets.FirstOrDefault(x => x.UserId == userId && x.InstrumentId == model.PostVM.InstrumentId);

                    if (asset is not null)
                    {
                        if (asset.Quantity == model.PostVM.Quantity)
                        {
                            _db.Assets.Remove(asset);
                        }
                        else
                        {
                            asset.Quantity -= model.PostVM.Quantity;
                            asset.AveragePrice = (asset.Quantity * asset.AveragePrice + model.PostVM.Quantity * model.PostVM.AveragePrice) / (asset.Quantity + model.PostVM.Quantity);
                        }

                        _db.SaveChanges();
                    }
                    else
                    {
                        return NotFound();
                    }
                }
                else
                {
                    var asset = _db.Assets.FirstOrDefault(x => x.UserId == userId && x.InstrumentId == model.PostVM.InstrumentId);

                    if (asset is not null)
                    {
                        asset.Quantity += model.PostVM.Quantity;
                        asset.AveragePrice = (asset.Quantity * asset.AveragePrice + model.PostVM.Quantity * model.PostVM.AveragePrice) / (asset.Quantity + model.PostVM.Quantity);
                    }
                    else
                    {
                        Asset newAsset = new()
                        {
                            UserId = User.FindFirstValue(ClaimTypes.NameIdentifier).ToString(),
                            InstrumentId = model.PostVM.InstrumentId,
                            Quantity = model.PostVM.Quantity,
                            AveragePrice = model.PostVM.AveragePrice,
                        };

                        _db.Assets.Add(newAsset);
                    }

                    _db.SaveChanges();
                }
            }
            else
            {
                return BadRequest();
            }

            return RedirectToAction("GetMyPortfolio");
        }
    }
}