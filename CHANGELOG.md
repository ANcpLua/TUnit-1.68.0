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

## [1.68.0] — 2026-09-18

Packages: `TUnit 1.68.0` · `TUnit.Mocks 1.68.0` · `TUnit.Playwright 1.68.0` · `Microsoft.AspNetCore.Mvc.Testing 10.0.12` · SDK `10.0.401` · `net10.0`
Projects: `TUnit.Patterns 1.68.0` (204 pass, 2 intentional skips) · `TUnit.Patterns.Policies 1.68.0` (11; also published Native AOT) · `TUnit.Mixed 1.68.0` (30) · `TUnit.Mocks 1.68.0` (20) · `TUnit 1.68.0` + `TUnit.AspNetCore` (5) · `AdvancedPatterns` (10 + 2 key-gated skips) · `TUnit.Playwright 1.68.0` (6; browsers installed on first run)
Extra packages: `TUnit.OpenTelemetry 1.68.0` · `TUnit.Logging.Microsoft 1.68.0` · `TUnit.AspNetCore 1.68.0` · `OpenTelemetry.Exporter.InMemory 1.18.0` · `Microsoft.Extensions.Diagnostics.Testing 10.9.0` · `Microsoft.Extensions.Logging 10.0.12`
No other dependency moves: 1.68.0 builds against the same `Microsoft.*` 10.0.12 packages (0 warnings).

### Since 1.67.0 — 1.68.0

New public API (diff of the `TUnit.PublicAPI` snapshots; only `TUnit.Playwright` changed):

