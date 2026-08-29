# TUnit.Mocks — source-generated mocking

[TUnit.Mocks](https://tunit.dev/docs/writing-tests/mocking/) generates mocks at compile time:
AOT/trimming-safe, no runtime proxies, and `IGreeter.Mock()` works as a C# 14 static extension
whose wrapper *is* the interface. Setups, matchers, and verification share one typed surface —
`mock.Greet(Any()).Returns("hi")` to stub, `mock.Greet("Alice").WasCalled(Times.Once)` to verify —
with matchers imported globally.

- `TUnit.Mocks 1.65.68/MockEssentials.cs` — first mock, loose-mode smart defaults, inline lambda
  matchers, argument capture, sequential `.Then()` setups, strict mode.
- `TUnit.Mocks 1.65.68/StatefulConnectionTests.cs` — state-machine mocking (`InState` /
  `TransitionsTo`), typed event raising, subscription tracking.
- `TUnit.Mocks 1.65.68/PendingTaskTests.cs` — **new in 1.62.0**: `Returns(async () => …)` on async
  members hands the task back still pending, so timeout and cancellation races become testable
  (1.63.0 extends the alias to net8.0 and .NET Framework).
- `TUnit.Mocks 1.65.68/RuntimeAutoStubTests.cs` — **new in 1.63.0**: loose mocks emit functional
  runtime stubs for interfaces the source generator cannot see (SDK-internal types), where the
  previous behavior was `null` → `NullReferenceException` inside the SDK.
- `TUnit.Mocks 1.65.68/InternalsAccessTests.cs` — **experimental in 1.63.0**: name, mock, and
  configure `internal` types of a referenced assembly with zero `InternalsVisibleTo` — beyond
  what runtime-proxy libraries can do.
- `TUnit.Mocks 1.65.68/StaticAbstractTests.cs` — **new in 1.65.0**: `T.Mock()` on an interface
  with `static abstract` members (the generator mocks a derived `…Mockable` view, so pass
  `.Object`), plus the **1.65.63** fix that keeps a non-generic method visible next to its
  same-named generic overload.
- `VendorSdk/` — tiny stand-in for a third-party SDK (public `IFeatureBag` seam, internal
  feature types) that the two showcases introduced in 1.63.0 drive end-to-end.

Run it with:

```bash
dotnet run --project "TUnit.Mocks 1.65.68/TUnit.Mocks 1.65.68/TUnit.Mocks 1.65.68.csproj"
```
