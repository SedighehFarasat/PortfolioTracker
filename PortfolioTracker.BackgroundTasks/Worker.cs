using PortfolioTracker.BackgroundTasks.Helper;
using PortfolioTracker.BackgroundTasks.Services;
using PortfolioTracker.Data;
using PortfolioTracker.EntityModels.Entities;
using PortfolioTracker.EntityModels.Enums;

namespace PortfolioTracker.BackgroundTasks
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly IServiceProvider _serviceProvider;

        public Worker(ILogger<Worker> logger, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var _db = scope.ServiceProvider.GetRequiredService<PortfolioTrackerDbContext>();
                var instruments = _db.Instruments.Select(x => new { x.Id, x.Ticker }).ToArray();
                foreach (var instrument in instruments)
                {
                    if (instrument.Id is not null)
                    {
                        try
                        {
                            var data = TseService.FetchLiveData(instrument.Id);
                            if (data is not null)
                            {
                                TradingData tradingData = new()
                                {
                                    InstrumentId = instrument.Id,
                                    Status = Convertor.ToStatusEnum(data.Result.header[0].state),
                                };
                                if (tradingData.Status == Status.Trading)
                                {
                                    tradingData.OpeningPrice = decimal.Parse(data.Result.mainData.agh);
                                    tradingData.HighestPrice = decimal.Parse(data.Result.mainData.bt.u);
                                    tradingData.LowestPrice = decimal.Parse(data.Result.mainData.bt.d);
                                    tradingData.LastPrice = decimal.Parse(data.Result.header[1].am);
                                    tradingData.ClosingPrice = decimal.Parse(data.Result.mainData.ghp.v);
                                    tradingData.PreviousClosingPrice = decimal.Parse(data.Result.mainData.rgh);
                                    tradingData.UpperBoundPrice = decimal.Parse(data.Result.mainData.bm.u);
                                    tradingData.LowerBoundPrice = decimal.Parse(data.Result.mainData.bm.d);
                                    tradingData.NumberOfTrades = int.Parse(data.Result.mainData.dm.Replace(",", string.Empty));
                                    tradingData.TradingValue = Convertor.ToNumber(data.Result.mainData.arm);
                                    tradingData.TradingVolume = (long)Convertor.ToNumber(data.Result.mainData.hmo);
                                }
                                if (!_db.TradingData.Any(x => x.Date == tradingData.Date && x.InstrumentId == tradingData.InstrumentId))
                                {
                                    _db.TradingData.Add(tradingData);
                                    int affected = _db.SaveChanges();
                                    _logger.LogInformation($"{affected} row affected for {instrument.Id}");
                                }
                            }
                        }
                        catch (HttpRequestException e)
                        {
                            _logger.LogError($"Http Exception Caught On {instrument.Id}: {e.Message}");
                        }
                        catch (Exception e)
                        {
                            _logger.LogError($"Other Exception Caught On {instrument.Id}: {e.Message}");
                        }
                        await Task.Delay(3000, stoppingToken);
                    }
                }
            }
        }
    }
}