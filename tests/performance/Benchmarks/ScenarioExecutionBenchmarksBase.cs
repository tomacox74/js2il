using BenchmarkDotNet.Attributes;
using System.Reflection;
using System.Runtime.Loader;
using Jroc;
using Jroc.Runtime;
using Jint;
using Acornima.Ast;
using Microsoft.Extensions.DependencyInjection;
using Okojo.Bytecode;
using Okojo.Compiler;
using Okojo.Parsing;
using Okojo.Runtime;

namespace Benchmarks;

/// <summary>
/// Shared execution-only benchmark harness for file based scenario suites.
/// Compilation/preparation happens in global setup so that the measured region
/// contains script execution only, which keeps the numbers comparable across
/// jroc, Jint and Okojo.
/// </summary>
public abstract class ScenarioExecutionBenchmarksBase : ExecutionBenchmarksBase
{
    private Prepared<Script> _jintPreparedScript = default!;
    private JsRuntime? _okojoPreparedRuntime;
    private JsScript _okojoPreparedScript = null!;
    private AssemblyLoadContext? _compiledLoadContext;
    private Assembly? _compiledAssembly;
    private string? _compiledModuleId;
    private string? _jrocCompileFailure;
    private string _tempDir = "";

    /// <summary>
    /// Absolute path of the directory holding the scenario scripts for this suite.
    /// </summary>
    protected abstract string ScenariosDirectory { get; }

    [GlobalSetup(Target = nameof(Jroc_ExecuteOnly))]
    public void SetupJroc()
    {
        var scenario = LoadScenario();
        _tempDir = Path.Combine(Path.GetTempPath(), $"jroc-benchmarks-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        var tempScriptFile = Path.Combine(_tempDir, $"{scenario.ScriptName}.js");
        File.WriteAllText(tempScriptFile, scenario.Content);

        var outputPath = Path.Combine(_tempDir, scenario.ScriptName);
        var options = new CompilerOptions { OutputDirectory = outputPath };
        var serviceProvider = CompilerServices.BuildServiceProvider(options);
        var compiler = serviceProvider.GetRequiredService<Compiler>();
        if (!compiler.Compile(tempScriptFile, scenario.ScriptName))
        {
            _jrocCompileFailure = "jroc compilation failed for this scenario";
            return;
        }

        var dllPath = Path.Combine(outputPath, $"{scenario.ScriptName}.dll");
        if (!File.Exists(dllPath))
        {
            _jrocCompileFailure = $"compiled assembly not found: {dllPath}";
            return;
        }

        var fullDllPath = Path.GetFullPath(dllPath);
        _compiledLoadContext = new BenchmarkModuleLoadContext(
            typeof(JavaScriptRuntime.EnvironmentProvider).Assembly,
            fullDllPath,
            $"jroc-bench-{scenario.ScriptName}-{Guid.NewGuid():N}");
        _compiledAssembly = _compiledLoadContext.LoadFromAssemblyPath(fullDllPath);
        _compiledModuleId = ResolveModuleId(_compiledAssembly, scenario.ScriptName);
    }

    [GlobalSetup(Target = nameof(Jint_ExecutePrepared))]
    public void SetupJint()
    {
        var scenario = LoadScenario();
        _jintPreparedScript = Engine.PrepareScript(scenario.Content, $"{scenario.ScriptName}.js");
    }

    [GlobalSetup(Target = nameof(Okojo_ExecuteOnly))]
    public void SetupOkojo()
    {
        var scenario = LoadScenario();
        var runtime = JsRuntime.CreateBuilder().Build();
        try
        {
            var program = JavaScriptParser.ParseScript(scenario.Content);
            _okojoPreparedScript = JsCompiler.Compile(runtime.MainRealm, program);
            _okojoPreparedRuntime = runtime;
        }
        catch
        {
            runtime.Dispose();
            throw;
        }
    }

    [GlobalCleanup(Target = nameof(Jroc_ExecuteOnly))]
    public void CleanupJroc()
    {
        _compiledLoadContext?.Unload();
        _compiledLoadContext = null;
        _compiledAssembly = null;
        _compiledModuleId = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (!string.IsNullOrEmpty(_tempDir) && Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }

        _tempDir = "";
    }

    [GlobalCleanup(Target = nameof(Okojo_ExecuteOnly))]
    public void CleanupOkojo()
    {
        _okojoPreparedRuntime?.Dispose();
        _okojoPreparedRuntime = null;
    }

    public override IEnumerable<string> ScriptNames()
    {
        return BenchmarkScenarioCatalog.LoadScenarios(ScenariosDirectory)
            .Where(scenario => MatchesScenarioFilter(
                scenario.Key,
                scenario.ScriptName,
                scenario.ScriptName + ".js"))
            .Select(scenario => scenario.Key)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name, StringComparer.Ordinal);
    }

