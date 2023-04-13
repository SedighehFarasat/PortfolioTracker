using PortfolioTracker.BackgroundTasks.Models;

namespace PortfolioTracker.UnitTests;
public class BackgroundTasksTests : IAsyncLifetime
{
    private Task<Trade> _data;

    public async Task InitializeAsync()
    {
        var isin = "IRO1FKHZ0001";
        await Task.Run(() => _data = BackgroundTasks.Services.TseService.FetchLiveData(isin));
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void TseService()
    {
        Assert.NotNull(_data.Result);
    }
}
