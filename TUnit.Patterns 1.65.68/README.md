# TUnit.Patterns — the extensibility surface, end to end

Runnable, self-verifying examples of the patterns behind TUnit's
[docs](https://tunit.dev/docs/intro), pinned to TUnit 1.65.68. Every test asserts the behaviour
it demonstrates (display names, execution order, retry history, receiver timing), and every string
that matters is a `const`.

| Folder | Pattern | API |
|---|---|---|
| `Ordering/` | Test dependencies with state hand-over; serial ordered steps | `[DependsOn]`, `TestContext.Dependencies.GetTests`, `StateBag`, `[NotInParallel(key, Order = n)]` |
| `Parallelism/` | Concurrency cap shared by every test naming the limiter; mutual exclusion by key | `[ParallelLimiter<T>]` + `IParallelLimit`, `[NotInParallel(key)]`, `[Repeat]` |
| `Retry/` | Custom retry policy via `ShouldRetry`; built-in type filter and backoff; per-attempt history | `RetryAttribute`, `RetryOnExceptionTypes`, `BackoffMs`, `Execution.CurrentRetryAttempt`, `Execution.RetryAttempts` |
| `Data/` | Row metadata (display name, skip, category); `$parameter` templates + argument formatters; Cartesian mixing of sources; deferred enumeration; async typed generator; untyped generator feeding a trailing array | `TestDataRow<T>`, `[DisplayName]`, `[ArgumentDisplayFormatter<T>]`, `[CombinedDataSources]`, `DeferEnumeration`, `AsyncDataSourceGeneratorAttribute<T1,T2>`, `UntypedDataSourceGeneratorAttribute` |
| `Extensions/` | Ambient scope around the test body; event receivers on attributes with stage control; registration-time conditional skip | `ITestExecutor` + `[TestExecutor<T>]`, `[Culture]`, `ITestStartEventReceiver` / `ITestEndEventReceiver` + `EventReceiverStage.Early`, `SkipAttribute.ShouldSkip` |
| `Assertions/` | Source-generated assertions from predicates and from existing methods, chained with `And` / `Or` | `[GenerateAssertion]`, `[AssertionFrom<T>]`, `AssertionResult` |
| `Context/` | Custom properties, isolation helpers, artifacts in the platform results directory | `[Property]`, `Isolation.GetIsolatedName`, `TestContext.ResultsDirectory` (1.64.6), `Output.AttachArtifact` |
| `Dynamic/` | Tests assembled in code | `[DynamicTestBuilder]`, `DynamicTest<T>`, `DynamicTestHelper.Argument<T>()` |
| `Fixtures/` | Nested property injection with dependency-ordered async init; keyed sharing; discovery-time initialization feeding an instance data source | `[ClassDataSource<T>]` on properties, `SharedType.Keyed`, `IAsyncInitializer`, `IAsyncDiscoveryInitializer`, `[InstanceMethodDataSource]` |

Version-specific notes, verified against 1.65.68 rather than the docs:

- `Data/TrailingArrayTests.cs` exercises the 1.65.68 fix (#6681): an `object[]` produced by an
  untyped data source is converted element-wise into an `int[]` parameter instead of being boxed
  as a single element.
- `[AssertionFrom]` copies `{parameter}` placeholders verbatim into the generated expectation
  message — only `[GenerateAssertion]` substitutes them — so those messages are kept literal.
- `ITestRetryEventReceiver` exists in `TUnit.Core` but nothing in the engine raises it; retry
  history is read from `TestContext.Execution.RetryAttempts` instead.
- `TestDataRowTests` deliberately skips one row (`Skip:` metadata), so a full run reports one skip.

Run it with:

```bash
dotnet run --project "TUnit.Patterns 1.65.68/TUnit.Patterns 1.65.68.csproj"
```