    [Benchmark(Description = "jroc-execute")]
    public void Jroc_ExecuteOnly()
    {
        if (_jrocCompileFailure is not null)
        {
            throw new InvalidOperationException(
                $"jroc phased setup failed for scenario '{ScriptName}': {_jrocCompileFailure}");
        }

        var assembly = _compiledAssembly
            ?? throw new InvalidOperationException($"No compiled assembly is available for scenario '{ScriptName}'.");
        var moduleId = _compiledModuleId
            ?? throw new InvalidOperationException($"No compiled module ID is available for scenario '{ScriptName}'.");
        using var exports = JsEngine.LoadModule(assembly, moduleId);
    }

    [Benchmark(Description = "jint-execute")]
    public void Jint_ExecutePrepared()
    {
        var engine = new Engine(options => options.Strict());
        engine.Execute(_jintPreparedScript);
    }

    [Benchmark(Description = "okojo-execute")]
    public void Okojo_ExecuteOnly()
    {
        var runtime = _okojoPreparedRuntime
            ?? throw new InvalidOperationException($"No prepared Okojo runtime is available for scenario '{ScriptName}'.");
        runtime.MainRealm.Execute(_okojoPreparedScript);
    }

    private BenchmarkScenario LoadScenario()
    {
        var scenario = BenchmarkScenarioCatalog.LoadScenarios(ScenariosDirectory)
            .SingleOrDefault(scenario => string.Equals(scenario.Key, ScriptName, StringComparison.Ordinal));

        return scenario ?? throw new InvalidOperationException(
            $"Benchmark scenario '{ScriptName}' could not be loaded.");
    }

    private static string ResolveModuleId(Assembly assembly, string fallback)
    {
        var moduleIds = assembly
            .GetCustomAttributes<JsCompiledModuleAttribute>()
            .Select(a => a.ModuleId)
            .Where(m => !string.IsNullOrWhiteSpace(m))
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        if (moduleIds.Length == 0)
        {
            return fallback;
        }

        if (moduleIds.Contains(fallback, StringComparer.Ordinal))
        {
            return fallback;
        }

        return moduleIds[0];
    }

    private sealed class BenchmarkModuleLoadContext : AssemblyLoadContext
    {
        private readonly Assembly _runtimeAssembly;
        private readonly string _runtimeAssemblyName;
        private readonly AssemblyDependencyResolver _resolver;

        public BenchmarkModuleLoadContext(Assembly runtimeAssembly, string mainAssemblyPath, string contextName)
            : base(contextName, isCollectible: true)
        {
            _runtimeAssembly = runtimeAssembly;
            _runtimeAssemblyName = runtimeAssembly.GetName().Name ?? nameof(JavaScriptRuntime.EnvironmentProvider);
            _resolver = new AssemblyDependencyResolver(mainAssemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            if (string.Equals(assemblyName.Name, _runtimeAssemblyName, StringComparison.Ordinal))
            {
                return _runtimeAssembly;
            }

            var resolvedPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrWhiteSpace(resolvedPath))
            {
                return LoadFromAssemblyPath(resolvedPath);
            }

            return null;
        }
    }
}
