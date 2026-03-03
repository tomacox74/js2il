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
    BenchmarkSwitcher switcher;
    var benchmarkArgs = programArgs;

    if (programArgs.Length > 0 && programArgs[0] == "--phased")
    {
        // Run phased benchmarks (js2il compile/execute plus Jint prepare/prepared execution)
        var includeTsonic = programArgs.Contains("--tsonic", StringComparer.Ordinal);
        switcher = includeTsonic
            ? BenchmarkSwitcher.FromTypes([typeof(Js2ILPhasedBenchmarks), typeof(TsonicPhasedBenchmarks)])
            : BenchmarkSwitcher.FromTypes([typeof(Js2ILPhasedBenchmarks)]);
        benchmarkArgs = programArgs.Skip(1).Where(a => !string.Equals(a, "--tsonic", StringComparison.Ordinal)).ToArray();
    }
    else if (programArgs.Length > 0 && programArgs[0] == "--all")
    {
        // Run all benchmarks
        var includeTsonic = programArgs.Contains("--tsonic", StringComparer.Ordinal);
        switcher = includeTsonic
            ? BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks), typeof(Js2ILPhasedBenchmarks), typeof(TsonicPhasedBenchmarks)])
            : BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks), typeof(Js2ILPhasedBenchmarks)]);
        benchmarkArgs = programArgs.Skip(1).Where(a => !string.Equals(a, "--tsonic", StringComparison.Ordinal)).ToArray();
    }
    else
    {
        // Run cross-runtime comparison by default
        switcher = BenchmarkSwitcher.FromTypes([typeof(JavaScriptRuntimeBenchmarks)]);
    }

    switcher.Run(benchmarkArgs);
}

Console.WriteLine("\nBenchmark execution complete!");
Console.WriteLine("Results are saved in BenchmarkDotNet.Artifacts/");
Console.WriteLine("\nFor more options:");
Console.WriteLine("  dotnet run -c Release          # Run cross-runtime comparison");
Console.WriteLine("  dotnet run -c Release --phased # Run js2il phased + Jint prepared comparison");
Console.WriteLine("  dotnet run -c Release --phased --tsonic # Also run optional Tsonic phased benchmarks (requires tsonic CLI)");
Console.WriteLine("  dotnet run -c Release --all    # Run all benchmarks");
Console.WriteLine("  dotnet run -c Release --all --tsonic    # Also run optional Tsonic phased benchmarks (requires tsonic CLI)");
Console.WriteLine("  dotnet run -c Release --validate # Run validation tests");
