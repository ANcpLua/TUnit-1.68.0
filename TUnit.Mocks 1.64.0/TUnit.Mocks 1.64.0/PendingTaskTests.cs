namespace TUnit.Mocks_1._64._0;

public interface IQuoteFeed
{
    Task<decimal> LatestAsync(string symbol);
}

public class PendingTaskTests
{
    // New in 1.62.0: Returns(...) accepts an async factory on async members, handing the
    // task back still pending. Before, only already-completed tasks were expressible — a
    // caller's timeout could never win the race, making tests like this one unwritable.
    // (1.63.0 extends the same alias to net8.0 and .NET Framework targets.)
    [Test]
    public async Task Caller_timeout_beats_a_hanging_feed()
    {
        var feed = IQuoteFeed.Mock();
        var stall = new TaskCompletionSource<decimal>();
        feed.LatestAsync(Any()).Returns(async () => await stall.Task);

        IQuoteFeed quotes = feed;
        var pending = quotes.LatestAsync("TUNIT");
        var winner = await Task.WhenAny(pending, Task.Delay(TimeSpan.FromMilliseconds(50)));

        await Assert.That(winner).IsNotSameReferenceAs(pending);
        await Assert.That(pending.IsCompleted).IsFalse();

        stall.SetResult(99.5m);
        await Assert.That(await pending).IsEqualTo(99.5m);
    }

    [Test]
    public async Task Async_factory_runs_once_per_call()
    {
        var feed = IQuoteFeed.Mock();
        var calls = 0;
        feed.LatestAsync(Any()).Returns(async () =>
        {
            calls++;
            await Task.Yield();
            return 10m * calls;
        });

        IQuoteFeed quotes = feed;

        await Assert.That(await quotes.LatestAsync("A")).IsEqualTo(10m);
        await Assert.That(await quotes.LatestAsync("B")).IsEqualTo(20m);
    }
}
