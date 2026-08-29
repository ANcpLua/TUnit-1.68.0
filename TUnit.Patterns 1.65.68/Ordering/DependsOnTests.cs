namespace TUnit.Patterns.Ordering;

/// <summary>
/// [DependsOn] sequences tests without giving up parallelism elsewhere. The dependency's TestContext is
/// reachable through TestContext.Dependencies, so state can be handed over via its StateBag.
/// </summary>
public sealed class DependsOnTests
{
    private const string OrderIdKey = "OrderId";
    private const string OrderId = "order-4711";

    [Test]
    public void CreateOrder() => TestContext.Current!.StateBag[OrderIdKey] = OrderId;

    [Test]
    [DependsOn(nameof(CreateOrder))]
    public async Task PayOrder()
    {
        var createOrder = TestContext.Current!.Dependencies.GetTests(nameof(CreateOrder)).Single();

        await Assert.That(createOrder.Execution.Result?.State).IsEqualTo(TestState.Passed);
        await Assert.That(createOrder.StateBag[OrderIdKey]).IsEqualTo(OrderId);
    }

    [Test]
    [DependsOn(nameof(CreateOrder))]
    [DependsOn(nameof(PayOrder))]
    public async Task ShipOrder()
    {
        var dependencies = TestContext.Current!.Dependencies;

        await Assert.That(dependencies.GetTests(nameof(PayOrder)).Single().Execution.Result?.State).IsEqualTo(TestState.Passed);
        await Assert.That(dependencies.GetTests(nameof(CreateOrder)).Single().StateBag[OrderIdKey]).IsEqualTo(OrderId);
    }
}

/// <summary>[NotInParallel(Order = n)] runs the tests sharing a constraint key serially, in the given order.</summary>
public sealed class NotInParallelOrderTests
{
    private const string Pipeline = "Pipeline";

    private static readonly List<string> Executed = [];

    [Test]
    [NotInParallel(Pipeline, Order = 1)]
    public async Task Extract()
    {
        Executed.Add(nameof(Extract));

        await Assert.That(Executed).IsEquivalentTo([nameof(Extract)]);
    }

    [Test]
    [NotInParallel(Pipeline, Order = 2)]
    public async Task Transform()
    {
        Executed.Add(nameof(Transform));

        await Assert.That(Executed).IsEquivalentTo([nameof(Extract), nameof(Transform)]);
    }

    [Test]
    [NotInParallel(Pipeline, Order = 3)]
    public async Task Load()
    {
        Executed.Add(nameof(Load));

        await Assert.That(Executed).IsEquivalentTo([nameof(Extract), nameof(Transform), nameof(Load)]);
    }
}
