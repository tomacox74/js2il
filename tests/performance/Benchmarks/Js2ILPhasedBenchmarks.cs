using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Order;
using System.Reflection;
using System.Runtime.Loader;
using Js2IL;
using Js2IL.Runtime;
using Benchmarks.Runtimes;
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
public class Js2ILPhasedBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly JintRuntime _jintRuntime = new();
    private readonly Dictionary<string, string> _compiledPaths = new();
    private readonly Dictionary<string, AssemblyLoadContext> _compiledLoadContexts = new();
    private readonly Dictionary<string, Assembly> _compiledAssemblies = new();
    private readonly Dictionary<string, string> _compiledModuleIds = new();
    private string _tempDir = "";

    [GlobalSetup]
    public void Setup()
    {
        // Load all benchmark scripts
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        
        var scriptFiles = new[]
        {
            "minimal.js",
            "evaluation.js",
            "evaluation-modern.js",
            "stopwatch.js",
            "array-stress.js"
        };

        foreach (var scriptFile in scriptFiles)
        {
            var path = Path.Combine(scriptsDir, scriptFile);
            if (File.Exists(path))
            {
                var scriptName = Path.GetFileNameWithoutExtension(scriptFile);
                _scripts[scriptName] = File.ReadAllText(path);
            }
        }

        // Create temp directory for compiled outputs
        _tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmarks-{Guid.NewGuid()}");
        Directory.CreateDirectory(_tempDir);

        // Pre-compile all scripts for execution-only benchmarks
        foreach (var kvp in _scripts)
        {
            var scriptName = kvp.Key;
            var scriptContent = kvp.Value;
            var tempScriptFile = Path.Combine(_tempDir, $"{scriptName}.js");
            File.WriteAllText(tempScriptFile, scriptContent);

            var outputPath = Path.Combine(_tempDir, scriptName);
            
            // Build compiler with service provider
            var options = new CompilerOptions { OutputDirectory = outputPath };
            var serviceProvider = CompilerServices.BuildServiceProvider(options);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            if (!compiler.Compile(tempScriptFile, scriptName))
            {
                continue;
            }

            // The DLL is placed directly in outputPath (not in a nested directory)
            // It's named after the input file (scriptName.js -> scriptName.dll)
            var dllPath = Path.Combine(outputPath, $"{scriptName}.dll");
            if (!File.Exists(dllPath))
            {
                continue;
            }

            _compiledPaths[scriptName] = dllPath;

            var loadContext = new AssemblyLoadContext($"js2il-bench-{scriptName}-{Guid.NewGuid():N}", isCollectible: true);
            var assembly = loadContext.LoadFromAssemblyPath(Path.GetFullPath(dllPath));

            _compiledLoadContexts[scriptName] = loadContext;
            _compiledAssemblies[scriptName] = assembly;
            _compiledModuleIds[scriptName] = ResolveModuleId(assembly, scriptName);
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
        if (_compiledAssemblies.Count > 0)
        {
            return _compiledAssemblies.Keys.OrderBy(name => name, StringComparer.Ordinal);
        }

        if (_scripts.Count > 0)
        {
            return _scripts.Keys;
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
        var script = _scripts[ScriptName];
        var tempScriptFile = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.js");
        var tempOutputDir = Path.Combine(Path.GetTempPath(), $"js2il-compile-{Guid.NewGuid()}");

        try
        {
            File.WriteAllText(tempScriptFile, script);

            // Build compiler with service provider
            var options = new CompilerOptions { OutputDirectory = tempOutputDir };
            var serviceProvider = CompilerServices.BuildServiceProvider(options);
            var compiler = serviceProvider.GetRequiredService<Compiler>();
            compiler.Compile(tempScriptFile, ScriptName);
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
        var assembly = _compiledAssemblies[ScriptName];
        var moduleId = _compiledModuleIds[ScriptName];
        using var exports = JsEngine.LoadModule(assembly, moduleId);
    }

    [Benchmark(Description = "Jint execute (in-proc)")]
    public void Jint_Execute()
    {
        var script = _scripts[ScriptName];
        var result = _jintRuntime.Execute(script, $"{ScriptName}.js");

        if (!result.Success)
        {
            throw new Exception($"Jint execution failed: {result.Error}");
        }
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
}
