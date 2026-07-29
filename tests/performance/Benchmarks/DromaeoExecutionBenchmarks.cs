namespace Benchmarks;

/// <summary>
/// Dromaeo execution benchmarks. Compilation/preparation is excluded from the
/// measured region so the numbers describe script execution only.
/// </summary>
public class DromaeoExecutionBenchmarks : ScenarioExecutionBenchmarksBase
{
    protected override string ScenariosDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Scenarios",
        "dromaeo");
}
