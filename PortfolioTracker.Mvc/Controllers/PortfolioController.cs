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
        private readonly PortfolioTrackerDbContext _db;
        public readonly IHttpClientFactory _clientFactory;

        public PortfolioController(PortfolioTrackerDbContext db, IHttpClientFactory clientFactory)
        {
            _db = db;
            _clientFactory = clientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPortfolio()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            HttpClient client = _clientFactory.CreateClient(name: "CapitalMarketDataWebApi");

            var assetList = _db.Assets
                .Where(x => x.UserId == userId)
                .ToList();

            PortfolioCommonViewModel commonViewModel = new();
            foreach (var asset in assetList)
            {
                HttpRequestMessage tickerRequest = new(HttpMethod.Get, $"api/instrument/{asset.InstrumentId}/InstrumentById");
                HttpResponseMessage tickerResponse = await client.SendAsync(tickerRequest);
                var tickerModel = await tickerResponse.Content.ReadFromJsonAsync<Instrument>();

                HttpRequestMessage tradingDataRequest = new(HttpMethod.Get, $"api/tradingdata/{asset.InstrumentId}");
                HttpResponseMessage tradingDataResponse = await client.SendAsync(tradingDataRequest);
                var tradingDataModel = await tradingDataResponse.Content.ReadFromJsonAsync<TradingData>();

                commonViewModel.GetVM.Add(new PortfolioGetMyPortfolioViewModel()
                {
                    InstrumentId = asset.InstrumentId,
                    Ticker = tickerModel?.Ticker ?? string.Empty,
                    Quantity = asset.Quantity,
                    AveragePrice = asset.AveragePrice,
                    ClosingPrice = tradingDataModel?.ClosingPrice ?? null,
                    ProfitOrLoss = (tradingDataModel?.ClosingPrice - asset.AveragePrice) * asset.Quantity,
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