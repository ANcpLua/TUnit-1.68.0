# Changelog — TUnit API surface

Reverse API doc for this repository. One row per API member that has a **runnable, asserting example**
here; the file is the contract for what qyl-workspace may copy out and execute. Update it on every
version bump, in the same commit as the code.

Schema (fixed columns, one table per area, no merged cells):

`| API | Namespace | Kind | Example | Verified by | Since | Note |`

- `Kind` ∈ `attribute` · `interface` · `class` · `method` · `property` · `enum` · `config` · `cli`
- `Example` is the file that declares or applies the API; `Verified by` is the test that asserts the behaviour
- `Since` is the TUnit version that introduced or last changed the member (`≤1.61` = long-standing)
- All rows are verified by running the projects on the pinned version, not by reading the docs

## [1.65.68] — 2026-08-29

Packages: `TUnit 1.65.68` · `TUnit.Mocks 1.65.68` · `TUnit.Playwright 1.65.68` · `Microsoft.AspNetCore.Mvc.Testing 10.0.10` · SDK `10.0.400` · `net10.0`
Projects: `TUnit.Patterns 1.65.68` (163 pass, 1 intentional skip) · `TUnit.Mixed 1.65.68` (29) · `TUnit.Mocks 1.65.68` (15) · `TUnit 1.65.68` (1) · `AdvancedPatterns` (10 + 2 key-gated skips) · `TUnit.Playwright 1.65.68` (builds; needs browsers to run)

### Ordering and dependencies — `TUnit.Patterns 1.65.68/Ordering`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `DependsOnAttribute(string)` (repeatable) | `TUnit.Core` | attribute | `Ordering/DependsOnTests.cs` | `DependsOnTests.ShipOrder` | ≤1.61 | |
| `TestContext.Dependencies.GetTests(string)` | `TUnit.Core` | method | `Ordering/DependsOnTests.cs` | `DependsOnTests.PayOrder` | ≤1.61 | returns all invocations of the dependency |
| `TestContext.StateBag` | `TUnit.Core` | property | `Ordering/DependsOnTests.cs` | `DependsOnTests.PayOrder` | ≤1.61 | hand-over between dependent tests |
| `TestContext.Execution.Result.State` / `TestState` | `TUnit.Core` | property | `Ordering/DependsOnTests.cs` | `DependsOnTests.ShipOrder` | ≤1.61 | |
| `NotInParallelAttribute(string key) { Order }` | `TUnit.Core` | attribute | `Ordering/DependsOnTests.cs` | `NotInParallelOrderTests` | ≤1.61 | serial, ordered, only among the key |

### Parallelism — `TUnit.Patterns 1.65.68/Parallelism`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ParallelLimiterAttribute<T>` + `IParallelLimit.Limit` | `TUnit.Core` / `TUnit.Core.Interfaces` | attribute, interface | `Parallelism/ParallelismTests.cs` | `ParallelLimiterTests` (peak ≤ 2 over 10 runs) | ≤1.61 | limit shared by every test naming the type |
| `RepeatAttribute(int)` | `TUnit.Core` | attribute | `Parallelism/ParallelismTests.cs` | `ParallelLimiterTests` | ≤1.61 | `(RepeatIndex: n)` display suffix |
| `NotInParallelAttribute(string key)` | `TUnit.Core` | attribute | `Parallelism/ParallelismTests.cs` | `NotInParallelKeyTests` (holders == 1) | ≤1.61 | |

### Retry — `TUnit.Patterns 1.65.68/Retry`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `RetryAttribute.ShouldRetry(TestContext, Exception, int)` | `TUnit.Core` | method | `Retry/RetryOnTransientAttribute.cs` | `RetryTests.Custom_policy_retries_transient_failures_until_they_pass` | ≤1.61 | |
| `RetryAttribute.RetryOnExceptionTypes` | `TUnit.Core` | property | `Retry/RetryTests.cs` | `RetryTests.Built_in_policy_filters_by_exception_type` | 1.6x | `IsAssignableFrom` match |
| `RetryAttribute.BackoffMs` / `BackoffMultiplier` | `TUnit.Core` | property | `Retry/RetryTests.cs` | same | 1.6x | exponential backoff |
| `TestContext.Execution.CurrentRetryAttempt` | `TUnit.Core.Interfaces.ITestExecution` | property | `Retry/RetryTests.cs` | `RetryTests` | 1.6x | 0-based |
| `TestContext.Execution.RetryAttempts` | `TUnit.Core.Interfaces.ITestExecution` | property | `Retry/RetryTests.cs` | `RetryTests` | 1.6x | failed prior attempts, empty when none |

