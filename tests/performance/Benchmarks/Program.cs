using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using Benchmarks;

// Run benchmarks based on command line arguments
var programArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
var debugBenchmarks = TakeFlag(ref programArgs, "--debug-benchmarks");
FullParamsConfig.DebugModeEnabled = debugBenchmarks;

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
    else if (programArgs.Length > 0 && programArgs[0] == "--object-operations")
    {
        var summary = BenchmarkRunner.Run<ObjectInternalOperationsBenchmarks>(args: programArgs.Skip(1).ToArray());
        SetExitCodeFromSummaries([summary]);
    }
    else if (programArgs.Length > 0 && programArgs[0] == "--array-operations")
    {
        var summary = BenchmarkRunner.Run<ArrayInternalOperationsBenchmarks>(args: programArgs.Skip(1).ToArray());
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
            switcher = BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks), typeof(JrocPhasedBenchmarks), typeof(KrackenBenchmarks)]);
            benchmarkArgs = programArgs.Skip(1).ToArray();
        }
        else if (programArgs.Length > 0 && programArgs[0] == "--kracken")
        {
            // Run the Kraken benchmarks
            switcher = BenchmarkSwitcher.FromTypes([typeof(KrackenBenchmarks)]);
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

if (Environment.ExitCode != 0)
{
    Console.WriteLine("\nBenchmark execution FAILED (see failure summary above).");
}
else
{
    Console.WriteLine("\nBenchmark execution complete!");
}
Console.WriteLine("Results are saved in BenchmarkDotNet.Artifacts/");
Console.WriteLine("\nFor more options:");
Console.WriteLine("  dotnet run -c Release          # Run cross-runtime comparison");
Console.WriteLine("  dotnet run -c Release --dispatch # Run late-bound dispatch microbenchmarks");
Console.WriteLine("  dotnet run -c Release --object-operations # Run ordinary-object operation microbenchmarks");
Console.WriteLine("  dotnet run -c Release --array-operations # Run dense-array operation microbenchmarks");
Console.WriteLine("  dotnet run -c Release --phased # Run jroc phased + Jint prepared + Okojo execute comparison");
Console.WriteLine("  dotnet run -c Release --all    # Run all benchmarks");
Console.WriteLine("  dotnet run -c Debug -- --dispatch --debug-benchmarks # Allow debugging benchmark code");
Console.WriteLine("  dotnet run -c Release --validate # Run validation tests");

static void SetExitCodeFromSummaries(IEnumerable<Summary> summaries)
{
    var summaryList = summaries.ToArray();
    var failedReports = summaryList
        .SelectMany(summary => summary.Reports.Where(report => !report.Success))
        .ToArray();

    var hasValidationFailures = summaryList.Any(summary =>
        summary.HasCriticalValidationErrors || summary.ValidationErrors.Any());

    if (!hasValidationFailures && failedReports.Length == 0)
    {
        return;
    }

    // Write the failure summary to stdout (not stderr) so it appears in-order in CI logs
    // instead of being interleaved after later stdout writes.
    if (failedReports.Length > 0)
    {
        Console.WriteLine();
        Console.WriteLine($"Benchmark run failed. {failedReports.Length} case(s) with issues:");
        foreach (var report in failedReports)
        {
            var caseName = report.BenchmarkCase.ToString();
            Console.WriteLine();
            Console.WriteLine($"  FAILED: {caseName}");

            var details = DescribeFailure(report).ToArray();
            foreach (var detail in details)
            {
                Console.WriteLine($"    {detail}");
            }

            var rootCause = details.FirstOrDefault(IsJsErrorLine)
                ?? details.FirstOrDefault(d => d.Contains("Exception", StringComparison.Ordinal))
                ?? details.FirstOrDefault();
            EmitGitHubErrorAnnotation(caseName, rootCause);
        }
    }

    if (hasValidationFailures)
    {
        Console.WriteLine();
        Console.WriteLine("Benchmark run failed due to BenchmarkDotNet validation errors:");
        foreach (var error in summaryList.SelectMany(summary => summary.ValidationErrors))
        {
            Console.WriteLine($"  {error.Message}");
        }

        EmitGitHubErrorAnnotation("BenchmarkDotNet validation", "One or more validation errors; see log for details.");
    }

    Environment.ExitCode = 1;
}

static IEnumerable<string> DescribeFailure(BenchmarkReport report)
{
    foreach (var executeResult in report.ExecuteResults)
    {
        if (executeResult.IsSuccess)
        {
            continue;
        }

        if (executeResult.ExitCode is int exitCode && exitCode != 0)
        {
            yield return $"benchmark process exited with code {exitCode}";
        }

        foreach (var error in executeResult.Errors)
        {
            yield return error;
        }

        // The exception text from a crashed benchmark process lands in the parsed
        // process output (non-measurement lines go to Results; StandardOutput is
        // toolchain-dependent). Surface exception/stack lines so the root cause is
        // visible in the final summary without scanning the full log.
        var outputLines = executeResult.Results
            .Concat(executeResult.StandardOutput)
            .Concat(executeResult.PrefixedLines);
        foreach (var line in outputLines)
        {
            var trimmed = line.Trim();
            if (trimmed.Contains("Exception", StringComparison.Ordinal)
                || trimmed.StartsWith("--->", StringComparison.Ordinal)
                || trimmed.StartsWith("at ", StringComparison.Ordinal)
                || IsJsErrorLine(trimmed))
            {
                yield return trimmed;
            }
        }
    }
}

static bool IsJsErrorLine(string line)
{
    var value = line.StartsWith("---> ", StringComparison.Ordinal) ? line.Substring(5) : line;
    return value.StartsWith("TypeError", StringComparison.Ordinal)
        || value.StartsWith("RangeError", StringComparison.Ordinal)
        || value.StartsWith("SyntaxError", StringComparison.Ordinal)
        || value.StartsWith("ReferenceError", StringComparison.Ordinal);
}

static void EmitGitHubErrorAnnotation(string title, string? message)
{
    if (!string.Equals(Environment.GetEnvironmentVariable("GITHUB_ACTIONS"), "true", StringComparison.OrdinalIgnoreCase))
    {
        return;
    }

    // GitHub Actions workflow command; renders as an error annotation in the run UI.
    var detail = string.IsNullOrWhiteSpace(message) ? "see benchmark log for details" : message;
    Console.WriteLine($"::error title=Benchmark failed: {Sanitize(title)}::{Sanitize(detail)}");

    static string Sanitize(string value) => value
        .Replace("%", "%25", StringComparison.Ordinal)
        .Replace("\r", "%0D", StringComparison.Ordinal)
        .Replace("\n", "%0A", StringComparison.Ordinal);
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

static bool TakeFlag(ref string[] args, string name)
{
    for (var i = 0; i < args.Length; i++)
    {
        var arg = args[i];
        if (!arg.Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        args = args
            .Where((_, index) => index != i)
            .ToArray();

        return true;
    }

    return false;
}
