# TUnit.Mocks — source-generated mocking

[TUnit.Mocks](https://tunit.dev/docs/writing-tests/mocking/) generates mocks at compile time:
AOT/trimming-safe, no runtime proxies, and `IGreeter.Mock()` works as a C# 14 static extension
whose wrapper *is* the interface. Setups, matchers, and verification share one typed surface —
`mock.Greet(Any()).Returns("hi")` to stub, `mock.Greet("Alice").WasCalled(Times.Once)` to verify —
with matchers imported globally.

- `TUnit.Mocks 1.68.17/MockEssentials.cs` — first mock, loose-mode smart defaults, inline lambda
  matchers, argument capture, sequential `.Then()` setups, strict mode.
- `TUnit.Mocks 1.68.17/StatefulConnectionTests.cs` — state-machine mocking (`InState` /
  `TransitionsTo`), typed event raising, subscription tracking.
- `TUnit.Mocks 1.68.17/PendingTaskTests.cs` — **new in 1.62.0**: `Returns(async () => …)` on async
  members hands the task back still pending, so timeout and cancellation races become testable
  (1.63.0 extends the alias to net8.0 and .NET Framework).
- `TUnit.Mocks 1.68.17/RuntimeAutoStubTests.cs` — **new in 1.63.0**: loose mocks emit functional
  runtime stubs for interfaces the source generator cannot see (SDK-internal types), where the
  previous behavior was `null` → `NullReferenceException` inside the SDK.
- `TUnit.Mocks 1.68.17/InternalsAccessTests.cs` — **experimental in 1.63.0**, still experimental
  in 1.68.17: name, mock, and configure `internal` types of a referenced assembly with zero
  `InternalsVisibleTo` — beyond what runtime-proxy libraries can do. The opt-in is two lines in
  *this* csproj (`<TUnitMocksExperimentalInternalsAccess>` plus a
  `<TUnitMocksInternalsAccess Include="VendorSdk"/>` item); dropping either one turns the test into
  `CS0122`.
- `TUnit.Mocks 1.68.17/DesignTimeReferenceTests.cs` — **fixed in 1.68.17** (#6836): up to 1.68.0
  an editor kept `VendorSdk` as a live project reference next to the publicized copy, the project
  won, and `InternalsAccessTests` was red with `CS0122` while `dotnet build` was clean. Design-time
  builds now set `ReferenceOutputAssembly=false` on that reference; the test runs `dotnet msbuild`
  on this project and checks it for a design-time and a real build.
- `TUnit.Mocks 1.68.17/InitOnlyMemberTests.cs` — **fixed in 1.68.17** (#6833): interfaces and
  classes with `init` properties or indexers can be mocked; on 1.68.0 the generated mock did not
  compile (CS8853 / CS8854 / CS8855). An unconfigured virtual `init` property keeps its base value.
- `TUnit.Mocks 1.68.17/WrapAndMockTests.cs` — **fixed in 1.68.17** (#6835): one type used through
  both `T.Mock()` and `Mock.Wrap(instance)` in one project; on 1.68.0 the generator aborted
  (CS8785) and every mock in the project broke with it. Also the showcase's `Mock.Wrap` example:
  unconfigured calls reach the real instance and are still verifiable.
- `TUnit.Mocks 1.68.17/StaticAbstractTests.cs` — **new in 1.65.0**: `T.Mock()` on an interface
  with `static abstract` members (the generator mocks a derived `…Mockable` view, so pass
  `.Object`), plus the **1.65.63** fix that keeps a non-generic method visible next to its
  same-named generic overload.
- `TUnit.Mocks 1.68.17/ConstructorCallbackTests.cs` — **fixed in 1.66.27**: mocking a class whose
  base constructor calls virtual or abstract members no longer throws `NullReferenceException`;
  the engine exists before `base(...)` runs, so constructor-time calls are recorded and verifiable.
- `TUnit.Mocks 1.68.17/RefStructEventTests.cs` — **fixed in 1.68.0**: events whose arguments are
  `ref struct`s (`EventHandler<TRefStruct>`, `ReadOnlySpan<T>`) or passed by `ref` / `in` / `out`
  are raised typed, without boxing; on 1.67.0 the mock did not compile. There is no deferred
  `.Raises{Event}(args)` for them — create the argument inside a `.Callback(...)` instead.
- `VendorSdk/` — tiny stand-in for a third-party SDK (public `IFeatureBag` seam, internal
  feature types) that the two showcases introduced in 1.63.0 drive end-to-end.

Run it with:

```bash
dotnet run --project "TUnit.Mocks 1.68.17/TUnit.Mocks 1.68.17/TUnit.Mocks 1.68.17.csproj"
```