### Data sources — `TUnit.Patterns 1.65.68/Data`, `TUnit.Mixed 1.65.68`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `TestDataRow<T>(value, DisplayName:, Skip:, Categories:)` | `TUnit.Core` | class | `Data/DataTests.cs` | `TestDataRowTests` (`Admin login`, one skip, `[Category=Security]` filter) | ≤1.61 | tuple `T` is spread across parameters |
| `TestContext.Metadata.DisplayName` | `TUnit.Core` | property | `Data/DataTests.cs`, `Context/ContextTests.cs` | `TestDataRowTests`, `DisplayNameTests` | ≤1.61 | docs' `GetDisplayName()` does not exist |
| `DisplayNameAttribute("$param …")` | `TUnit.Core` | attribute | `Data/DataTests.cs` | `DisplayNameTests` (`1+2 equals 3`) | ≤1.61 | |
| `ArgumentDisplayFormatter.CanHandle/FormatValue` + `ArgumentDisplayFormatterAttribute<T>` | `TUnit.Core` | class, attribute | `Data/SumFormatter.cs` | `DisplayNameTests` | ≤1.61 | composes with `$param` |
| `CombinedDataSourcesAttribute` + per-parameter `[Arguments]` / `[MethodDataSource]` | `TUnit.Core` | attribute | `Data/DataTests.cs` | `CombinedDataSourceTests` (8 cases) | ≤1.61 | Cartesian product |
| `MethodDataSourceAttribute.DeferEnumeration` | `TUnit.Core` | property | `Data/DataTests.cs` | `DeferredEnumerationTests` (`--list-tests` = 1 node) | 1.6x | rows nest under a placeholder |
| `AsyncDataSourceGeneratorAttribute<T1, T2>.GenerateDataSourcesAsync` | `TUnit.Core` | class | `Data/FibonacciAttribute.cs` | `AsyncGeneratorTests` | ≤1.61 | runs at discovery |
| `UntypedDataSourceGeneratorAttribute.GenerateDataSources` | `TUnit.Core` | class | `Data/BatchesAttribute.cs` | `TrailingArrayTests` (`(batch, 1, 2, 3)`) | ≤1.61 | |
| `object[]` → trailing `int[]` element-wise conversion | engine | — | `Data/DataTests.cs` | `TrailingArrayTests` | 1.65.68 | fix #6681 |
| `ArgumentsAttribute` (method and class level) | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/DataDrivenTests.cs` | `DataDrivenTests`, `ClassLevelArgumentTests` | ≤1.61 | |
| `MethodDataSourceAttribute` (tuple rows) | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/DataDrivenTests.cs` | `DataDrivenTests.Subtract_WithMethodDataSource` | ≤1.61 | |
| `DataSourceGeneratorAttribute<T1, T2, T3>` | `TUnit.Core` | class | `TUnit.Mixed 1.65.68/Data/AdditionDataGenerator.cs` | `DataDrivenTests.Add_WithCustomDataGenerator` | ≤1.61 | |
| `MatrixDataSourceAttribute` + `MatrixAttribute` | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/DataDrivenTests.cs` | `DataDrivenTests.Multiply_AllCombinations` | ≤1.61 | |

### Extension points — `TUnit.Patterns 1.65.68/Extensions`, `TUnit.Mixed 1.65.68/CancellationTests.cs`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ITestExecutor.ExecuteTest(TestContext, Func<ValueTask>)` + `TestExecutorAttribute<T>` | `TUnit.Core.Interfaces` / `TUnit.Core` | interface, attribute | `Extensions/ScopedCultureExecutor.cs` | `TestExecutorTests.Executor_establishes_the_ambient_culture` | ≤1.61 | |
| `CultureAttribute(string)` | `TUnit.Core.Executors` | attribute | `Extensions/ExtensionTests.cs` | `TestExecutorTests.Built_in_culture_attribute_does_the_same` | ≤1.61 | |
| `ITestStartEventReceiver` / `ITestEndEventReceiver` on an attribute | `TUnit.Core.Interfaces` | interface | `Extensions/StopwatchAttribute.cs` | `EventReceiverTests` | ≤1.61 | `Order` required |
| `EventReceiverStage.Early` | `TUnit.Core.Enums` | enum | `Extensions/StopwatchAttribute.cs` | `EventReceiverTests` (start before `[Before(Test)]`, end before `[After(Test)]`) | 1.6x | |
| `SkipAttribute.ShouldSkip(TestRegisteredContext)` | `TUnit.Core` | method | `Extensions/SkipOnAttribute.cs` | `SkipTests` | ≤1.61 | registration-time decision |
| `[Before(Test)]` / `[After(Test)]` with `TestContext` parameter | `TUnit.Core` | attribute | `Extensions/ExtensionTests.cs`, `TUnit.Mixed 1.65.68/BasicTests.cs` | `EventReceiverTests` | ≤1.61 | |
| `[Before(Class)]` / `[After(Class)]` (`ClassHookContext`), `[Before(TestSession)]` / `[After(TestSession)]` (`TestSessionContext`) | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/BasicTests.cs`, `HooksAndLifecycle.cs` | run of `TUnit.Mixed` | ≤1.61 | |
| `TestContext.Execution.Cancel()` | `TUnit.Core` | method | `TUnit.Mixed 1.65.68/CancellationTests.cs` | `PerTestCancellationShowcase` (`[Explicit]`) | 1.64.0 | per-test cancellation |
| `TestContext.Execution.AddLinkedCancellationToken` | `TUnit.Core` | method | `TUnit.Mixed 1.65.68/CancellationTests.cs` | `BeforeHookLinkedCancellationShowcase`, `ExecutorLinkedCancellationShowcase` | 1.64.0 | honoured from hooks and executors |
| `TimeoutAttribute(int)` + injected `CancellationToken` | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/CancellationTests.cs` | same | ≤1.61 | |
| `ExplicitAttribute` | `TUnit.Core` | attribute | `TUnit.Mixed 1.65.68/CancellationTests.cs` | opt-in run | ≤1.61 | |

