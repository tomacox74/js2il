using BenchmarkDotNet.Running;
using Benchmarks;

// Run benchmarks based on command line arguments
var programArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();

if (programArgs.Length > 0 && programArgs[0] == "--validate")
{
    // Run validation tests for runtime adapters
    ValidationTest.RunValidation();
}
else if (programArgs.Length > 0 && programArgs[0] == "--phased")
{
    // Run phased benchmarks (js2il compile/execute plus Jint in-proc comparison)
    BenchmarkRunner.Run<Js2ILPhasedBenchmarks>();
}
else if (programArgs.Length > 0 && programArgs[0] == "--all")
{
    // Run all benchmarks
    BenchmarkRunner.Run<JavaScriptRuntimeBenchmarks>();
    BenchmarkRunner.Run<Js2ILPhasedBenchmarks>();
}
else
{
    // Run cross-runtime comparison by default
    BenchmarkRunner.Run<JavaScriptRuntimeBenchmarks>();
}

Console.WriteLine("\nBenchmark execution complete!");
Console.WriteLine("Results are saved in BenchmarkDotNet.Artifacts/");
Console.WriteLine("\nFor more options:");
Console.WriteLine("  dotnet run -c Release          # Run cross-runtime comparison");
Console.WriteLine("  dotnet run -c Release --phased # Run js2il phased + Jint in-proc comparison");
Console.WriteLine("  dotnet run -c Release --all    # Run all benchmarks");
Console.WriteLine("  dotnet run -c Release --validate # Run validation tests");
