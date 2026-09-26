using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters.Json;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks;

[MemoryDiagnoser]
[ShortRunJob]
[JsonExporterAttribute.FullCompressed]
public class PrimeValidationBenchmark
{
    private JrocLoadedAssembly? _optimized;
    private JrocLoadedAssembly? _scalar;
    private string? _optimizedModuleId;
    private string? _scalarModuleId;

    [GlobalSetup]
    public void Setup()
    {
        var source = PrimeExecuteBenchmark.LoadPrimeScript();
        const string invocation = "main(config);";
        const string increment = "\t\t\t\ttotal++;";
        if (source.Split(invocation, StringSplitOptions.None).Length != 2
            || source.Split(increment, StringSplitOptions.None).Length != 2)
        {
            throw new InvalidOperationException("The Prime validation control no longer matches the pinned fixture.");
        }

        var validationSource = source.Replace(invocation,
            "new PrimeSieve(config.sieveSize).runSieve().validatePrimeCount(false);",
            StringComparison.Ordinal);
        var scalarSource = validationSource.Replace(increment,
            "\t\t\t\ttotal += 1;", StringComparison.Ordinal);
        (_optimized, _optimizedModuleId) = Prepare(validationSource, "PrimeValidationOptimized.js");
        (_scalar, _scalarModuleId) = Prepare(scalarSource, "PrimeValidationScalar.js");
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _optimized?.Dispose();
        _scalar?.Dispose();
        _optimized = null;
        _scalar = null;
    }

    [Benchmark(Description = "validation-scalar-control", Baseline = true)]
    public void ScalarControl()
    {
        using var exports = JsEngine.LoadModule(
            _scalar?.Assembly ?? throw new InvalidOperationException("Scalar control is not prepared."),
            _scalarModuleId ?? throw new InvalidOperationException("Scalar control module is not prepared."));
    }

    [Benchmark(Description = "validation-popcount")]
    public void PopCount()
    {
        using var exports = JsEngine.LoadModule(
            _optimized?.Assembly ?? throw new InvalidOperationException("Validation is not prepared."),
            _optimizedModuleId ?? throw new InvalidOperationException("Validation module is not prepared."));
    }

    private static (JrocLoadedAssembly Assembly, string ModuleId) Prepare(
        string source, string entry)
    {
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(entry)
        {
            SourceText = source
        });
        return (JrocInMemoryAssemblyLoader.Load(artifact), artifact.ModuleIds.Single());
    }
}