### Assertions — `TUnit.Patterns 1.65.68/Assertions`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `GenerateAssertionAttribute(ExpectationMessage = "… {param}")` on `bool` / `AssertionResult` methods | `TUnit.Assertions.Attributes` | attribute | `Assertions/OrderAssertions.cs` | `AssertionTests` | ≤1.61 | placeholders substituted |
| `AssertionResult.Passed` / `Failed(string)` | `TUnit.Assertions.Core` | class | `Assertions/OrderAssertions.cs` | `AssertionTests.Generated_assertion_reports_its_custom_failure` | ≤1.61 | |
| `AssertionFromAttribute<T>(Type, string, CustomName, NegateLogic, ExpectationMessage)` | `TUnit.Assertions.Attributes` | attribute | `Assertions/OrderAssertions.cs` | `AssertionTests.Lifted_assertions_and_their_negation` | ≤1.61 | `{param}` placeholders NOT substituted — keep literal |
| `.And` / `.Or` chaining on generated assertions | `TUnit.Assertions` | — | `Assertions/AssertionTests.cs` | `AssertionTests.Generated_assertions_chain`, `Or_short_circuits_on_the_first_pass` | ≤1.61 | |
| `Assert.That(Func<Task>).Throws<T>()` → `AssertionException` | `TUnit.Assertions.Exceptions` | method | `Assertions/AssertionTests.cs` | same | ≤1.61 | |
| `.All().Satisfy(x => x.IsEqualTo(...))` | `TUnit.Assertions` | method | `Retry/RetryTests.cs` | `RetryTests` | ≤1.61 | lambda receives an `IAssertionSource<T>` |

### Context and artifacts — `TUnit.Patterns 1.65.68/Context`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `PropertyAttribute(name, value)` + `Metadata.TestDetails.CustomProperties` | `TUnit.Core` | attribute, property | `Context/ContextTests.cs` | `ContextTests.Custom_properties_are_readable_at_run_time`, `--treenode-filter "/*/*/*/*[Owner=platform]"` | ≤1.61 | |
| `TestContext.Isolation.UniqueId` / `GetIsolatedName` | `TUnit.Core.Interfaces.ITestIsolation` | property, method | `Context/ContextTests.cs` | `ContextTests.Isolation_helpers_produce_unique_resource_names` | 1.6x | `Test_{id}_{name}` |
| `TestContext.ResultsDirectory` (static) | `TUnit.Core` | property | `Context/ContextTests.cs` | `ContextTests.Artifacts_land_in_the_results_directory` | 1.64.6 | honours `--results-directory` |
| `TestContext.Output.AttachArtifact(path, displayName:, description:)` | `TUnit.Core.Interfaces.ITestOutput` | method | `Context/ContextTests.cs` | same (artifact listed in run output) | ≤1.61 | |
| `TestContext.Metadata.TestDetails.TestId` | `TUnit.Core` | property | (used during development) | — | ≤1.61 | stable per test case |

