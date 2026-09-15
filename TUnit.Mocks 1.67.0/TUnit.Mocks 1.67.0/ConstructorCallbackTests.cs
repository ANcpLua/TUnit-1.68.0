using TUnit.Mocks;

namespace TUnit.Mocks_1._67._0;

/// <summary>Template-method base class: the constructor calls overridable members to load its initial state.</summary>
public abstract class ConfigurationSource
{
    protected ConfigurationSource()
    {
        Connect();
        InitialRetries = ReadRetries();
    }

    public int InitialRetries { get; }

    public virtual void Connect()
    {
    }

    public abstract int ReadRetries();
}

public class ConstructorCallbackTests
{
    private const int ConfiguredRetries = 3;

    // Fixed in 1.66.27: a class mock's generated constructor assigned its engine only after base(...) returned,
    // so a base constructor calling an overridden member threw NullReferenceException on creation. The engine
    // now exists before the base constructor runs, and those calls are recorded like any other.
    [Test]
    public async Task Base_constructor_calls_are_recorded_on_the_mock()
    {
        var source = ConfigurationSource.Mock();

        source.Connect().WasCalled(Times.Once);
        source.ReadRetries().WasCalled(Times.Once);

        // Construction ran under loose defaults; setups made afterwards apply to later calls.
        await Assert.That(source.Object.InitialRetries).IsEqualTo(0);
        source.ReadRetries().Returns(ConfiguredRetries);
        await Assert.That(source.Object.ReadRetries()).IsEqualTo(ConfiguredRetries);
        source.ReadRetries().WasCalled(Times.Exactly(2));
    }
}
