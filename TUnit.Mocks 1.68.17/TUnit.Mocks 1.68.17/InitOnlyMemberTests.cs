using TUnit.Mocks;

namespace TUnit.Mocks_1._68._17;

/// <summary>Settings snapshot: values are fixed at construction through <c>init</c> accessors.</summary>
public interface ITenantSettings
{
    string TenantId { get; init; }

    string this[string key] { get; init; }
}

public class TenantDefaults
{
    public virtual int MaxUsers { get; init; } = 25;
}

public class InitOnlyMemberTests
{
    // Fixed in 1.68.17 (#6833): the generator emitted every `init` accessor as a plain `set`, so mocking a type
    // with an init-only property or indexer did not compile (CS8854 / CS8855) and there was no workaround on the
    // consumer side. Both are now emitted as `init` and configure like any other member.
    [Test]
    public async Task Init_only_property_and_indexer_are_configurable()
    {
        var settings = ITenantSettings.Mock();
        settings.TenantId.Returns("acme");
        settings.Item("region").Returns("eu-west");

        await Assert.That(settings.Object.TenantId).IsEqualTo("acme");
        await Assert.That(settings.Object["region"]).IsEqualTo("eu-west");
        settings.TenantId.Setter.WasNeverCalled();
    }

    [Test]
    public async Task Unconfigured_virtual_init_property_keeps_the_base_value()
    {
        var defaults = TenantDefaults.Mock();

        await Assert.That(defaults.Object.MaxUsers).IsEqualTo(25);

        defaults.MaxUsers.Returns(100);

        await Assert.That(defaults.Object.MaxUsers).IsEqualTo(100);
    }
}
