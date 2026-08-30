using BenchmarkDotNet.Running;
using Benchmarks;

var programArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
KrackenBenchmarkSelection.IncludeDisabled = TakeFlag(ref programArgs, "--comprehensive");

BenchmarkSwitcher switcher;
var benchmarkArgs = programArgs;

if (programArgs.Length > 0 && programArgs[0] == "--dromaeo")
{
    switcher = BenchmarkSwitcher.FromTypes([typeof(DromaeoExecutionBenchmarks)]);
    benchmarkArgs = programArgs.Skip(1).ToArray();
}
else if (programArgs.Length > 0 && programArgs[0] == "--kracken")
{
    switcher = BenchmarkSwitcher.FromTypes([typeof(KrackenExecutionBenchmarks)]);
    benchmarkArgs = programArgs.Skip(1).ToArray();
}
else if (programArgs.Length > 0 && programArgs[0] == "--prime-execute")
{
    switcher = BenchmarkSwitcher.FromTypes([typeof(PrimeExecuteBenchmark)]);
    benchmarkArgs = programArgs.Skip(1).ToArray();
}
else
{
    switcher = BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks)]);
}

ExecutionBenchmarksBase.ScenarioFilter = TakeOption(ref benchmarkArgs, "--scenario");
var summaries = switcher.Run(benchmarkArgs);
var failed = summaries.Any(summary =>
    summary.HasCriticalValidationErrors
    || summary.ValidationErrors.Any()
    || summary.Reports.Any(report => !report.Success));

if (failed)
{
    Environment.ExitCode = 1;
    Console.WriteLine("\nJrocPrevious benchmark execution FAILED.");
}
else
{
    Console.WriteLine("\nJrocPrevious benchmark execution complete!");
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
            args = args.Where((_, index) => index != i).ToArray();
            return value;
        }
    }

    return null;
}

static bool TakeFlag(ref string[] args, string name)
{
    for (var i = 0; i < args.Length; i++)
    {
        if (!args[i].Equals(name, StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        args = args.Where((_, index) => index != i).ToArray();
        return true;
    }

    return false;
}
