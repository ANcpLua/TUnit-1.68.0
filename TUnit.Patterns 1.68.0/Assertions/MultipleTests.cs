using TUnit.Assertions.Exceptions;

namespace TUnit.Patterns.Assertions;

/// <summary>
/// 1.66.16: Assert.Multiple is safe to share across concurrent work. Failures raised from parallel tasks are all
/// collected (unsynchronised writes used to drop some), and an .Or chain only consumes its own failure.
/// </summary>
public sealed class MultipleTests
{
    private const int Workers = 8;
    private const int FailuresPerWorker = 250;
    private const string IndependentFailure = "Independent failure";
    private const int Value = 1;
    private const int Other = 2;

    [Test]
    public async Task Concurrent_failures_are_all_collected()
    {
        var start = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var exception = await Assert.That(async () =>
        {
            using (Assert.Multiple())
            {
                var workers = Enumerable.Range(0, Workers).Select(worker => Task.Run(async () =>
                {
                    await start.Task;

                    for (var failure = 0; failure < FailuresPerWorker; failure++)
                    {
                        Assert.Fail($"worker {worker} failure {failure}");
                    }
                })).ToArray();

                start.SetResult();
                await Task.WhenAll(workers);
            }
        }).Throws<AssertionException>();

        var failures = ((AggregateException)exception!.InnerException!).InnerExceptions;

        await Assert.That(failures.Count).IsEqualTo(Workers * FailuresPerWorker);
        await Assert.That(failures.Select(failure => failure.Message).Distinct().Count()).IsEqualTo(Workers * FailuresPerWorker);
    }

    [Test]
    public async Task Passing_or_chain_does_not_swallow_a_concurrent_failure()
    {
        var started = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);
        var release = new TaskCompletionSource(TaskCreationOptions.RunContinuationsAsynchronously);

        var exception = await Assert.That(async () =>
        {
            using (Assert.Multiple())
            {
                async Task RunChain() =>
                    await Assert.That(async () =>
                    {
                        started.SetResult();
                        await release.Task;
                        return Value;
                    }).IsEqualTo(Value).Or.IsEqualTo(Other);

                var chain = RunChain();

                await started.Task;
                Assert.Fail(IndependentFailure);
                release.SetResult();
                await chain;
            }
        }).Throws<AssertionException>();

        await Assert.That(exception!.Message).IsEqualTo(IndependentFailure);
    }
}
