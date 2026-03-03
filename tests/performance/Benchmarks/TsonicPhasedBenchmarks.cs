using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Order;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;

namespace Benchmarks;

/// <summary>
/// Optional phased benchmarks for Tsonic (TypeScript -> C#) in the same scenario catalog.
/// Run via: dotnet run -c Release -- --phased --tsonic
/// </summary>
[MemoryDiagnoser]
[Config(typeof(FullParamsConfig))]
[RankColumn]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[HideColumns("Error", "Gen0", "Gen1", "Gen2")]
[JsonExporterAttribute.FullCompressed]
public sealed class TsonicPhasedBenchmarks
{
    private readonly Dictionary<string, string> _scripts = new();
    private readonly Dictionary<string, string> _scenarioKeyToScriptName = new(StringComparer.Ordinal);
    private readonly Dictionary<string, ScenarioLoadContext> _compiledLoadContexts = new();
    private readonly Dictionary<string, Action> _compiledEntrypoints = new();
    private readonly Dictionary<string, string> _tsonicCompileFailures = new();

    // Keep this in sync with Js2ILPhasedBenchmarks until those scenarios are fixed.
    private static readonly HashSet<string> TemporarilyExcludedScriptNames = new(StringComparer.Ordinal)
    {
        "evaluation",
        "evaluation-modern",
        "linq-js"
    };

    private string _tempDir = "";
    private string _workspaceDir = "";
    private string _projectDir = "";
    private string _appTsPath = "";
    private string _generatedDir = "";

    [GlobalSetup]
    public void Setup()
    {
        EnsureTsonicAvailable();

        // Load all benchmark scripts
        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");

        var scriptFiles = Directory.GetFiles(scriptsDir, "*.js")
            .Where(path => !TemporarilyExcludedScriptNames.Contains(Path.GetFileNameWithoutExtension(path)))
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
        }

        // Workspace lives under temp to avoid contaminating the repo with generated output.
        _tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmarks-tsonic-{Guid.NewGuid()}");
        _workspaceDir = Path.Combine(_tempDir, "tsonic-workspace");
        Directory.CreateDirectory(_workspaceDir);

        RunProcessOrThrow("tsonic", "init --skip-types -q", _workspaceDir);

        // Resolve the created project directory (init creates a default project under packages/).
        var packagesDir = Path.Combine(_workspaceDir, "packages");
        if (!Directory.Exists(packagesDir))
        {
            throw new InvalidOperationException($"tsonic init did not create a packages/ folder under '{_workspaceDir}'.");
        }

        var projectDirs = Directory.GetDirectories(packagesDir)
            .OrderBy(d => d, StringComparer.Ordinal)
            .ToArray();

        if (projectDirs.Length == 0)
        {
            throw new InvalidOperationException($"tsonic init did not create a default project under '{packagesDir}'.");
        }

        _projectDir = projectDirs[0];
        _appTsPath = Path.Combine(_projectDir, "src", "App.ts");
        _generatedDir = Path.Combine(_projectDir, "generated");

