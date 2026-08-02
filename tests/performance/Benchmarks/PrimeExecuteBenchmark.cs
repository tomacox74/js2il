using Acornima.Ast;
using BenchmarkDotNet.Attributes;
using Jint;
using Jroc;
using Jroc.Runtime;
using Okojo.Bytecode;
using Okojo.Compiler;
using Okojo.Parsing;
using Okojo.Runtime;

namespace Benchmarks;

/// <summary>
/// Measures one execution pass of the PrimeJavaScript sieve after each runtime has compiled or prepared it.
/// </summary>
public class PrimeExecuteBenchmark : ExecutionBenchmarksBase
{
    private const string ScenarioName = "PrimeJavaScript.OnePass";

    private const string RuntimeBootstrapScript = """
        globalThis.console = { log: function() {} };
        globalThis.process = { argv: ['prime-execute'] };
        globalThis.performance = { now: function() { return 0; } };
        globalThis.require = function(moduleName) {
            if (moduleName === 'perf_hooks') {
                return { performance: globalThis.performance };
            }
            throw new Error("Module '" + moduleName + "' not found");
        };
        """;

    private JrocLoadedAssembly? _jrocAssembly;
    private string? _jrocModuleId;
    private Prepared<Script> _jintBootstrapScript = default!;
    private Prepared<Script> _jintPrimeScript = default!;
    private JsRuntime? _okojoRuntime;
    private JsScript _okojoPrimeScript = null!;

    public override IEnumerable<string> ScriptNames()
    {
        return MatchesScenarioFilter(ScenarioName, ScenarioName + ".js")
            ? [ScenarioName]
            : [];
    }

    [GlobalSetup(Target = nameof(Jroc_ExecuteOnly))]
    public void SetupJroc()
    {
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(ScenarioName + ".js")
        {
            SourceText = LoadPrimeScript()
        });

        _jrocAssembly = JrocInMemoryAssemblyLoader.Load(artifact);
        _jrocModuleId = artifact.ModuleIds.Single();
    }

    [GlobalSetup(Target = nameof(Jint_ExecutePrepared))]
    public void SetupJint()
    {
        _jintBootstrapScript = Engine.PrepareScript(RuntimeBootstrapScript, "prime-execute-bootstrap.js");
        _jintPrimeScript = Engine.PrepareScript(LoadPrimeScript(), ScenarioName + ".js");
    }

    [GlobalSetup(Target = nameof(Okojo_ExecutePrepared))]
    public void SetupOkojo()
    {
        _okojoRuntime = JsRuntime.CreateBuilder().Build();
        _okojoRuntime.MainRealm.Execute(RuntimeBootstrapScript);
        _okojoPrimeScript = JsCompiler.Compile(
            _okojoRuntime.MainRealm,
            JavaScriptParser.ParseScript(LoadPrimeScript()));
    }

    [GlobalCleanup(Target = nameof(Jroc_ExecuteOnly))]
    public void CleanupJroc()
    {
        _jrocAssembly?.Dispose();
        _jrocAssembly = null;
        _jrocModuleId = null;
    }

    [GlobalCleanup(Target = nameof(Okojo_ExecutePrepared))]
    public void CleanupOkojo()
    {
        _okojoRuntime?.Dispose();
        _okojoRuntime = null;
    }

    [Benchmark(Description = "jroc-execute")]
    public void Jroc_ExecuteOnly()
    {
        var assembly = _jrocAssembly
            ?? throw new InvalidOperationException("The JROC Prime assembly has not been prepared.");
        var moduleId = _jrocModuleId
            ?? throw new InvalidOperationException("The JROC Prime module ID has not been prepared.");

        using var exports = JsEngine.LoadModule(assembly.Assembly, moduleId);
    }

    [Benchmark(Description = "jint-execute")]
    public void Jint_ExecutePrepared()
    {
        var engine = new Engine(options => options.Strict());
        engine.Execute(_jintBootstrapScript);
        engine.Execute(_jintPrimeScript);
    }

    [Benchmark(Description = "okojo-execute")]
    public void Okojo_ExecutePrepared()
    {
        var runtime = _okojoRuntime
            ?? throw new InvalidOperationException("The Okojo Prime runtime has not been prepared.");

        runtime.MainRealm.Execute(_okojoPrimeScript);
    }

    private static string LoadPrimeScript()
    {
        var path = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Scenarios",
            ScenarioName + ".js");
        return File.ReadAllText(path);
    }
}
