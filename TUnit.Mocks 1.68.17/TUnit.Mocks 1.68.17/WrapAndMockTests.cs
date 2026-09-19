using TUnit.Mocks;

namespace TUnit.Mocks_1._68._17;

public class PriceCalculator
{
    public virtual decimal Net(decimal gross) => Math.Round(gross / 1.19m, 2);
}

public class WrapAndMockTests
{
    // Fixed in 1.68.17 (#6835): when one compilation reached a type through both T.Mock() and Mock.Wrap(...), the
    // generator produced two sources with the same hint name and aborted (CS8785). Every mock in the project
    // vanished with it, so the build failed with CS1061 on unrelated tests. Both forms now coexist.
    [Test]
    public async Task One_type_is_stubbed_and_wrapped_in_the_same_compilation()
    {
        var stub = PriceCalculator.Mock();
        stub.Net(Any()).Returns(1m);

        // Mock.Wrap forwards unconfigured calls to the real instance and records them.
        var spy = Mock.Wrap(new PriceCalculator());

        await Assert.That(stub.Object.Net(119m)).IsEqualTo(1m);
        await Assert.That(spy.Object.Net(119m)).IsEqualTo(100m);
        spy.Net(119m).WasCalled(Times.Once);
    }
}