- `RecordVideoAttribute(string path = "playwright-artifacts", int width = 1280, int height = 1400)` with `Path` / `Width` / `Height` (#6799) — see *TUnit.Playwright*

Behaviour changes with an example; each fails on 1.67.0 and passes on 1.68.0:

- mocks of interfaces whose events take `ref struct` or `ref` / `in` / `out` arguments (#6814) → `RefStructEventTests` (on 1.67.0 the generated mock does not compile: CS0030 / CS1620)
- `TUnit0023` no longer reports members disposed through a cast (#6818) → `CastDisposalShowcase` (on 1.67.0 the build warns twice)

`TUnit.Playwright` is now executed, not only built: `Hooks.cs` installs the browsers, 6/6 pass.

Not shown: docs on thread-pool use in parallel tests (#6817) · issue-template change (#6795).

### Ordering and dependencies — `TUnit.Patterns 1.68.0/Ordering`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `DependsOnAttribute(string)` (repeatable) | `TUnit.Core` | attribute | `Ordering/DependsOnTests.cs` | `DependsOnTests.ShipOrder` | ≤1.61 | |
| `TestContext.Dependencies.GetTests(string)` | `TUnit.Core` | method | `Ordering/DependsOnTests.cs` | `DependsOnTests.PayOrder` | ≤1.61 | returns all invocations of the dependency |
| `TestContext.StateBag` | `TUnit.Core` | property | `Ordering/DependsOnTests.cs` | `DependsOnTests.PayOrder` | ≤1.61 | hand-over between dependent tests |
| `TestContext.Execution.Result.State` / `TestState` | `TUnit.Core` | property | `Ordering/DependsOnTests.cs` | `DependsOnTests.ShipOrder` | ≤1.61 | |
| `NotInParallelAttribute(string key) { Order }` | `TUnit.Core` | attribute | `Ordering/DependsOnTests.cs` | `NotInParallelOrderTests` | ≤1.61 | serial, ordered, only among the key |
| `DependsOnAttribute<TClass>(string)` | `TUnit.Core` | attribute | `Ordering/DependsOnTests.cs` | `CrossClassDependsOnTests` | ≤1.61 | cross-class dependency |
| `DependsOnAttribute.ProceedOnFailure` + `Dependencies.GetTests(...).Execution.Result.Exception` | `TUnit.Core` | property | `Ordering/DependsOnTests.cs` | `ProceedOnFailureShowcase` (`[Explicit]`, run via tree-node filter: 1 failed, 1 passed) | ≤1.61 | |

### Parallelism — `TUnit.Patterns 1.68.0/Parallelism`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ParallelLimiterAttribute<T>` + `IParallelLimit.Limit` | `TUnit.Core` / `TUnit.Core.Interfaces` | attribute, interface | `Parallelism/ParallelismTests.cs` | `ParallelLimiterTests` (peak ≤ 2 over 10 runs) | ≤1.61 | limit shared by every test naming the type |
| `RepeatAttribute(int)` | `TUnit.Core` | attribute | `Parallelism/ParallelismTests.cs` | `ParallelLimiterTests` | ≤1.61 | `(RepeatIndex: n)` display suffix |
| `NotInParallelAttribute(string key)` | `TUnit.Core` | attribute | `Parallelism/ParallelismTests.cs` | `NotInParallelKeyTests` (holders == 1) | ≤1.61 | |
| `ParallelGroupAttribute(string)` | `TUnit.Core` | attribute | `Parallelism/ParallelGroupTests.cs` | `UserRepositoryTests`, `OrderRepositoryTests`, `PaymentApiTests` (other group count == 0) | ≤1.61 | groups never overlap each other |
| `[assembly: ParallelLimiter<T>]` | `TUnit.Core` | attribute | `TUnit.Patterns.Policies 1.68.0/AssemblyPolicies.cs` | `PolicyTests` | ≤1.61 | replaces class- and method-level limiters on 1.68.0 (see divergences) |
| `[assembly: NotInParallel]` | `TUnit.Core` | attribute | `TUnit.Patterns.Policies 1.68.0/AssemblyPolicies.cs` | `PolicyTests`, `SecondClassTests` (running == 1 across classes) | ≤1.61 | |

### Retry — `TUnit.Patterns 1.68.0/Retry`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `RetryAttribute.ShouldRetry(TestContext, Exception, int)` | `TUnit.Core` | method | `Retry/RetryOnTransientAttribute.cs` | `RetryTests.Custom_policy_retries_transient_failures_until_they_pass` | ≤1.61 | |
| `RetryAttribute.RetryOnExceptionTypes` | `TUnit.Core` | property | `Retry/RetryTests.cs` | `RetryTests.Built_in_policy_filters_by_exception_type` | 1.6x | `IsAssignableFrom` match |
| `RetryAttribute.BackoffMs` / `BackoffMultiplier` | `TUnit.Core` | property | `Retry/RetryTests.cs` | same | 1.6x | exponential backoff |
| `TestContext.Execution.CurrentRetryAttempt` | `TUnit.Core.Interfaces.ITestExecution` | property | `Retry/RetryTests.cs` | `RetryTests` | 1.6x | 0-based |
| `TestContext.Execution.RetryAttempts` | `TUnit.Core.Interfaces.ITestExecution` | property | `Retry/RetryTests.cs` | `RetryTests` | 1.6x | failed prior attempts, empty when none |
| `[Timeout]` + `[Retry]` interplay (fresh timeout per attempt) | `TUnit.Core` | attribute | `Extensions/RegistrationTests.cs` | `TimeoutRetryTests` (attempt 0 times out, attempt 1 passes) | ≤1.61 | |
| `[assembly: Retry(n)]` → `Metadata.TestDetails.RetryLimit` | `TUnit.Core` | attribute, property | `TUnit.Patterns.Policies 1.68.0/AssemblyPolicies.cs` | `PolicyTests.Assembly_wide_retry_limit_is_visible_on_the_test_details` | ≤1.61 | |
| `[assembly: Timeout(ms)]` → `Metadata.TestDetails.Timeout` | `TUnit.Core` | attribute, property | `TUnit.Patterns.Policies 1.68.0/AssemblyPolicies.cs` | `PolicyTests.Assembly_wide_timeout_is_visible_on_the_test_details` | ≤1.61 | |

### Data sources — `TUnit.Patterns 1.68.0/Data`, `TUnit.Mixed 1.68.0`

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
| `ArgumentsAttribute` (method and class level) | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/DataDrivenTests.cs` | `DataDrivenTests`, `ClassLevelArgumentTests` | ≤1.61 | |
| `MethodDataSourceAttribute` (tuple rows) | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/DataDrivenTests.cs` | `DataDrivenTests.Subtract_WithMethodDataSource` | ≤1.61 | |
| `DataSourceGeneratorAttribute<T1, T2, T3>` | `TUnit.Core` | class | `TUnit.Mixed 1.68.0/Data/AdditionDataGenerator.cs` | `DataDrivenTests.Add_WithCustomDataGenerator` | ≤1.61 | |
| `MatrixDataSourceAttribute` + `MatrixAttribute` | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/DataDrivenTests.cs` | `DataDrivenTests.Multiply_AllCombinations` | ≤1.61 | |
| `MethodDataSourceAttribute<TClass>(string)` | `TUnit.Core` | attribute | `Data/MoreDataSources.cs` | `MoreDataSourceTests.Generic_method_data_source` | ≤1.61 | source class must be non-static |
| `TypedDataSourceAttribute<T>.GetTypedDataRowsAsync` | `TUnit.Core` | class | `Data/MoreDataSources.cs` | `MoreDataSourceTests.Typed_data_source_yields_instances` | ≤1.61 | `IAsyncEnumerable<Func<Task<T>>>` |
| `IKeyedDataSource.Key` (set before `InitializeAsync`) | `TUnit.Core.Interfaces` | interface | `Data/MoreDataSources.cs` | `MoreDataSourceTests.Keyed_fixture_knows_its_key` | 1.6x | non-nullable `string` |
| `TestBuilderContext.Current.StateBag` / `TestMetadata.Name` | `TUnit.Core` | property | `Context/SessionArtifacts.cs` | `TestBuilderContextTests` | ≤1.61 | discovery-time state copied into `TestContext.StateBag` |

### Extension points — `TUnit.Patterns 1.68.0/Extensions`, `TUnit.Mixed 1.68.0/CancellationTests.cs`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ITestExecutor.ExecuteTest(TestContext, Func<ValueTask>)` + `TestExecutorAttribute<T>` | `TUnit.Core.Interfaces` / `TUnit.Core` | interface, attribute | `Extensions/ScopedCultureExecutor.cs` | `TestExecutorTests.Executor_establishes_the_ambient_culture` | ≤1.61 | |
| `CultureAttribute(string)` | `TUnit.Core.Executors` | attribute | `Extensions/ExtensionTests.cs` | `TestExecutorTests.Built_in_culture_attribute_does_the_same` | ≤1.61 | |
| `ITestStartEventReceiver` / `ITestEndEventReceiver` on an attribute | `TUnit.Core.Interfaces` | interface | `Extensions/StopwatchAttribute.cs` | `EventReceiverTests` | ≤1.61 | `Order` required |
| `EventReceiverStage.Early` | `TUnit.Core.Enums` | enum | `Extensions/StopwatchAttribute.cs` | `EventReceiverTests` (start before `[Before(Test)]`, end before `[After(Test)]`) | 1.6x | |
| `SkipAttribute.ShouldSkip(TestRegisteredContext)` | `TUnit.Core` | method | `Extensions/SkipOnAttribute.cs` | `SkipTests` | ≤1.61 | registration-time decision |
| `[Before(Test)]` / `[After(Test)]` with `TestContext` parameter | `TUnit.Core` | attribute | `Extensions/ExtensionTests.cs`, `TUnit.Mixed 1.68.0/BasicTests.cs` | `EventReceiverTests` | ≤1.61 | |
| `[Before(Class)]` / `[After(Class)]` (`ClassHookContext`), `[Before(TestSession)]` / `[After(TestSession)]` (`TestSessionContext`) | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/BasicTests.cs`, `HooksAndLifecycle.cs` | run of `TUnit.Mixed` | ≤1.61 | |
| `TestContext.Execution.Cancel()` | `TUnit.Core` | method | `TUnit.Mixed 1.68.0/CancellationTests.cs` | `PerTestCancellationShowcase` (`[Explicit]`) | 1.64.0 | per-test cancellation |
| `TestContext.Execution.AddLinkedCancellationToken` | `TUnit.Core` | method | `TUnit.Mixed 1.68.0/CancellationTests.cs` | `BeforeHookLinkedCancellationShowcase`, `ExecutorLinkedCancellationShowcase` | 1.64.0 | honoured from hooks and executors |
| `TimeoutAttribute(int)` + injected `CancellationToken` | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/CancellationTests.cs` | same | ≤1.61 | |
| exception thrown while a test handles its timeout kept in the result | engine | — | `TUnit.Mixed 1.68.0/CancellationTests.cs` | `TimeoutDiagnosticsShowcase` (`[Explicit]`; run output: `timed out after 00:00:00.1000000` + `Task exception: OperationCanceledException: database container still starting…`) | 1.66.0 | timeout classification race fixed in 1.67.0 |
| `ExplicitAttribute` | `TUnit.Core` | attribute | `TUnit.Mixed 1.68.0/CancellationTests.cs` | opt-in run | ≤1.61 | |
| `IHookExecutor` (10 methods, `MethodMetadata.Name`) + `HookExecutorAttribute<T>` | `TUnit.Core.Interfaces` / `TUnit.Core.Executors` | interface, attribute | `Extensions/RecordingHookExecutor.cs`, `Extensions/RegistrationTests.cs` | `RegistrationTests.Hook_ran_through_the_custom_executor` | ≤1.61 | |
| `ITestRegisteredEventReceiver` + `TestRegisteredContext.SetHookExecutor` / `SetParallelLimiter` | `TUnit.Core.Interfaces` / `TUnit.Core` | interface, method | `Extensions/RegistrationAttributes.cs` | `RegistrationTests.Registration_receiver_installed_the_hook_executor` (`Execution.CustomHookExecutor`) | ≤1.61 | |
| `TestRegisteredContext.SetTestExecutor` with an executor that is also an `ITestRegisteredEventReceiver` | `TUnit.Core` | method | `Extensions/ExecutorRegistration.cs` | `ExecutorRegistrationTests.Installed_executor_receives_its_registration_callback_once` (count == 1, `Parallelism.Limiter` is the executor's) | 1.67.0 | installed receivers run after their installer, once per instance (reference identity); never called on 1.65.68 |
| explicit `[ParallelLimiter<T>]` over `SetParallelLimiter` from a receiver or executor | `TUnit.Core` | attribute | `Extensions/ExecutorRegistrationTests.cs` | `ExecutorRegistrationTests.Explicit_limiter_beats_the_executor_default` | 1.67.0 | independent of callback order; an explicit limit may also be wider |
| `ITestDiscoveryEventReceiver.OnTestDiscovered` + `DiscoveredTestContext.TestContext.StateBag` | `TUnit.Core.Interfaces` | interface | `Extensions/RegistrationAttributes.cs` | `RegistrationTests.Discovery_receiver_assigned_an_id` | ≤1.61 | docs' Global Test IDs pattern |
| `ILastTestInClassEventReceiver` / `ILastTestInAssemblyEventReceiver` / `ILastTestInTestSessionEventReceiver` | `TUnit.Core.Interfaces` | interface | `Extensions/RegistrationAttributes.cs` | `RegistrationTests.Last_test_in_class_was_observed` (`[After(Class)]`) | ≤1.61 | |
| `DisplayNameFormatterAttribute.FormatDisplayName(DiscoveredTestContext)` | `TUnit.Core` | class | `Extensions/RegistrationAttributes.cs` | `RegistrationTests.Formatter_rewrites_the_display_name` (`--list-tests` shows `[…]`) | ≤1.61 | |
| `Skip.When` / `Skip.Unless` / `Skip.Test` | `TUnit.Core` | method | `Extensions/RegistrationTests.cs` | `SkipAndInconclusiveTests` (one run-time skip) | ≤1.61 | |
| `InconclusiveTestException` | `TUnit.Core.Exceptions` | class | `Extensions/RegistrationTests.cs` | `SkipAndInconclusiveTests.Inconclusive_at_run_time` (`[Explicit]`) | ≤1.61 | reported as failed on 1.68.0 |
| `TestContext.Parameters.TryGetValue` (`--test-parameter key=value`) | `TUnit.Core` | property | `Extensions/RegistrationTests.cs` | `SkipAndInconclusiveTests.Test_parameters_come_from_the_command_line` (run with `environment=staging`) | ≤1.61 | |
| `[assembly: Culture]` / `[assembly: Category]` | `TUnit.Core.Executors` / `TUnit.Core` | attribute | `TUnit.Patterns.Policies 1.68.0/AssemblyPolicies.cs` | `PolicyTests` | ≤1.61 | `Metadata.TestDetails.Categories` |

### Assertions — `TUnit.Patterns 1.68.0/Assertions`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `GenerateAssertionAttribute(ExpectationMessage = "… {param}")` on `bool` / `AssertionResult` methods | `TUnit.Assertions.Attributes` | attribute | `Assertions/OrderAssertions.cs` | `AssertionTests` | ≤1.61 | placeholders substituted |
| `AssertionResult.Passed` / `Failed(string)` | `TUnit.Assertions.Core` | class | `Assertions/OrderAssertions.cs` | `AssertionTests.Generated_assertion_reports_its_custom_failure` | ≤1.61 | |
| `AssertionFromAttribute<T>(Type, string, CustomName, NegateLogic, ExpectationMessage)` | `TUnit.Assertions.Attributes` | attribute | `Assertions/OrderAssertions.cs` | `AssertionTests.Lifted_assertions_and_their_negation` | ≤1.61 | `{param}` placeholders NOT substituted — keep literal |
| `.And` / `.Or` chaining on generated assertions | `TUnit.Assertions` | — | `Assertions/AssertionTests.cs` | `AssertionTests.Generated_assertions_chain`, `Or_short_circuits_on_the_first_pass` | ≤1.61 | |
| `Assert.That(Func<Task>).Throws<T>()` → `AssertionException` | `TUnit.Assertions.Exceptions` | method | `Assertions/AssertionTests.cs` | same | ≤1.61 | |
| `.All().Satisfy(x => x.IsEqualTo(...))` | `TUnit.Assertions` | method | `Retry/RetryTests.cs` | `RetryTests` | ≤1.61 | lambda receives an `IAssertionSource<T>` |
| `Assert.That(Type?)` → `TypeValueAssertion.IsAssignableTo<T>` / `IsAssignableFrom<T>` / `IsNotAssignableTo<T>` / `IsNotAssignableFrom<T>` | `TUnit.Assertions.Sources` | class, method | `Assertions/TypeAssertionTests.cs` | `TypeAssertionTests.Generic_assignability_evaluates_the_represented_type` | 1.66.0 | evaluated `RuntimeType` before |
| `IsAssignableTo(Type)` / `IsAssignableFrom(Type)` on a `Type` source | `TUnit.Assertions.Extensions` | method | `Assertions/TypeAssertionTests.cs` | `TypeAssertionTests.Runtime_type_overloads_take_a_type_argument`, `Failure_names_the_represented_types` | 1.66.0 | source-generated from `TypeAssertionExtensions` |
| type assignability behind `.And` / `.Or` | `TUnit.Assertions` | — | `Assertions/TypeAssertionTests.cs` | `TypeAssertionTests.Chained_assignability_inspects_the_runtime_type` | 1.66.0 | checks `RuntimeType` again (see divergences) |
| `Assert.Multiple()` with concurrent failures; `.Or` chain inside it | `TUnit.Assertions` | method | `Assertions/MultipleTests.cs` | `MultipleTests` (all 8 × 250 failures in the inner `AggregateException`; an independent failure survives a passing `.Or`) | 1.66.16 | 1.65.68 kept 1495 of 2000 |

### Context and artifacts — `TUnit.Patterns 1.68.0/Context`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `PropertyAttribute(name, value)` + `Metadata.TestDetails.CustomProperties` | `TUnit.Core` | attribute, property | `Context/ContextTests.cs` | `ContextTests.Custom_properties_are_readable_at_run_time`, `--treenode-filter "/*/*/*/*[Owner=platform]"` | ≤1.61 | |
| `TestContext.Isolation.UniqueId` / `GetIsolatedName` / `GetIsolatedPrefix` | `TUnit.Core.Interfaces.ITestIsolation` | property, method | `Context/ContextTests.cs` | `ContextTests.Isolation_helpers_produce_unique_resource_names` | 1.66.16 | `test_{id}_{name}` (was `Test_{id}_{name}`, breaking) · `test{sep}{id}{sep}` |
| `TestContext.ResultsDirectory` (static) | `TUnit.Core` | property | `Context/ContextTests.cs` | `ContextTests.Artifacts_land_in_the_results_directory` | 1.64.6 | honours `--results-directory` |
| `TestContext.Output.AttachArtifact(path, displayName:, description:)` | `TUnit.Core.Interfaces.ITestOutput` | method | `Context/ContextTests.cs` | same (artifact listed in run output) | ≤1.61 | |
| `TestContext.Metadata.TestDetails.TestId` | `TUnit.Core` | property | (used during development) | — | ≤1.61 | stable per test case |
| `TestSessionContext.Current.AddArtifact(Artifact)` | `TUnit.Core` | method | `Context/SessionArtifacts.cs` | run output lists `session-info.txt` | ≤1.61 | `[Before(TestSession)]` |

### Dynamic tests — `TUnit.Patterns 1.68.0/Dynamic`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `DynamicTestBuilderAttribute` + `DynamicTestBuilderContext.AddTest` | `TUnit.Core` | attribute, method | `Dynamic/DynamicTests.cs` | `DynamicTests.Greets` ×3 | ≤1.61 | |
| `DynamicTest<T> { TestMethod, TestMethodArguments, Attributes }` + `DynamicTestHelper.Argument<T>()` | `TUnit.Core` | class | `Dynamic/DynamicTests.cs` | same | ≤1.61 | lambda is an expression, not a delegate |

### Fixtures and injection — `TUnit.Patterns 1.68.0/Fixtures`, `TUnit.Mixed 1.68.0`, `TUnit 1.68.0`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `ClassDataSourceAttribute<T>` on a `required` property (test class and fixture) | `TUnit.Core` | attribute | `Fixtures/Fixtures.cs`, `Fixtures/FixtureTests.cs` | `NestedInjectionTests.Nested_property_is_initialized_before_its_owner` | ≤1.61 | dependency-ordered init |
| `SharedType.Keyed` + `Key` | `TUnit.Core` | enum, property | `Fixtures/Fixtures.cs` | `NestedInjectionTests.Keyed_sharing_hands_out_the_same_instance` | ≤1.61 | same instance across property and parameter injection |
| `SharedType.PerClass` / `PerTestSession` | `TUnit.Core` | enum | `TUnit.Mixed 1.68.0/DependencyInjectionTests.cs`, `TUnit 1.68.0/Tests.cs` | run of those projects | ≤1.61 | |
| `IAsyncInitializer` / `IAsyncDisposable` on fixtures | `TUnit.Core.Interfaces` | interface | `Fixtures/Fixtures.cs`, `TUnit.Mixed 1.68.0/Data/InMemoryDb.cs` | `NestedInjectionTests` | ≤1.61 | |
| `IAsyncDiscoveryInitializer` + `InstanceMethodDataSourceAttribute` | `TUnit.Core.Interfaces` / `TUnit.Core` | interface, attribute | `Fixtures/Fixtures.cs`, `Fixtures/FixtureTests.cs` | `DiscoveryInitializerTests` (2 rows discovered) | 1.6x | discovery-time init |
| `WebApplicationFactory<Program>` as `ClassDataSource` + `IAsyncInitializer` | `Microsoft.AspNetCore.Mvc.Testing` | class | `TUnit 1.68.0/TUnit 1.68.0/WebApplicationFactory.cs` | `Tests.Test` | ≤1.61 | |

### TUnit.Mocks — `TUnit.Mocks 1.68.0`

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
| runtime auto-stubs for ungenerated interfaces | `TUnit.Mocks` | — | `RuntimeAutoStubTests.cs` | `RuntimeAutoStubTests` | 1.63.0 | reuses `DynamicProxyGenAssembly2` identity (still, on 1.68.0: deleting `VendorSdk`'s grant brings the pre-1.63.0 `NullReferenceException` back — 1 of 20 fails) |
| `<TUnitMocksExperimentalInternalsAccess>` + `<TUnitMocksInternalsAccess Include>` | MSBuild | config | `TUnit.Mocks 1.68.0.csproj`, `InternalsAccessTests.cs` | `InternalsAccessTests` | 1.63.0 (experimental) | still experimental in 1.68.0; IDEs report false `CS0122` until 1.68.17 (see divergences) |
| `T.Mock()` on interfaces with `static abstract` members → `Mock<TMockable>`; pass `.Object` | `TUnit.Mocks` | method | `StaticAbstractTests.cs` | `StaticAbstractTests` | 1.65.0 | wrapper is not the interface here |
| non-generic method next to same-named generic overload | generator | — | `StaticAbstractTests.cs` | `StaticAbstractTests` | 1.65.63 | fix |
| `T.Mock()` on a class whose base constructor calls virtual / abstract members | generator | — | `ConstructorCallbackTests.cs` | `ConstructorCallbackTests.Base_constructor_calls_are_recorded_on_the_mock` (constructor calls `WasCalled(Times.Once)`) | 1.66.27 | `NullReferenceException` on 1.65.68; construction runs under loose defaults |
| `Raise{Event}(args)` with a `ref struct` argument (`EventHandler<TRefStruct>`, `ReadOnlySpan<T>` delegate) | `TUnit.Mocks` | method | `RefStructEventTests.cs` | `RefStructEventTests.Ref_struct_payload_reaches_the_subscriber`, `Span_argument_is_raised_without_copying_to_the_heap` | 1.68.0 | the mock did not compile on 1.67.0 (CS0030 casts to `object`); typed dispatch, no boxing |
| `Raise{Event}(ref arg)` on a delegate with `ref` / `in` / `out` parameters | `TUnit.Mocks` | method | `RefStructEventTests.cs` | `RefStructEventTests.Ref_argument_changes_reach_the_caller_and_later_subscribers` (10 − 3 − 4 = 3) | 1.68.0 | modifiers kept; changes reach the caller and later subscribers |
| `.Callback(() => mock.Raise{Event}(new …))` instead of `.Raises{Event}(args)` for stack-only arguments | `TUnit.Mocks` | method | `RefStructEventTests.cs` | `RefStructEventTests.Callback_creates_the_argument_when_the_setup_runs` | 1.68.0 | no deferred `.Raises{Event}` is generated for these events |

### TUnit.Playwright — `TUnit.Playwright 1.68.0` (run with browsers installed by `Hooks.cs`)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `PageTest` base + `Page` / `Expect(...)` | `TUnit.Playwright` | class | `Tests.cs` | `Tests.Test` | ≤1.61 | needs network (playwright.dev) |
| `PageFixture` via `[ClassDataSource<PageFixture>]` ×2 (shared browser, isolated contexts) | `TUnit.Playwright` | class | `TwoContextFixtureTests.cs` | `TwoContextFixtureTests.Two_Pages_Have_Isolated_Storage_But_Share_Browser` | ≤1.61 | |
| `Microsoft.Playwright.Program.Main(["install"])` in `[Before(TestSession)]` | `Microsoft.Playwright` | method | `Hooks.cs` | run of `TUnit.Playwright 1.68.0` | ≤1.61 | |
| `RecordVideoAttribute(path, width, height)` on a `PageTest` method | `TUnit.Playwright` | attribute | `VideoRecordingTests.cs` | `VideoRecordingTests.Recorded_page_uses_the_recording_viewport` (`Page.Video` set, viewport 640×360) | 1.68.0 | method-only; unmarked tests do not record (`Tests_without_the_attribute_do_not_record`) |
| recording renamed to `{TestName}.webm` (or `{TestName}-{n}.webm`) and attached via `Output.AttachArtifact` | `TUnit.Playwright` | — | `VideoRecordingTests.cs` | `VideoRecordingTests.Recording_is_named_after_the_test_and_attached` (`[DependsOn]`; reads `Output.Artifacts` of the recorded test) | 1.68.0 | finalised after teardown; retries get `-attempt{n}`, extra pages `-{i}`; never overwrites — a rerun into the same directory adds `-2`, `-3`, … |
| `[RecordVideo]` on a per-test `PageFixture` | `TUnit.Playwright` | attribute | `VideoRecordingTests.cs` | `FixtureVideoRecordingTests.Fixture_page_is_recorded_at_the_default_size` (1280×1400) | 1.68.0 | fixture must stay `SharedType.None` |

### Telemetry — traces, logs, metrics (`TUnit.Patterns 1.68.0/Telemetry`)

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `TUnitOpenTelemetry.Configure(Action<TracerProviderBuilder>)` in `[Before(TestDiscovery, Order = int.MinValue)]` | `TUnit.OpenTelemetry` | method | `Telemetry/TraceSetup.cs` | `TraceTests` | 1.6x | auto-start runs at `Order = int.MaxValue` |
| `TUNIT_OTEL_AUTOSTART=1` | env | config | `Telemetry/TraceSetup.cs` | `TraceTests.Sut_spans_nest_under_the_test_and_carry_the_test_id` | 1.6x | required: the HTML reporter already listens to `TUnit`, which makes auto-start step aside |
| `Activity.Current` under the `TUnit` source with `tunit.test.id` baggage; `TestContext.Current.Activity` | `System.Diagnostics` / `TUnit.Core` | property | `Telemetry/TraceTests.cs` | `TraceTests.Test_body_runs_under_the_tunit_span` | 1.6x | net8.0+ |
| `TUnitTestCorrelationProcessor` (pre-registered) tags SUT spans with `tunit.test.id` | `TUnit.OpenTelemetry` | class | `Telemetry/TraceTests.cs` | `TraceTests.Sut_spans_nest_under_the_test_and_carry_the_test_id` | 1.6x | parent = test body span |
| exported `test case` span with `test.case.result.status = pass`, status `Unset` | engine | — | `Telemetry/TraceTests.cs` | `TraceTests.Dependency_exported_a_passed_test_case_span` | 1.6x | asserted from a `[DependsOn]` test |
| `TestContext.RegisterTrace(ActivityTraceId)` | `TUnit.Core` | method | `Telemetry/TraceTests.cs` | `TraceTests.External_traces_can_be_linked` | 1.6x | links out-of-process traces into the HTML report |
| `AddInMemoryExporter(ICollection<Activity>)` with a lock-guarded collection | `OpenTelemetry.Trace` | method | `Telemetry/TraceSetup.cs` | `TraceTests` | — | tests export concurrently |
| `ILoggingBuilder.AddTUnit(TestContext)` (`TUnit.Logging.Microsoft`) | `TUnit.Logging.Microsoft` | method | `Telemetry/TraceTests.cs` | `LoggingTests.Microsoft_logging_is_bridged_into_the_test_output` | 1.6x | `ILogger` → `GetStandardOutput()` |
| `TestContext.GetDefaultLogger().LogInformation` / `GetStandardOutput()` | `TUnit.Core` | method | `Telemetry/TraceTests.cs` | `LoggingTests.Default_logger_reaches_output_and_custom_sinks` | ≤1.61 | |
| `ILogSink` + `TUnitLoggerFactory.AddSink` (`[Before(TestDiscovery)]`) | `TUnit.Core.Logging` | interface, method | `Telemetry/TraceSetup.cs` | same (`Context` is the `TestContext`) | ≤1.61 | `TUnit.Core.Context` clashes with a `Context` namespace — qualify it |
| `TestContext.GetById(id)` + `MakeCurrent()` under `ExecutionContext.SuppressFlow()` | `TUnit.Core` | method | `Telemetry/TraceTests.cs` | `LoggingTests.Make_current_reattaches_output_from_a_foreign_context` | 1.6x | cross-thread output correlation |
| `MetricCollector<T>` / `FakeLogger<T>` / OpenTelemetry in-memory metric exporter | `Microsoft.Extensions.Diagnostics.Metrics.Testing` / `Microsoft.Extensions.Logging.Testing` / `OpenTelemetry.Metrics` | class | `Telemetry/SignalsTests.cs` | `SignalsTests` | — | framework-neutral; same code as in xunit.v3-4.0.0 |
| `[LoggerMessage]` + `ActivitySource` + `Meter` in the SUT | `Microsoft.Extensions.Logging` / `System.Diagnostics` | — | `Telemetry/OrderService.cs` | all telemetry tests | — | tests share `[NotInParallel(Telemetry.Key)]` |

### ASP.NET Core — `TUnit 1.68.0` + `TUnit.AspNetCore`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `TestWebApplicationFactory<TEntryPoint>` | `TUnit.AspNetCore` | class | `TUnit 1.68.0/TUnit 1.68.0/WebApplicationFactory.cs` | `Tests.Test`, `TracedWebTests` | 1.6x | replaces vanilla `WebApplicationFactory` (analyzer TUnit0064) |
| `WebApplicationTest<TFactory, TEntryPoint>` (`Factory`, `Services`, `UniqueId`, `GetIsolatedName`, `GetIsolatedPrefix`) | `TUnit.AspNetCore` | class | `TUnit 1.68.0/TUnit 1.68.0/TracedWebTests.cs` | `TracedWebTests.Isolation_helpers_are_available_on_the_base_class` | 1.6x | per-test isolated factory |
| `traceparent` / `X-TUnit-TestId` propagation from `Factory.CreateClient()` | `TUnit.AspNetCore` | — | `TUnit 1.68.0/WebApp/Program.cs` (`/trace`) | `TracedWebTests.Server_sees_the_test_trace_and_test_id` | 1.6x | server `Activity.TraceId` == test `TraceId`; header == `TestContext.Id` |
| server-side `ILogger` routed into the calling test | `TUnit.AspNetCore` | — | `TUnit 1.68.0/WebApp/Program.cs` | `TracedWebTests.Server_side_logs_are_routed_into_this_test` | 1.6x | `CorrelatedTUnitLoggerProvider` + `TUnitTestContextMiddleware` |
| `WebApplicationTestOptions.EnableHttpExchangeCapture` + `HttpExchangeCapture.Last` | `TUnit.AspNetCore` / `TUnit.AspNetCore.Interception` | property, class | `TUnit 1.68.0/TUnit 1.68.0/TracedWebTests.cs` | `TracedWebTests.Http_exchanges_are_captured_for_assertions` | 1.6x | resolve the store from `Services` (see divergences) |

### Reporting — `TUnit.Patterns.Policies 1.68.0`

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `BeforeTestDiscoveryContext.Settings.Reporting` (`ReportingSettings`) | `TUnit.Core.Settings` | class, property | `TUnit.Patterns.Policies 1.68.0/ReportingPolicy.cs` | `ReportingPolicyTests.Discovery_hook_configured_the_reports` | 1.66.0 | set in `[Before(TestDiscovery)]` |
| `ReportingSettings.HtmlReportEnabled` / `JsonReportEnabled` | `TUnit.Core.Settings` | property | same | `TestResults/` after a run holds `…-report.html` and no `….tunit-report.json`; a stale sidecar is deleted | 1.66.0 | `TUNIT_DISABLE_HTML_REPORTER` / `TUNIT_DISABLE_JSON_REPORT` take precedence |
| `ReportingSettings.ArtifactUploadEnabled` | `TUnit.Core.Settings` | property | same | `ReportingPolicyTests` | 1.66.0 | CI artifact upload only; `TUNIT_DISABLE_ARTIFACT_UPLOAD` takes precedence |

### Native AOT

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `<PublishAot>true</PublishAot>` on a TUnit project | MSBuild | config | `TUnit.Patterns.Policies 1.68.0/TUnit.Patterns.Policies 1.68.0.csproj` | `dotnet publish -c Release -r osx-arm64` → 18 MB Mach-O arm64 binary, 11/11 pass | ≤1.61 | source-generated mode needs no reflection |

### Analyzers

| API | Namespace | Kind | Example | Verified by | Since | Note |
|---|---|---|---|---|---|---|
| `TUnit0023` (disposable member must be disposed in a cleanup method) accepts disposal through casts | `TUnit.Analyzers` | config | `TUnit.Mixed 1.68.0/CancellationTests.cs` (`CastDisposalShowcase`) | build: 0 warnings on 1.68.0; 2 false `TUnit0023` on 1.67.0 (`_viaCast`, `_viaAs`) | 1.68.0 | `((IDisposable)x).Dispose()`, `(x as IDisposable)?.Dispose()`, `object` field cast back |

### Present in 1.68.0, no example yet

`TUnit.Aspire` (Aspire dashboard / OTLP receiver) · `AutoReceiver.Endpoint` (out-of-process OTLP receiver) · `TracedWebApplicationFactory<T>` wrapper for foreign factories · `HttpExchangeCapture` body options (`CaptureRequestBody`, `MaxBodySize`) · `TUnit.Mocks.Http` · `TUnit.Assertions.Should` · `--tunit-report-html-filename` (registered only next to `Microsoft.Testing.Extensions.HtmlReport`)

### Divergences from docs (verified on 1.68.0)

- `ITestRetryEventReceiver.OnTestRetry` is declared in `TUnit.Core` but nothing in the engine invokes it; read `Execution.RetryAttempts` / `CurrentRetryAttempt` instead.
- `[AssertionFrom]` does not substitute `{parameter}` placeholders in `ExpectationMessage` (only `[GenerateAssertion]` does); `nameof(string.StartsWith)` also fails to generate because of its overloads — point it at a single-overload static helper.
- `TestContext.GetDisplayName()` does not exist; use `TestContext.Current.Metadata.DisplayName`.
- `EventReceiverStage.Early` moves the **end** receiver before `[After(Test)]` hooks as well, not only the start receiver before `[Before(Test)]`.
- The analyzer rejects typed tuple data sources whose element type is `object[]` for an `int[]` parameter (TUnit0001); the 1.65.68 element-wise conversion is reachable through an untyped generator.
- An assembly-level `[ParallelLimiter<T>]` is applied instead of a class- or method-level one (1.67.0 probe: tests under a class-level and a method-level limiter both report `Parallelism.Limiter` = the assembly's type; on 1.65.68 a class limited to 2 exceeded it under an assembly limit of 32); the docs describe Method > Class > Assembly precedence. The 1.67.0 explicit-over-programmatic fix does not change this.
- `TUnit.OpenTelemetry` auto-start stays dormant when any listener is attached to the `TUnit` source, and the built-in HTML reporter always is one; `TUNIT_OTEL_AUTOSTART=1` (set before the `Order = int.MaxValue` hook runs) forces it.
- `InconclusiveTestException` is reported as a failed test, not an inconclusive one.
- `WebApplicationTest.HttpCapture` returns a store the capture middleware never writes to; the populated `HttpExchangeCapture` is the one registered in the SUT's services.
- `<TUnitMocksInternalsAccess>` on an assembly that comes from a `ProjectReference` confuses Roslyn-workspace tooling (C# language server, OmniSharp, anything on `MSBuildWorkspace`) on 1.68.0: it gets the publicized copy as a metadata reference **and** keeps `VendorSdk` as a live project reference, the two share one assembly identity, the project's own compilation wins, and every `IQuotaPolicy` in `InternalsAccessTests.cs` reads as `CS0122` while `dotnet build` is clean. Fixed in 1.68.17 (#6836, #6837: the project reference is detached in design-time builds only; opt out with `TUnitMocksInternalsAccessDetachDesignTimeProjectReferences=false`). Probed with an `MSBuildWorkspace` 5.9.0 load of a standalone copy: 1.68.0 → `VendorSdk` as project reference + `CS0122`; 1.68.17 → metadata reference only, 0 errors.
- `Assert.That(typeof(X))` uses the represented type only for the first assignability assertion; after `.And` / `.Or` the check runs against the `RuntimeType` object (stated in the XML docs of `TypeValueAssertion`, not in the assertion docs).

## [1.67.0] — 2026-09-15

Packages: `TUnit 1.67.0` · `TUnit.Mocks 1.67.0` · `TUnit.Playwright 1.67.0` · `Microsoft.AspNetCore.Mvc.Testing 10.0.12` · SDK `10.0.401` · `net10.0`
Projects: `TUnit.Patterns 1.67.0` (204 pass, 2 intentional skips) · `TUnit.Patterns.Policies 1.67.0` (11; also published Native AOT) · `TUnit.Mixed 1.67.0` (29) · `TUnit.Mocks 1.67.0` (16) · `TUnit 1.67.0` + `TUnit.AspNetCore` (5) · `AdvancedPatterns` (10 + 2 key-gated skips) · `TUnit.Playwright 1.67.0` (builds; needs browsers to run)
Extra packages: `TUnit.OpenTelemetry 1.67.0` · `TUnit.Logging.Microsoft 1.67.0` · `TUnit.AspNetCore 1.67.0` · `OpenTelemetry.Exporter.InMemory 1.18.0` · `Microsoft.Extensions.Diagnostics.Testing 10.9.0` · `Microsoft.Extensions.Logging 10.0.12`
`Microsoft.Extensions.Logging` and `Microsoft.AspNetCore.Mvc.Testing` move to 10.0.12 because the 1.67.0 packages require it (NU1605 otherwise).

Superseded by the tables above; the 1.67.0 tree is commit `048b7c3`.

## [1.65.68] — 2026-08-29

Packages: `TUnit 1.65.68` · `TUnit.Mocks 1.65.68` · `TUnit.Playwright 1.65.68` · `Microsoft.AspNetCore.Mvc.Testing 10.0.10` · SDK `10.0.400` · `net10.0`
Projects: `TUnit.Patterns 1.65.68` (196 pass, 2 intentional skips) · `TUnit.Patterns.Policies 1.65.68` (10; also published Native AOT) · `TUnit.Mixed 1.65.68` (29) · `TUnit.Mocks 1.65.68` (15) · `TUnit 1.65.68` + `TUnit.AspNetCore` (5) · `AdvancedPatterns` (10 + 2 key-gated skips) · `TUnit.Playwright 1.65.68` (builds; needs browsers to run)
Extra packages: `TUnit.OpenTelemetry 1.65.68` · `TUnit.Logging.Microsoft 1.65.68` · `TUnit.AspNetCore 1.65.68` · `OpenTelemetry.Exporter.InMemory 1.18.0` · `Microsoft.Extensions.Diagnostics.Testing 10.9.0` · `Microsoft.Extensions.Logging 10.0.11` · `Microsoft.AspNetCore.Mvc.Testing 10.0.11`

Superseded by the tables above; the 1.65.68 tree is commit `c31ad41`.
