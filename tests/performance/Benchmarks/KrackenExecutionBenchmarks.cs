using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Jroc;
using Jroc.Runtime;
using Jint;
using YantraJS.Core;

namespace Benchmarks;

/// <summary>
/// The Kraken performance benchmarks consist of a test script and a data script.
/// Compile time, script load time, and data load time are excluded from the measurements.
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class KrackenExecutionBenchmarks
{
    public static string? ScenarioFilter { get; set; }
    private const string BenchmarkScriptNamePrefix = "kracken-";

    private static readonly string[] ScenarioScriptNames =
    [
        "ai-astar.js",
        "audio-beat-detection.js",
        "audio-fft.js"
    ];

    private const string WorkloadRegistrationScript = """
        var __jrocKrackenWorkload = null;
        var __jrocKrackenIterations = 0;
        var runTest = function(workload, iterations) {
            __jrocKrackenWorkload = workload;
            __jrocKrackenIterations = iterations;
        };
        """;

    private const string BenchmarkRunnerScript = """
        function runBenchmark() {
            for (var i = 0; i < __jrocKrackenIterations; i++) {
                __jrocKrackenWorkload();
            }
            return 'done';
        }
        """;

    /// <summary>
    /// Jroc compiled and loaded
    /// </summary>
    private IDisposable? _jrocExports = null;

    /// <summary>
    /// Okojo realm and runTest function
    /// </summary>
    private Okojo.Runtime.JsRealm? _okojoRealm = null;
    private Okojo.JsValue _okojoRunTest;

    /// <summary>
    /// Jint engine and prepared runTest function.
    /// </summary>
    private Engine? _jintEngine = null;

    /// <summary>
    /// YantraJS context and prepared runTest function.
    /// </summary>
    private JSContext? _yantraJsContext;
    private JSValue? _yantraJsRunTest;

    [GlobalSetup(Target = nameof(RunJrocTest))]
    public void SetupJroc()
    {
        LoadScriptContents(out var dataScriptContent, out var testScriptContent);
        var scriptBuilder = new System.Text.StringBuilder();
        scriptBuilder.Append(dataScriptContent);
        scriptBuilder.AppendLine();
        scriptBuilder.Append(WorkloadRegistrationScript);
        scriptBuilder.AppendLine();
        scriptBuilder.Append(testScriptContent);
        scriptBuilder.AppendLine();
        scriptBuilder.Append("export ");
        scriptBuilder.Append(BenchmarkRunnerScript);
        var finalScriptContent = scriptBuilder.ToString();

        var tmpPath = Path.GetTempPath();
        File.WriteAllText(Path.Combine(tmpPath, "kracken.js"), finalScriptContent);

        var request = new JrocInMemoryCompileRequest("kracken.js")
        {
            SourceText = finalScriptContent
        };
        var artifact = JrocInMemoryCompiler.Compile(request);
        var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);

        _jrocExports = JsEngine.LoadModule(loadedAssembly.Assembly, artifact.ModuleIds[0]);        
    }

    [GlobalSetup(Target = nameof(RunOkojoTest))]
    public void SetupOkojo()
    {
        LoadScriptContents(out var dataScriptContent, out var testScriptContent);
        var runtime = Okojo.Runtime.JsRuntime.CreateBuilder().Build();
        this._okojoRealm = runtime.MainRealm;
        _okojoRealm.Execute(dataScriptContent);
        _okojoRealm.Execute(WorkloadRegistrationScript);
        _okojoRealm.Execute(testScriptContent);
        _okojoRealm.Execute(BenchmarkRunnerScript);
     
        this._okojoRunTest = _okojoRealm.Global["runBenchmark"];
    }

    [GlobalSetup(Target = nameof(RunJintTest))]
    public void SetupJint()
    {
        LoadScriptContents(out var dataScriptContent, out var testScriptContent);   
        var sourceScriptName = GetSourceScriptName(ScriptName);
        _jintEngine = new Engine();
        _jintEngine.Execute(dataScriptContent, $"{sourceScriptName}-data.js");
        _jintEngine.Execute(WorkloadRegistrationScript, "kracken-workload-registration.js");
        _jintEngine.Execute(testScriptContent, sourceScriptName);
        _jintEngine.Execute(BenchmarkRunnerScript, "kracken-benchmark-runner.js");
    }

    [GlobalSetup(Target = nameof(RunYantraJsTest))]
    public void SetupYantraJs()
    {
        LoadScriptContents(out var dataScriptContent, out var testScriptContent);
        var sourceScriptName = GetSourceScriptName(ScriptName);
        var context = new JSContext();
        context.Eval(dataScriptContent, $"{sourceScriptName}-data.js");
        context.Eval(WorkloadRegistrationScript, "kracken-workload-registration.js");
        context.Eval(testScriptContent, sourceScriptName);
        _yantraJsRunTest = context.Eval(BenchmarkRunnerScript + "\nrunBenchmark;", "kracken-benchmark-runner.js");
        _yantraJsContext = context;
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames()
    {
        var scriptNames = ScenarioScriptNames
            .Where(MatchesScenarioFilter)
            .Select(GetBenchmarkScriptName)
            .ToArray();

        if (scriptNames.Length == 0)
        {
            throw new InvalidOperationException(
                $"No Kraken benchmark scenario matched '{ScenarioFilter}'.");
        }

        return scriptNames;
    }

    private void LoadScriptContents(out string dataScriptContent, out string testScriptContent)
    {
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios", "kracken-1.1");
        var sourceScriptName = GetSourceScriptName(ScriptName);
        var testScriptPath = Path.Combine(scriptsDir, sourceScriptName);
        var dataScriptPath = Path.Combine(
            scriptsDir,
            Path.GetFileNameWithoutExtension(sourceScriptName) + "-data.js");
        testScriptContent = File.ReadAllText(testScriptPath);
        dataScriptContent = File.ReadAllText(dataScriptPath);
    }

    [Benchmark(Description = "jroc-execute")]
    public void RunJrocTest()
    {
        dynamic exports = _jrocExports!;
        var result = exports.runBenchmark();
        if (result.ToString() != "done")
        {
            throw new InvalidOperationException($"Unexpected result from Jroc test: {result}");
        }
    }

    [Benchmark(Description = "okojo-execute")]
    public void RunOkojoTest()
    {
        var result = _okojoRealm!.Call(_okojoRunTest, _okojoRealm.GlobalObject);
        if (result.ToString() != "done")
        {
            throw new InvalidOperationException($"Unexpected result from Okojo test: {result}");
        }
    }

    [Benchmark(Description = "jint-execute")]
    public void RunJintTest()
    {
        var result = _jintEngine!.Invoke("runBenchmark");
        if (result.ToString() != "done")
        {
            throw new InvalidOperationException($"Unexpected result from Jint test: {result}");
        }
    }

    [Benchmark(Description = "yantrajs-execute")]
    public void RunYantraJsTest()
    {
        var arguments = Arguments.Empty;
        var result = _yantraJsRunTest!.InvokeFunction(in arguments);
        if (result.ToString() != "done")
        {
            throw new InvalidOperationException($"Unexpected result from YantraJS test: {result}");
        }
    }

    private static bool MatchesScenarioFilter(string scriptName)
    {
        if (string.IsNullOrWhiteSpace(ScenarioFilter))
        {
            return true;
        }

        var scenarioName = Path.GetFileNameWithoutExtension(scriptName);
        var benchmarkScriptName = GetBenchmarkScriptName(scriptName);
        var benchmarkScenarioName = Path.GetFileNameWithoutExtension(benchmarkScriptName);
        return string.Equals(scenarioName, ScenarioFilter, StringComparison.Ordinal)
            || string.Equals(scriptName, ScenarioFilter, StringComparison.Ordinal)
            || string.Equals(benchmarkScenarioName, ScenarioFilter, StringComparison.Ordinal)
            || string.Equals(benchmarkScriptName, ScenarioFilter, StringComparison.Ordinal);
    }

    private static string GetBenchmarkScriptName(string sourceScriptName)
    {
        return sourceScriptName.StartsWith(BenchmarkScriptNamePrefix, StringComparison.Ordinal)
            ? sourceScriptName
            : BenchmarkScriptNamePrefix + sourceScriptName;
    }

    private static string GetSourceScriptName(string benchmarkScriptName)
    {
        return benchmarkScriptName.StartsWith(BenchmarkScriptNamePrefix, StringComparison.Ordinal)
            ? benchmarkScriptName[BenchmarkScriptNamePrefix.Length..]
            : benchmarkScriptName;
    }
}