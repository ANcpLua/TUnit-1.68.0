using TUnit.Mocks;

namespace TUnit.Mocks_1._68._17;

public interface IClock
{
    static abstract IClock System { get; }

    DateTimeOffset Now { get; }
}

public interface IRateLimiter
{
    // Hidden by a same-named generic overload below.
    bool TryAcquire(string key);

    bool TryAcquire<TScope>(string key);
}

public sealed class Scheduler(IClock clock, IRateLimiter limiter)
{
    public bool CanRun(string job) => limiter.TryAcquire<Scheduler>(job) && clock.Now.Hour < 12;
}

public class StaticAbstractTests
{
    private const string Job = "nightly-report";
    private static readonly DateTimeOffset Morning = new(2026, 8, 29, 9, 0, 0, TimeSpan.Zero);

    // New in 1.65.0: T.Mock() also works for interfaces that declare static abstract members.
    // The generator mocks a derived "IClockMockable" view of the instance surface, so unlike a
    // plain interface the wrapper is not the interface itself — pass .Object to the SUT.
    [Test]
    public async Task Interfaces_with_static_abstract_members_are_mockable()
    {
        var clock = IClock.Mock();
        clock.Now.Returns(Morning);

        // Fixed in 1.65.63: the generic overload no longer hides the non-generic one during generation.
        var limiter = IRateLimiter.Mock();
        limiter.TryAcquire<Scheduler>(Job).Returns(true);

        var canRun = new Scheduler(clock.Object, limiter).CanRun(Job);

        await Assert.That(canRun).IsTrue();
        limiter.TryAcquire<Scheduler>(Job).WasCalled(Times.Once);
        limiter.TryAcquire(Job).WasNeverCalled();
    }
}
