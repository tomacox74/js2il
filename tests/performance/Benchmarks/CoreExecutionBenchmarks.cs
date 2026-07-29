namespace Benchmarks;

/// <summary>
/// Execution-only benchmarks for the core cross-runtime scenarios that
/// <see cref="JavaScriptRuntimeBenchmarks"/> measures end-to-end. Keeping an
/// execute-only view of the same scenarios preserves the historical
/// <c>jroc-execute</c>/<c>jint-execute</c>/<c>okojo-execute</c> series that the
/// Dromaeo and Kraken suites publish.
/// </summary>
public class CoreExecutionBenchmarks : ScenarioExecutionBenchmarksBase
{
    protected override string ScenariosDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Scenarios");
}
