using BenchmarkDotNet.Attributes;
using Jroc;
using Jroc.Runtime;
using Jint;
using YantraJS.Core;
using Benchmarks.Runtimes;

namespace Benchmarks;

/// <summary>
/// The Kraken performance benchmarks consist of a test script and a data script.
/// Compile time, script load time, and data load time are excluded from the measurements.
/// </summary>
public partial class KrackenExecutionBenchmarks : ExecutionBenchmarksBase
{
    private const string BenchmarkScriptNamePrefix = "kracken-";

    private static string KrackenScriptsDirectory => Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Scenarios",
        "kracken-1.1");

    // A block avoids YantraJS's false duplicate-declaration error;
    // the var binding still belongs to the classic script's global scope.
    private const string WorkloadRegistrationScript = """
        var __jrocKrackenWorkload = null;
        var __jrocKrackenIterations = 0;
        {
            var runTest = function(workload, iterations) {
                __jrocKrackenWorkload = workload;
                __jrocKrackenIterations = iterations;
            };
        }
        """;

    private const string BenchmarkRunnerScript = """
        function runBenchmark() {
            for (var i = 0; i < __jrocKrackenIterations; i++) {
                __jrocKrackenWorkload();
            }
            return 'done';
        }
        """;

    private const string JrocHostScript = """
        require('./kracken.js');
        module.exports = globalThis;
        """;

    /// <summary>
    /// Jroc compiled and loaded
    /// </summary>
    private JrocLoadedAssembly? _jrocAssembly;
    private JsDynamicExports? _jrocExports;

    /// <summary>
    /// Okojo realm and runTest function
    /// </summary>
    private Okojo.Runtime.JsRuntime? _okojoRuntime;
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
    private SharpTsCompiledScript? _sharpTsScript;
    private Func<object>? _sharpTsRunTest;

    [GlobalSetup(Target = nameof(RunJrocTest))]
    public void SetupJroc()
    {
        var request = new JrocInMemoryMultiEntryCompileRequest(
        [
            new("kracken-host.cjs", JrocHostScript),
            new("kracken.js", LoadScriptSource()) { ParseAsScript = true }
        ])
        {
#if !JROC_PREVIOUS
            AssumeUnmodifiedHostGlobals = true
#endif
        };
        var artifact = JrocInMemoryCompiler.Compile(request);
        _jrocAssembly = JrocInMemoryAssemblyLoader.Load(artifact);
        try
        {
            _jrocExports = JsEngine.LoadDynamicModule(_jrocAssembly.Assembly, artifact.EntryModuleId);
        }
        catch
        {
            CleanupJroc();
            throw;
        }
    }

    [GlobalCleanup(Target = nameof(RunJrocTest))]
    public void CleanupJroc()
    {
        try
        {
            _jrocExports?.Dispose();
        }
        finally
        {
            _jrocExports = null;
            _jrocAssembly?.Dispose();
            _jrocAssembly = null;
        }
    }

    [GlobalSetup(Target = nameof(RunOkojoTest))]
    public void SetupOkojo()
    {
        _okojoRuntime = Okojo.Runtime.JsRuntime.CreateBuilder().Build();
        _okojoRealm = _okojoRuntime.MainRealm;
        try
        {
            _okojoRealm.Execute(LoadScriptSource());
            _okojoRunTest = _okojoRealm.Global["runBenchmark"];
        }
        catch
        {
            CleanupOkojo();
            throw;
        }
    }

    [GlobalCleanup(Target = nameof(RunOkojoTest))]
    public void CleanupOkojo()
    {
        _okojoRunTest = default;
        _okojoRealm = null;
        _okojoRuntime?.Dispose();
        _okojoRuntime = null;
    }

    [GlobalSetup(Target = nameof(RunJintTest))]
    public void SetupJint()
    {
        _jintEngine = new Engine();
        _jintEngine.Execute(LoadScriptSource(), GetSourceScriptName(ScriptName));
    }

    [GlobalCleanup(Target = nameof(RunJintTest))]
    public void CleanupJint()
    {
        _jintEngine = null;
    }

    [GlobalSetup(Target = nameof(RunYantraJsTest))]
    public void SetupYantraJs()
    {
        _yantraJsContext = new JSContext();
        try
        {
            _yantraJsContext.Eval(LoadScriptSource(), GetSourceScriptName(ScriptName));
            _yantraJsRunTest = _yantraJsContext[new JSString("runBenchmark")];
        }
        catch
        {
            CleanupYantraJs();
            throw;
        }
    }

    [GlobalCleanup(Target = nameof(RunYantraJsTest))]
    public void CleanupYantraJs()
    {
        _yantraJsRunTest = null;
        _yantraJsContext?.Dispose();
        _yantraJsContext = null;
    }

    [GlobalSetup(Target = nameof(RunSharpTsTest))]
    public void SetupSharpTs()
    {
        var script = new SharpTsCompiledScript(LoadScriptSource(), GetSourceScriptName(ScriptName));
        try
        {
            // Initialize data and register the workload once, exactly as for the other runtimes.
            script.Execute();
            _sharpTsRunTest = script.GetFunction("runBenchmark");
            _sharpTsScript = script;
        }
        catch
        {
            script.Dispose();
            throw;
        }
    }

    [GlobalCleanup(Target = nameof(RunSharpTsTest))]
    public void CleanupSharpTs()
    {
        _sharpTsRunTest = null;
        _sharpTsScript?.Dispose();
        _sharpTsScript = null;
    }

    public override IEnumerable<string> ScriptNames()
    {
        var scriptNames = Directory.Exists(KrackenScriptsDirectory)
            ? Directory.EnumerateFiles(KrackenScriptsDirectory, "*.js", SearchOption.TopDirectoryOnly)
                .OrderBy(path => path, StringComparer.Ordinal)
                .Where(path => !path.EndsWith("-data.js", StringComparison.Ordinal))
                .Select(path => Path.GetFileName(path)!)
                .Where(MatchesKrackenScenarioFilter)
                .Where(KrackenBenchmarkSelection.IsScenarioEnabled)
                .Select(GetBenchmarkScriptName)
                .ToArray()
            : [];

        if (scriptNames.Length == 0)
        {
            var comprehensiveHint = KrackenBenchmarkSelection.IncludeDisabled
                ? string.Empty
                : " Disabled scenarios can be included with --comprehensive.";
            throw new InvalidOperationException(
                $"No enabled Kraken benchmark scenario matched '{ScenarioFilter}'.{comprehensiveHint}");
        }

        return scriptNames;
    }

    private string LoadScriptSource()
    {
        var sourceScriptName = GetSourceScriptName(ScriptName);
        var testScriptPath = Path.Combine(KrackenScriptsDirectory, sourceScriptName);
        var dataScriptPath = Path.Combine(
            KrackenScriptsDirectory,
            GetDataScriptName(sourceScriptName));
        return string.Join(Environment.NewLine,
            File.ReadAllText(dataScriptPath),
            WorkloadRegistrationScript,
            File.ReadAllText(testScriptPath),
            BenchmarkRunnerScript);
    }

    [Benchmark(Description = JrocBenchmarkLabels.Execute)]
    public void RunJrocTest()
    {
        var result = _jrocExports!.Invoke("runBenchmark");
        if (result?.ToString() != "done")
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

    [Benchmark(Description = "sharpts-execute")]
    public void RunSharpTsTest()
    {
        var result = _sharpTsRunTest!();
        if (result.ToString() != "done")
        {
            throw new InvalidOperationException($"Unexpected result from SharpTS test: {result}");
        }
    }

    private static bool MatchesKrackenScenarioFilter(string scriptName)
    {
        var scenarioName = Path.GetFileNameWithoutExtension(scriptName);
        var benchmarkScriptName = GetBenchmarkScriptName(scriptName);
        var benchmarkScenarioName = Path.GetFileNameWithoutExtension(benchmarkScriptName);
        return MatchesScenarioFilter(
            scenarioName,
            scriptName,
            benchmarkScenarioName,
            benchmarkScriptName);
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

    private static string GetDataScriptName(string sourceScriptName)
    {
        return Path.GetFileNameWithoutExtension(sourceScriptName) switch
        {
            "audio-oscillator" => "audio-dft-data.js",
            var scenarioName => scenarioName + "-data.js"
        };
    }
}
