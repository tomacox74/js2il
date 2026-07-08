using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Jroc;
using Jroc.Runtime;

namespace Benchmarks;

/// <summary>
/// The Kraken performance benchmarks consist of a the test script and the data script.
/// compile time and script load time and data load time are all excluded from the measurements.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class KrackenBenchmarks
{
    private IDisposable? _exports = null;

    [GlobalSetup]
    public void Setup()
    {
        var runTestContent = @"export function runTest() { go(); }";
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios", "kracken-1.1");
        var astarTestScript = Path.Combine(scriptsDir,  "ai-astar.js");
        var astarDataScript = Path.Combine(scriptsDir,  "ai-astar-data.js");
        var astarTestScriptContent = File.ReadAllText(astarTestScript);
        var astarDataScriptContent = File.ReadAllText(astarDataScript);

        var scriptBuilder = new System.Text.StringBuilder();
        scriptBuilder.Append(astarDataScriptContent);
        scriptBuilder.AppendLine();
        scriptBuilder.Append(astarTestScriptContent);
        scriptBuilder.AppendLine();
        scriptBuilder.Append(runTestContent);
        var finalScriptContent = scriptBuilder.ToString();

        var tmpPath = Path.GetTempPath();
        File.WriteAllText(Path.Combine(tmpPath, "kracken.js"), finalScriptContent);

        var request = new JrocInMemoryCompileRequest("kracken.js")
        {
            SourceText = finalScriptContent
        };
        var artifact = JrocInMemoryCompiler.Compile(request);
        var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);

        _exports?.Dispose(); // defensive coding - should not be needed
        _exports = JsEngine.LoadModule(loadedAssembly.Assembly, artifact.ModuleIds[0]);
    }

    [Benchmark(Description = "jroc-kracken")]
    public void RunKrackenTest()
    {
        dynamic exports = _exports!;
        exports.runTest();
    }
}