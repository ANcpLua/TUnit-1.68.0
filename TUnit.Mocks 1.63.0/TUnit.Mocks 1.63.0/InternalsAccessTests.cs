using TUnit.Mocks;
using VendorSdk;

namespace TUnit.Mocks_1._63._0;

public class InternalsAccessTests
{
    // Experimental in 1.63.0: IQuotaPolicy is internal to VendorSdk, which grants this test
    // assembly nothing. Listing VendorSdk under <TUnitMocksInternalsAccess> in the csproj
    // publicizes the compiler's view of it, so the type becomes nameable, source-generator
    // mocked, and fully configurable — runtime-proxy libraries can auto-substitute such
    // types at best, but they can never let a test configure one.
    [Test]
    public async Task Internal_sdk_policy_is_fully_mockable()
    {
        var policy = IQuotaPolicy.Mock();
        policy.Allow(Any()).Returns(false);

        var features = IFeatureBag.Mock();
        features.Get<IQuotaPolicy>().Returns(policy.Object);

        var admitted = QuotaGate.TryAdmit(features, "tenant-42");

        await Assert.That(admitted).IsFalse();
        policy.Allow("tenant-42").WasCalled(Times.Once);
    }

    [Test]
    public async Task Explicit_null_overrides_auto_mocking()
    {
        // Loose mocks auto-mock interface returns once a generated factory exists (naming
        // IQuotaPolicy.Mock() above created one) — handing the SDK a genuine "no policy
        // registered" answer therefore takes an explicit null setup.
        var features = IFeatureBag.Mock();
        features.Get<IQuotaPolicy>().Returns((IQuotaPolicy?)null);

        await Assert.That(QuotaGate.TryAdmit(features, "anyone")).IsTrue();
    }
}
