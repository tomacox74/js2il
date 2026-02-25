using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using System.Reflection;
using System.Runtime.Loader;
using Js2IL;
using Js2IL.Runtime;
using Benchmarks.Runtimes;
using Jint;
using Acornima.Ast;
using Microsoft.Extensions.DependencyInjection;

namespace Benchmarks;

/// <summary>
/// js2il-specific benchmark that separates compile and execute phases.
/// This provides detailed timing for AOT compilation vs execution.
/// </summary>
[MemoryDiagnoser]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public class Js2ILPhasedBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly Dictionary<string, string> _scenarioKeyToScriptName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, Prepared<Script>> _jintPreparedScripts = new();
    private readonly Dictionary<string, AssemblyLoadContext> _compiledLoadContexts = new();
    private readonly Dictionary<string, Assembly> _compiledAssemblies = new();
    private readonly Dictionary<string, string> _compiledModuleIds = new();
    private readonly Dictionary<string, string> _js2IlCompileFailures = new();
    private string _tempDir = "";

    [GlobalSetup]
    public void Setup()
    {
        // Load all benchmark scripts
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        
        var scriptFiles = Directory.GetFiles(scriptsDir, "*.js")
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        for (int i = 0; i < scriptFiles.Length; i++)
        {
            var scriptPath = scriptFiles[i];
            var scriptFile = Path.GetFileName(scriptPath);
            var scriptName = Path.GetFileNameWithoutExtension(scriptFile);
            var scenarioKey = scriptName;
            var scriptContent = File.ReadAllText(scriptPath);
            _scripts[scenarioKey] = scriptContent;
            _scenarioKeyToScriptName[scenarioKey] = scriptName;
            _jintPreparedScripts[scenarioKey] = Engine.PrepareScript(scriptContent, scriptFile);
        }

        // Create temp directory for compiled outputs
        _tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmarks-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        // Pre-compile all scripts for execution-only benchmarks
        foreach (var kvp in _scripts)
        {
            var scenarioKey = kvp.Key;
            var scriptContent = kvp.Value;
            var scriptName = ResolveScriptName(scenarioKey);
            var tempScriptFile = Path.Combine(_tempDir, $"{scriptName}.js");
            try
            {
                File.WriteAllText(tempScriptFile, scriptContent);

                var outputPath = Path.Combine(_tempDir, scriptName);

                // Build compiler with service provider
                var options = new CompilerOptions { OutputDirectory = outputPath };
                var serviceProvider = CompilerServices.BuildServiceProvider(options);
                var compiler = serviceProvider.GetRequiredService<Compiler>();
                if (!compiler.Compile(tempScriptFile, scriptName))
                {
                    _js2IlCompileFailures[scenarioKey] = "js2il compilation failed for this scenario";
                    continue;
                }

                // The DLL is placed directly in outputPath (not in a nested directory)
                // It's named after the input file (scriptName.js -> scriptName.dll)
                var dllPath = Path.Combine(outputPath, $"{scriptName}.dll");
                if (!File.Exists(dllPath))
                {
                    _js2IlCompileFailures[scenarioKey] = $"compiled assembly not found: {dllPath}";
                    continue;
                }

                var fullDllPath = Path.GetFullPath(dllPath);
                var loadContext = new BenchmarkModuleLoadContext(
                    typeof(JavaScriptRuntime.EnvironmentProvider).Assembly,
                    fullDllPath,
                    $"js2il-bench-{scriptName}-{Guid.NewGuid():N}");
                var assembly = loadContext.LoadFromAssemblyPath(fullDllPath);

                _compiledLoadContexts[scenarioKey] = loadContext;
                _compiledAssemblies[scenarioKey] = assembly;
                _compiledModuleIds[scenarioKey] = ResolveModuleId(assembly, scriptName);
            }
            catch (Exception ex)
            {
                _js2IlCompileFailures[scenarioKey] = $"setup failed: {ex.Message}";
            }
        }
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        foreach (var loadContext in _compiledLoadContexts.Values)
        {
            loadContext.Unload();
        }

        _compiledLoadContexts.Clear();
        _compiledAssemblies.Clear();
        _compiledModuleIds.Clear();
        _js2IlCompileFailures.Clear();
        _scenarioKeyToScriptName.Clear();
        _jintPreparedScripts.Clear();

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (Directory.Exists(_tempDir))
        {
            try
            {
                Directory.Delete(_tempDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [ParamsSource(nameof(ScriptNames))]
    public string ScriptName { get; set; } = "";

    public IEnumerable<string> ScriptNames()
    {
        if (_scripts.Count > 0)
        {
            var names = _scripts.Keys.AsEnumerable();
            if (_js2IlCompileFailures.Count > 0)
            {
                names = names.Where(name => !_js2IlCompileFailures.ContainsKey(name));
            }

            return names.OrderBy(name => name, StringComparer.Ordinal);
        }

        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        if (!Directory.Exists(scriptsDir))
        {
            return Array.Empty<string>();
        }

        return Directory.GetFiles(scriptsDir, "*.js")
            .Select(Path.GetFileNameWithoutExtension)
            .Where(name => !string.IsNullOrWhiteSpace(name))
            .OrderBy(name => name)!;
    }

    [Benchmark(Description = "js2il compile")]
    public void Js2IL_Compile()
    {
        if (_js2IlCompileFailures.TryGetValue(ScriptName, out _))
        {
            return;
        }

        var script = _scripts[ScriptName];
        var resolvedScriptName = ResolveScriptName(ScriptName);
        var tempScriptFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");
        var tempOutputDir = Path.Combine(Path.GetTempPath(), $"js2il-compile-{Guid.NewGuid()}");

        try
        {
            File.WriteAllText(tempScriptFile, script);

            // Build compiler with service provider
            var options = new CompilerOptions { OutputDirectory = tempOutputDir };
            var serviceProvider = CompilerServices.BuildServiceProvider(options);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            compiler.Compile(tempScriptFile, resolvedScriptName);
        }
        finally
        {
            try
            {
                if (File.Exists(tempScriptFile))
                    File.Delete(tempScriptFile);
                if (Directory.Exists(tempOutputDir))
                    Directory.Delete(tempOutputDir, recursive: true);
            }
            catch
            {
                // Ignore cleanup errors
            }
        }
    }

    [Benchmark(Description = "js2il execute (pre-compiled)")]
    public void Js2IL_ExecuteOnly()
    {
        if (_js2IlCompileFailures.TryGetValue(ScriptName, out _))
        {
            return;
        }

        var assembly = _compiledAssemblies[ScriptName];
        var moduleId = _compiledModuleIds[ScriptName];
        using var exports = JsEngine.LoadModule(assembly, moduleId);
    }

    [Benchmark(Description = "Jint prepare")]
    public void Jint_Prepare()
    {
        var script = _scripts[ScriptName];
        var resolvedScriptName = ResolveScriptName(ScriptName);
        _ = Engine.PrepareScript(script, $"{resolvedScriptName}.js");
    }

    [Benchmark(Description = "Jint execute (prepared)")]
    public void Jint_ExecutePrepared()
    {
        var engine = new Engine(options => options.Strict());
        engine.Execute(_jintPreparedScripts[ScriptName]);
    }

    private string ResolveScriptName(string scenarioKey)
    {
        return _scenarioKeyToScriptName.TryGetValue(scenarioKey, out var scriptName)
            ? scriptName
            : scenarioKey;
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
