using BenchmarkDotNet.Filters;
using BenchmarkDotNet.Running;

namespace Benchmarks;

internal static class KrackenBenchmarkSelection
{
    private static readonly HashSet<string> DisabledScenarios =
    [
        "ai-astar"
    ];

    private static readonly HashSet<string> DisabledRuntimeMethods =
    [
        nameof(KrackenExecutionBenchmarks.RunOkojoTest),
        nameof(KrackenExecutionBenchmarks.RunYantraJsTest)
    ];

    public static bool IncludeDisabled { get; set; }

    public static bool IsScenarioEnabled(string scriptName)
    {
        var scenarioName = Path.GetFileNameWithoutExtension(scriptName);
        return IncludeDisabled || !DisabledScenarios.Contains(scenarioName);
    }

    public static IFilter CreateRuntimeFilter()
    {
        return new SimpleFilter(benchmarkCase =>
            IncludeDisabled
            || benchmarkCase.Descriptor.Type != typeof(KrackenExecutionBenchmarks)
            || !DisabledRuntimeMethods.Contains(benchmarkCase.Descriptor.WorkloadMethod.Name));
    }
}
