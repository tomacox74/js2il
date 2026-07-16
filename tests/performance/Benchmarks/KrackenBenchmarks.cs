using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Jroc;
using Jroc.Runtime;
using Jint;
using YantraJS.Core;

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

    private void SetupJroc(string astarDataScriptContent, string astarTestScriptContent)
    {
        var runTestContent = @"export function runTest() { go(); return 'done'; }";
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

        _jrocExports = JsEngine.LoadModule(loadedAssembly.Assembly, artifact.ModuleIds[0]);        
    }

    private void SetupOkojo(string astarDataScriptContent, string astarTestScriptContent)
    {
        var runTestContent = @"function runTest() { go(); return 'done';}";

        var runtime = Okojo.Runtime.JsRuntime.CreateBuilder().Build();
        this._okojoRealm = runtime.MainRealm;
        _okojoRealm.Execute(astarDataScriptContent);
        _okojoRealm.Execute(astarTestScriptContent);
        _okojoRealm.Execute(runTestContent);
     
        this._okojoRunTest = _okojoRealm.Global["runTest"];
    }

    private void SetupJint(string astarDataScriptContent, string astarTestScriptContent)
    {
        var runTestContent = @"function runTest() { go(); return 'done';}";

        _jintEngine = new Engine(options => options.Strict());
        _jintEngine.Execute(astarDataScriptContent, "ai-astar-data.js");
        _jintEngine.Execute(astarTestScriptContent, "ai-astar.js");
        _jintEngine.Execute(runTestContent, "kracken-run-test.js");
    }

    private void SetupYantraJs(string astarDataScriptContent, string astarTestScriptContent)
    {
        var runTestContent = @"function runTest() { go(); return 'done';}";

        var context = new JSContext();
        context.Eval(astarDataScriptContent, "ai-astar-data.js");
        context.Eval(astarTestScriptContent, "ai-astar.js");
        _yantraJsRunTest = context.Eval(runTestContent + "\nrunTest;", "kracken-run-test.js");
        _yantraJsContext = context;
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames()
    {
        return new string []
        {
            "ai-astar.js"
        };
    }


    [GlobalSetup]
    public void Setup()
    {
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios", "kracken-1.1");
        var astarTestScript = Path.Combine(scriptsDir,  "ai-astar.js");
        var astarDataScript = Path.Combine(scriptsDir,  "ai-astar-data.js");
        var astarTestScriptContent = File.ReadAllText(astarTestScript);
        var astarDataScriptContent = File.ReadAllText(astarDataScript);

        SetupJroc(astarDataScriptContent, astarTestScriptContent);
        SetupOkojo(astarDataScriptContent, astarTestScriptContent);
        SetupJint(astarDataScriptContent, astarTestScriptContent);
        SetupYantraJs(astarDataScriptContent, astarTestScriptContent);
    }

    [Benchmark(Description = "jroc-execute")]
    public void RunJrocTest()
    {
        dynamic exports = _jrocExports!;
        var result = exports.runTest();
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
        var result = _jintEngine!.Invoke("runTest");
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
}