### Dynamic tests — `TUnit.Patterns 1.65.68/Dynamic`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `DynamicTestBuilderAttribute` + `DynamicTestBuilderContext.AddTest` | `TUnit.Core` | attribute, method | `Dynamic/DynamicTests.cs` | `DynamicTests.Greets` ×3 | ≤1.61 | |
| `DynamicTest<T> { TestMethod, TestMethodArguments, Attributes }` + `DynamicTestHelper.Argument<T>()` | `TUnit.Core` | class | `Dynamic/DynamicTests.cs` | same | ≤1.61 | lambda is an expression, not a delegate |

### Fixtures and injection — `TUnit.Patterns 1.65.68/Fixtures`, `TUnit.Mixed 1.65.68`, `TUnit 1.65.68`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ClassDataSourceAttribute<T>` on a `required` property (test class and fixture) | `TUnit.Core` | attribute | `Fixtures/Fixtures.cs`, `Fixtures/FixtureTests.cs` | `NestedInjectionTests.Nested_property_is_initialized_before_its_owner` | ≤1.61 | dependency-ordered init |
| `SharedType.Keyed` + `Key` | `TUnit.Core` | enum, property | `Fixtures/Fixtures.cs` | `NestedInjectionTests.Keyed_sharing_hands_out_the_same_instance` | ≤1.61 | same instance across property and parameter injection |
| `SharedType.PerClass` / `PerTestSession` | `TUnit.Core` | enum | `TUnit.Mixed 1.65.68/DependencyInjectionTests.cs`, `TUnit 1.65.68/Tests.cs` | run of those projects | ≤1.61 | |
| `IAsyncInitializer` / `IAsyncDisposable` on fixtures | `TUnit.Core.Interfaces` | interface | `Fixtures/Fixtures.cs`, `TUnit.Mixed 1.65.68/Data/InMemoryDb.cs` | `NestedInjectionTests` | ≤1.61 | |
| `IAsyncDiscoveryInitializer` + `InstanceMethodDataSourceAttribute` | `TUnit.Core.Interfaces` / `TUnit.Core` | interface, attribute | `Fixtures/Fixtures.cs`, `Fixtures/FixtureTests.cs` | `DiscoveryInitializerTests` (2 rows discovered) | 1.6x | discovery-time init |
| `WebApplicationFactory<Program>` as `ClassDataSource` + `IAsyncInitializer` | `Microsoft.AspNetCore.Mvc.Testing` | class | `TUnit 1.65.68/TUnit 1.65.68/WebApplicationFactory.cs` | `Tests.Test` | ≤1.61 | |

