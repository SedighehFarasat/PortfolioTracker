using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PortfolioTracker.EntityModels.Contracts;
using PortfolioTracker.EntityModels.Entities;
using PortfolioTracker.Mvc.Models;
using System.Security.Claims;

namespace PortfolioTracker.Mvc.Controllers;

[Authorize]
public class PortfolioController : Controller
{
    private readonly IAssetRepository _assetRepo;
    public readonly IHttpClientFactory _clientFactory;

    public PortfolioController(IAssetRepository assetRepo, IHttpClientFactory clientFactory)
    {
        _assetRepo = assetRepo;
        _clientFactory = clientFactory;
    }

    [HttpGet]
    public async Task<IActionResult> GetMyPortfolio()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId is null) return StatusCode(401, "User Not Authorized.");

        var assetList = await _assetRepo.GetByUserId(userId);

        PortfolioCommonViewModel commonViewModel = new();
        HttpClient client = _clientFactory.CreateClient(name: "CapitalMarketDataWebApi");
        foreach (var asset in assetList)
        {
            Instrument? tickerModel = null;
            HttpRequestMessage tickerRequest = new(HttpMethod.Get, $"api/v1/instrument/{asset.InstrumentId}/InstrumentById");
            HttpResponseMessage tickerResponse = await client.SendAsync(tickerRequest);
            if (tickerResponse.IsSuccessStatusCode)
            {
                tickerModel = await tickerResponse.Content.ReadFromJsonAsync<Instrument>();
            }

            TradingData? tradingDataModel = null;
            HttpRequestMessage tradingDataRequest = new(HttpMethod.Get, $"api/v1/tradingdata/{asset.InstrumentId}");
            HttpResponseMessage tradingDataResponse = await client.SendAsync(tradingDataRequest);
            if (tickerResponse.IsSuccessStatusCode)
            {
                tradingDataModel = await tradingDataResponse.Content.ReadFromJsonAsync<TradingData>();
            }

            commonViewModel.GetVM.Add(new PortfolioGetMyPortfolioViewModel()
            {
                InstrumentId = asset.InstrumentId,
                Ticker = tickerModel?.Ticker ?? string.Empty,
                Quantity = asset.Quantity,
                AveragePrice = asset.AveragePrice,
                ClosingPrice = tradingDataModel?.ClosingPrice ?? null,
                ProfitOrLossMoney = (tradingDataModel?.ClosingPrice - asset.AveragePrice) * asset.Quantity,
                ProfitOrLossPercent = ((tradingDataModel?.ClosingPrice - asset.AveragePrice) / asset.AveragePrice) * 100,
            });
        }

        return View(commonViewModel);
    }

    [HttpPost]
    public async Task<IActionResult> UpdateMyPortfolio(PortfolioCommonViewModel model)
    {
        if (ModelState.IsValid)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId is null) return StatusCode(401, "User is not authorized");

            if (model.PostVM?.InstrumentId is null) return BadRequest();
            var asset = await _assetRepo.GetByUserIdAndInstrumentId(userId, model.PostVM.InstrumentId);

            if (model.PostVM.IsSold)
            {
                if (asset is not null)
                {
                    if (asset.Quantity == model.PostVM.Quantity)
                    {
                        await _assetRepo.Remove(asset);
                    }
                    else
                    {
                        asset.Quantity -= model.PostVM.Quantity;
                        asset.AveragePrice = (asset.Quantity * asset.AveragePrice + model.PostVM.Quantity * model.PostVM.AveragePrice) / (asset.Quantity + model.PostVM.Quantity);
                        await _assetRepo.Update(asset);
                    }
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                if (asset is not null)
                {
                    asset.Quantity += model.PostVM.Quantity;
                    asset.AveragePrice = (asset.Quantity * asset.AveragePrice + model.PostVM.Quantity * model.PostVM.AveragePrice) / (asset.Quantity + model.PostVM.Quantity);
                    await _assetRepo.Update(asset);
                }
                else
                {
                    Asset newAsset = new()
                    {
                        UserId = userId,
                        InstrumentId = model.PostVM.InstrumentId,
                        Quantity = model.PostVM.Quantity,
                        AveragePrice = model.PostVM.AveragePrice,
                    };
                    await _assetRepo.Add(newAsset);
                }
            }
        }
        else
        {
            return BadRequest();
        }

        return RedirectToAction("GetMyPortfolio");
    }
}