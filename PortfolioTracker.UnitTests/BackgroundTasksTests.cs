using PortfolioTracker.BackgroundTasks.Models;

namespace PortfolioTracker.UnitTests;

public class BackgroundTasksTests : IAsyncLifetime
{
    private Task<Trade> _data;

    public async Task InitializeAsync()
    {
        string isin = "IRO1FKHZ0001";
        await Task.Run(() => _data = BackgroundTasks.Services.TseService.FetchLiveData(isin));
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public void TseService_ValidIsin()
    {
        Assert.NotNull(_data.Result);
    }

    [Fact(Skip = "Do not know how to write two tests in asynce mode!")]
    public void TseService_NullIsin()
    {
        Assert.NotNull(_data.Result);
    }
}