### TUnit.Mocks — `TUnit.Mocks 1.65.68`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `T.Mock()` static extension; wrapper *is* the interface | `TUnit.Mocks` | method | `MockEssentials.cs` | `MockEssentials.Known_user_gets_a_welcome_mail` | 1.6x | C# 14 static extension |
| `Returns(value)` / `Any()` / `Any<T>()` / inline lambda matchers | `TUnit.Mocks` | method | `MockEssentials.cs` | `MockEssentials.Inline_lambdas_are_argument_matchers` | 1.6x | |
| captured matcher `.Values` / `.Latest` | `TUnit.Mocks` | property | `MockEssentials.cs` | `MockEssentials.Captured_arguments_are_inspectable` | 1.6x | |
| `.Throws<T>().Then().Returns(...)` | `TUnit.Mocks` | method | `MockEssentials.cs` | `MockEssentials.Sequential_setups_model_flaky_dependencies` | 1.6x | |
| `WasCalled(Times.Once)` / `WasNeverCalled()` | `TUnit.Mocks` | method | `MockEssentials.cs` | `MockEssentials` | 1.6x | |
| `MockBehavior.Strict` → `MockStrictBehaviorException` | `TUnit.Mocks` / `TUnit.Mocks.Exceptions` | enum, class | `MockEssentials.cs` | `MockEssentials.Strict_mocks_reject_surprise_calls` | 1.6x | |
| `SetState` / `InState` / `TransitionsTo` | `TUnit.Mocks` | method | `StatefulConnectionTests.cs` | `StatefulConnectionTests.Status_follows_the_connection_state_machine` | 1.6x | |
| `.Raises{Event}(args)` / `Raise{Event}(args)` / `Events.{Event}.WasSubscribed` / `SubscriberCount` | `TUnit.Mocks` | method, property | `StatefulConnectionTests.cs` | `StatefulConnectionTests` | 1.6x | generated per event |
| `Returns(async () => …)` keeps the task pending | `TUnit.Mocks` | method | `PendingTaskTests.cs` | `PendingTaskTests.Caller_timeout_beats_a_hanging_feed` | 1.62.0 | net8.0/.NET Framework in 1.63.0 |
| runtime auto-stubs for ungenerated interfaces | `TUnit.Mocks` | — | `RuntimeAutoStubTests.cs` | `RuntimeAutoStubTests` | 1.63.0 | reuses `DynamicProxyGenAssembly2` identity |
| `<TUnitMocksExperimentalInternalsAccess>` + `<TUnitMocksInternalsAccess Include>` | MSBuild | config | `TUnit.Mocks 1.65.68.csproj`, `InternalsAccessTests.cs` | `InternalsAccessTests` | 1.63.0 (experimental) | |
| `T.Mock()` on interfaces with `static abstract` members → `Mock<TMockable>`; pass `.Object` | `TUnit.Mocks` | method | `StaticAbstractTests.cs` | `StaticAbstractTests` | 1.65.0 | wrapper is not the interface here |
| non-generic method next to same-named generic overload | generator | — | `StaticAbstractTests.cs` | `StaticAbstractTests` | 1.65.63 | fix |

### TUnit.Playwright — `TUnit.Playwright 1.65.68` (builds; not executed in this bump)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `PageTest` base + `Page` / `Expect(...)` | `TUnit.Playwright` | class | `Tests.cs` | — | ≤1.61 | |
| `PageFixture` via `[ClassDataSource<PageFixture>]` ×2 (shared browser, isolated contexts) | `TUnit.Playwright` | class | `TwoContextFixtureTests.cs` | — | ≤1.61 | |
| `Microsoft.Playwright.Program.Main(["install"])` in `[Before(TestSession)]` | `Microsoft.Playwright` | method | `Hooks.cs` | — | ≤1.61 | |

### Present in 1.65.68, no example yet

`ParallelGroupAttribute` · `[assembly: NotInParallel]` / `[assembly: ParallelLimiter<T>]` · `DependsOnAttribute<T>` · `DependsOn(ProceedOnFailure = true)` · `IHookExecutor` / `HookExecutorAttribute<T>` · `ITestRegisteredEventReceiver.SetTestExecutor/SetHookExecutor` · `ITestDiscoveryEventReceiver` · `ILastTestInClass|Assembly|TestSessionEventReceiver` · `DisplayNameFormatterAttribute` · `MethodDataSourceAttribute<T>` · `TypedDataSourceAttribute<T>` · `IKeyedDataSource` · `TestContext.Parameters` (`--test-parameter`) · `TestSessionContext.AddArtifact` · `Skip.Test(reason)` / `SkipTestException` / `InconclusiveTestException` · `TestBuilderContext.Current` · `[Timeout]` + `[Retry]` interplay · `TUnit.AspNetCore` · Native AOT publish

### Divergences from docs (verified on 1.65.68)

- `ITestRetryEventReceiver.OnTestRetry` is declared in `TUnit.Core` but nothing in the engine invokes it; read `Execution.RetryAttempts` / `CurrentRetryAttempt` instead.
- `[AssertionFrom]` does not substitute `{parameter}` placeholders in `ExpectationMessage` (only `[GenerateAssertion]` does); `nameof(string.StartsWith)` also fails to generate because of its overloads — point it at a single-overload static helper.
- `TestContext.GetDisplayName()` does not exist; use `TestContext.Current.Metadata.DisplayName`.
- `EventReceiverStage.Early` moves the **end** receiver before `[After(Test)]` hooks as well, not only the start receiver before `[Before(Test)]`.
- The analyzer rejects typed tuple data sources whose element type is `object[]` for an `int[]` parameter (TUnit0001); the 1.65.68 element-wise conversion is reachable through an untyped generator.
