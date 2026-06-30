using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Benchmarks;

// Run benchmarks based on command line arguments
var programArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();

if (programArgs.Length > 0 && programArgs[0] == "--validate")
{
    // Run validation tests for runtime adapters
    ValidationTest.RunValidation();
}
else
{
    if (programArgs.Length > 0 && programArgs[0] == "--dispatch")
    {
        // Run the focused late-bound dispatch microbenchmarks without interactive benchmark selection.
        var summary = BenchmarkRunner.Run<LateBoundDispatchBenchmarks>(args: programArgs.Skip(1).ToArray());
        SetExitCodeFromSummaries([summary]);
    }
    else
    {
        BenchmarkSwitcher switcher;
        var benchmarkArgs = programArgs;

        if (programArgs.Length > 0 && programArgs[0] == "--phased")
        {
            // Run phased benchmarks (jroc compile/execute, Jint prepared execution, Okojo execution)
            switcher = BenchmarkSwitcher.FromTypes([typeof(JrocPhasedBenchmarks)]);
            benchmarkArgs = programArgs.Skip(1).ToArray();
        }
        else if (programArgs.Length > 0 && programArgs[0] == "--all")
        {
            // Run all benchmarks
            switcher = BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks), typeof(LateBoundDispatchBenchmarks), typeof(JrocPhasedBenchmarks)]);
            benchmarkArgs = programArgs.Skip(1).ToArray();
        }
        else
        {
            // Run cross-runtime comparison by default
            switcher = BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks)]);
        }

        var scenarioFilter = TakeOption(ref benchmarkArgs, "--scenario");
        if (!string.IsNullOrWhiteSpace(scenarioFilter))
        {
            JrocPhasedBenchmarks.ScenarioFilter = scenarioFilter;
        }
        else
        {
            JrocPhasedBenchmarks.ScenarioFilter = null;
        }

        var summaries = switcher.Run(benchmarkArgs);
        SetExitCodeFromSummaries(summaries);
    }
}

Console.WriteLine("\nBenchmark execution complete!");
Console.WriteLine("Results are saved in BenchmarkDotNet.Artifacts/");
Console.WriteLine("\nFor more options:");
Console.WriteLine("  dotnet run -c Release          # Run cross-runtime comparison");
Console.WriteLine("  dotnet run -c Release --dispatch # Run late-bound dispatch microbenchmarks");
Console.WriteLine("  dotnet run -c Release --phased # Run jroc phased + Jint prepared + Okojo execute comparison");
Console.WriteLine("  dotnet run -c Release --all    # Run all benchmarks");
Console.WriteLine("  dotnet run -c Release --validate # Run validation tests");

static void SetExitCodeFromSummaries(IEnumerable<Summary> summaries)
{
    var summaryList = summaries.ToArray();
    var failedBenchmarks = summaryList
        .SelectMany(summary => summary.Reports
            .Where(report => !report.Success)
            .Select(report => report.BenchmarkCase.ToString()))
        .Distinct(StringComparer.Ordinal)
        .ToArray();

    var hasValidationFailures = summaryList.Any(summary =>
        summary.HasCriticalValidationErrors || summary.ValidationErrors.Any());

    if (!hasValidationFailures && failedBenchmarks.Length == 0)
    {
        return;
    }

    if (failedBenchmarks.Length > 0)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("Benchmark run failed. Cases with issues:");
        foreach (var failedBenchmark in failedBenchmarks)
        {
            Console.Error.WriteLine($"  {failedBenchmark}");
        }
    }

    if (hasValidationFailures)
    {
        Console.Error.WriteLine();
        Console.Error.WriteLine("Benchmark run failed due to BenchmarkDotNet validation errors.");
    }

    Environment.ExitCode = 1;
}

static string? TakeOption(ref string[] args, string name)
{
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];

        if (arg.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            if (i + 1 >= args.Length)
            {
                throw new ArgumentException($"{name} requires a value.");
            }

            var value = args[i + 1];

            args = args
                .Where((_, index) => index != i && index != i + 1)
                .ToArray();

            return value;
        }

        var prefix = name + "=";
        if (arg.StartsWith(prefix, StringComparison.OrdinalIgnoreCase))
        {
            var value = arg.Substring(prefix.Length);

            args = args
                .Where((_, index) => index != i)
                .ToArray();

            return value;
        }
    }

    return null;
}
