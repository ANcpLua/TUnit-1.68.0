namespace TUnit.Patterns.Extensions;

[Throttled]
public sealed class ExecutorRegistrationTests
{
    [Test]
    public async Task Installed_executor_receives_its_registration_callback_once()
    {
        var context = TestContext.Current!;

        await Assert.That(context.StateBag[ThrottlingExecutor.RegistrationCountKey]).IsEqualTo(1);
        await Assert.That(context.Parallelism.Limiter).IsTypeOf<FourAtATime>();
    }

    // 1.67.0: an explicit [ParallelLimiter<T>] always beats a limiter set programmatically, whichever callback runs
    // last; before, the executor's SetParallelLimiter could silently overwrite the stricter method-level limit.
    [Test]
    [ParallelLimiter<OneAtATime>]
    public async Task Explicit_limiter_beats_the_executor_default()
    {
        var context = TestContext.Current!;

        await Assert.That(context.StateBag[ThrottlingExecutor.RegistrationCountKey]).IsEqualTo(1);
        await Assert.That(context.Parallelism.Limiter).IsTypeOf<OneAtATime>();
    }
}
