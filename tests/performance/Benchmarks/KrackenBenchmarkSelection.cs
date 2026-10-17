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

    // SharpTS 1.0.12 emits invalid IL for these unchanged fixtures. Keep them
    // available via --comprehensive so later upstream versions can be retested.
    private static readonly HashSet<string> DisabledSharpTsScenarios =
    [
        "kracken-audio-dft.js",
        "kracken-audio-oscillator.js",
        "kracken-audio-beat-detection.js",
        "kracken-ai-astar.js"
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
        {
            if (IncludeDisabled || benchmarkCase.Descriptor.Type != typeof(KrackenExecutionBenchmarks))
            {
                return true;
            }

            var method = benchmarkCase.Descriptor.WorkloadMethod.Name;
            if (DisabledRuntimeMethods.Contains(method))
            {
                return false;
            }

            var scriptName = benchmarkCase.Parameters.Items
                .Single(parameter => parameter.Name == nameof(ExecutionBenchmarksBase.ScriptName)).Value?.ToString();
            return method != nameof(KrackenExecutionBenchmarks.RunSharpTsTest)
                || !DisabledSharpTsScenarios.Contains(scriptName ?? "");
        });
    }
}
