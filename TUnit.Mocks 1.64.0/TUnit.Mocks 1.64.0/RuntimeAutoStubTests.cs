using VendorSdk;

namespace TUnit.Mocks_1._64._0;

public class RuntimeAutoStubTests
{
    // New in 1.63.0: when SDK-internal code asks a loose mock for an interface the source
    // generator produced no mock for — here Get<ITelemetryChannel>(), closed inside
    // VendorSdk over a type this file never names — the engine emits a functional stub at
    // runtime instead of returning null. Stub defaults match runtime-proxy libraries:
    // completed tasks, "" strings, empty collections, recursive stubs.
    //
    // The stub assembly reuses Castle DynamicProxy's identity (DynamicProxyGenAssembly2),
    // so VendorSdk's existing NSubstitute/Moq InternalsVisibleTo grant is all it needs.
    // Before 1.63.0 this exact call chain died in a NullReferenceException inside the SDK.
    [Test]
    public async Task Sdk_internal_feature_lookup_gets_a_working_stub()
    {
        var features = IFeatureBag.Mock();

        var endpoint = await TelemetryPipeline.FlushAsync(features);

        await Assert.That(endpoint).IsEmpty();
    }
}