        // Pre-generate + build managed assemblies for execution-only benchmarks.
        foreach (var kvp in _scripts)
        {
            var scenarioKey = kvp.Key;
            try
            {
                PrecompileScenario(scenarioKey);
            }
            catch (Exception ex)
            {
                _tsonicCompileFailures[scenarioKey] = ex.Message;
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
        _compiledEntrypoints.Clear();
        _tsonicCompileFailures.Clear();
        _scenarioKeyToScriptName.Clear();

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
            if (_tsonicCompileFailures.Count > 0)
            {
                names = names.Where(name => !_tsonicCompileFailures.ContainsKey(name));
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
            .Where(name => !string.IsNullOrWhiteSpace(name) && !TemporarilyExcludedScriptNames.Contains(name))
            .OrderBy(name => name, StringComparer.Ordinal)!;
    }

    [Benchmark(Description = "tsonic generate")]
    public void Tsonic_Generate()
    {
        if (_tsonicCompileFailures.TryGetValue(ScriptName, out _))
        {
            return;
        }

        WriteAppTsForScenario(ScriptName);
        RunProcessOrThrow("tsonic", "generate src\\App.ts -q", _projectDir);
    }

    [Benchmark(Description = "tsonic execute (pre-compiled)")]
    public void Tsonic_ExecuteOnly()
    {
        if (_tsonicCompileFailures.TryGetValue(ScriptName, out _))
        {
            return;
        }

        _compiledEntrypoints[ScriptName]();
    }

    private void PrecompileScenario(string scenarioKey)
    {
        WriteAppTsForScenario(scenarioKey);

        RunProcessOrThrow("tsonic", "generate src\\App.ts -q", _projectDir);
        RunProcessOrThrow("dotnet", "build -c Release -v q", _generatedDir);

        var buildOutputDir = Path.Combine(_generatedDir, "bin", "Release", "net10.0");
        if (!Directory.Exists(buildOutputDir))
        {
            throw new InvalidOperationException($"dotnet build output directory not found: {buildOutputDir}");
        }

        var scenarioOutDir = Path.Combine(_tempDir, "tsonic-managed", ResolveScriptName(scenarioKey));
        CopyDirectory(buildOutputDir, scenarioOutDir);

        var mainAssemblyPath = Path.Combine(scenarioOutDir, "app.dll");
        if (!File.Exists(mainAssemblyPath))
        {
            var candidate = Directory.GetFiles(scenarioOutDir, "*.dll")
                .OrderBy(f => f, StringComparer.Ordinal)
                .FirstOrDefault();

            if (string.IsNullOrWhiteSpace(candidate))
            {
                throw new InvalidOperationException($"no dll outputs found under: {scenarioOutDir}");
            }

            mainAssemblyPath = candidate;
        }

        var fullMainAssemblyPath = Path.GetFullPath(mainAssemblyPath);
        var loadContext = new ScenarioLoadContext(fullMainAssemblyPath, $"tsonic-bench-{ResolveScriptName(scenarioKey)}-{Guid.NewGuid():N}");
        var assembly = loadContext.LoadFromAssemblyPath(fullMainAssemblyPath);

        var entryPoint = assembly.EntryPoint;
        if (entryPoint == null)
        {
            throw new InvalidOperationException($"compiled assembly has no EntryPoint: {fullMainAssemblyPath}");
        }

        _compiledLoadContexts[scenarioKey] = loadContext;
        _compiledEntrypoints[scenarioKey] = CreateEntrypointInvoker(entryPoint);
    }

    private void WriteAppTsForScenario(string scenarioKey)
    {
        var script = _scripts[scenarioKey];

        var sb = new StringBuilder();
        sb.AppendLine("// @ts-nocheck");
        sb.AppendLine("// Modified copy of JS2IL benchmark scenario to enable running it through Tsonic.");
        sb.AppendLine("// NOTE: This is for benchmarking only; it is not intended as a correctness/compatibility test.");
        sb.AppendLine("export function main(): void {");
        sb.AppendLine("  (function __scenario(): void {");
        sb.AppendLine(script);
        sb.AppendLine("  })();");
        sb.AppendLine("}");

        Directory.CreateDirectory(Path.GetDirectoryName(_appTsPath)!);
        File.WriteAllText(_appTsPath, sb.ToString());
    }

    private string ResolveScriptName(string scenarioKey)
    {
        return _scenarioKeyToScriptName.TryGetValue(scenarioKey, out var scriptName)
            ? scriptName
            : scenarioKey;
    }

    private static void EnsureTsonicAvailable()
    {
        try
        {
            RunProcessOrThrow("tsonic", "--version", Environment.CurrentDirectory);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(
                "tsonic CLI not found or not runnable. Install it with: npm install -g tsonic\n" +
                "Then rerun the benchmarks with: dotnet run -c Release -- --phased --tsonic\n" +
                ex.Message);
        }
    }

    private static Action CreateEntrypointInvoker(MethodInfo entryPoint)
    {
        var parameters = entryPoint.GetParameters();
        if (parameters.Length == 0)
        {
            return () =>
            {
                var result = entryPoint.Invoke(null, null);
                if (result is Task task)
                {
                    task.GetAwaiter().GetResult();
                }
            };
        }

        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(string[]))
        {
            return () =>
            {
                var result = entryPoint.Invoke(null, [Array.Empty<string>()]);
                if (result is Task task)
                {
                    task.GetAwaiter().GetResult();
                }
            };
        }

        throw new InvalidOperationException($"unsupported EntryPoint signature: {entryPoint}");
    }

    private static void RunProcessOrThrow(string fileName, string arguments, string workingDirectory)
    {
        var psi = new ProcessStartInfo
        {
            FileName = fileName,
            Arguments = arguments,
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = Process.Start(psi);
        if (process == null)
        {
            throw new InvalidOperationException($"failed to start process: {fileName}");
        }

        var stdout = process.StandardOutput.ReadToEnd();
        var stderr = process.StandardError.ReadToEnd();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"command failed (exit {process.ExitCode}): {fileName} {arguments}\n" +
                stdout +
                (string.IsNullOrWhiteSpace(stderr) ? "" : "\n" + stderr));
        }
    }

    private static void CopyDirectory(string sourceDir, string destDir)
    {
        Directory.CreateDirectory(destDir);

        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
        }

        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var name = Path.GetFileName(dir);
            CopyDirectory(dir, Path.Combine(destDir, name));
        }
    }

    private sealed class ScenarioLoadContext : AssemblyLoadContext
    {
        private readonly AssemblyDependencyResolver _resolver;

        public ScenarioLoadContext(string mainAssemblyPath, string contextName)
            : base(contextName, isCollectible: true)
        {
            _resolver = new AssemblyDependencyResolver(mainAssemblyPath);
        }

        protected override Assembly? Load(AssemblyName assemblyName)
        {
            var resolvedPath = _resolver.ResolveAssemblyToPath(assemblyName);
            if (!string.IsNullOrWhiteSpace(resolvedPath))
            {
                return LoadFromAssemblyPath(resolvedPath);
            }

            return null;
        }
    }
}
