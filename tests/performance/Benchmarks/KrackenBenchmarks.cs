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
public class KrackenBenchmarks
{
    public static string? ScenarioFilter { get; set; }

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

    private void SetupJroc(string dataScriptContent, string testScriptContent)
    {
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

    private void SetupOkojo(string dataScriptContent, string testScriptContent)
    {
        var runtime = Okojo.Runtime.JsRuntime.CreateBuilder().Build();
        this._okojoRealm = runtime.MainRealm;
        _okojoRealm.Execute(dataScriptContent);
        _okojoRealm.Execute(WorkloadRegistrationScript);
        _okojoRealm.Execute(testScriptContent);
        _okojoRealm.Execute(BenchmarkRunnerScript);
     
        this._okojoRunTest = _okojoRealm.Global["runBenchmark"];
    }

    private void SetupJint(string dataScriptContent, string testScriptContent)
    {
        _jintEngine = new Engine();
        _jintEngine.Execute(dataScriptContent, $"{ScriptName}-data.js");
        _jintEngine.Execute(WorkloadRegistrationScript, "kracken-workload-registration.js");
        _jintEngine.Execute(testScriptContent, ScriptName);
        _jintEngine.Execute(BenchmarkRunnerScript, "kracken-benchmark-runner.js");
    }

    private void SetupYantraJs(string dataScriptContent, string testScriptContent)
    {
        var context = new JSContext();
        context.Eval(dataScriptContent, $"{ScriptName}-data.js");
        context.Eval(WorkloadRegistrationScript, "kracken-workload-registration.js");
        context.Eval(testScriptContent, ScriptName);
        _yantraJsRunTest = context.Eval(BenchmarkRunnerScript + "\nrunBenchmark;", "kracken-benchmark-runner.js");
        _yantraJsContext = context;
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames()
    {
        var scriptNames = ScenarioScriptNames
            .Where(MatchesScenarioFilter)
            .ToArray();

        if (scriptNames.Length == 0)
        {
            throw new InvalidOperationException(
                $"No Kraken benchmark scenario matched '{ScenarioFilter}'.");
        }

        return scriptNames;
    }

    [GlobalSetup]
    public void Setup()
    {
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios", "kracken-1.1");
        var testScriptPath = Path.Combine(scriptsDir, ScriptName);
        var dataScriptPath = Path.Combine(
            scriptsDir,
            Path.GetFileNameWithoutExtension(ScriptName) + "-data.js");
        var testScriptContent = File.ReadAllText(testScriptPath);
        var dataScriptContent = File.ReadAllText(dataScriptPath);

        SetupJroc(dataScriptContent, testScriptContent);
        SetupOkojo(dataScriptContent, testScriptContent);
        SetupJint(dataScriptContent, testScriptContent);
        SetupYantraJs(dataScriptContent, testScriptContent);
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
        return string.Equals(scenarioName, ScenarioFilter, StringComparison.Ordinal)
            || string.Equals(scriptName, ScenarioFilter, StringComparison.Ordinal);
    }
}