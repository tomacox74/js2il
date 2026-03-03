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
public class TsonicPhasedBenchmarks
{
    private static readonly Dictionary<string, string> Scripts = new();
    private static readonly Dictionary<string, string> ScenarioKeyToScriptName = new(StringComparer.Ordinal);

    private static readonly object WorkspaceInitLock = new();
    private static bool WorkspaceInitialized;
    private static string WorkspaceTempDir = "";
    private static string WorkspaceDir = "";
    private static string ProjectDir = "";
    private static string AppTsPath = "";
    private static string GeneratedDir = "";
    private static string WorkspaceTsonicCommand = "";

    private ScenarioLoadContext? _compiledLoadContext;
    private Action? _compiledEntrypoint;
    private string? _compileFailure;

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
    private string _tsonicCommand = "tsonic";

    [GlobalSetup(Target = nameof(Tsonic_Generate))]
    public void SetupGenerate()
    {
        SetupCommon(precompileForExecuteOnly: false);
    }

    [GlobalSetup(Target = nameof(Tsonic_ExecuteOnly))]
    public void SetupExecuteOnly()
    {
        SetupCommon(precompileForExecuteOnly: true);
    }

    private void SetupCommon(bool precompileForExecuteOnly)
    {
        EnsureWorkspaceInitialized();

        _workspaceDir = WorkspaceDir;
        _projectDir = ProjectDir;
        _appTsPath = AppTsPath;
        _generatedDir = GeneratedDir;
        _tsonicCommand = WorkspaceTsonicCommand;

        _compileFailure = null;
        _compiledEntrypoint = null;
        _compiledLoadContext = null;

        if (precompileForExecuteOnly)
        {
            _tempDir = Path.Combine(Path.GetTempPath(), $"js2il-benchmarks-tsonic-case-{Guid.NewGuid()}");
            Directory.CreateDirectory(_tempDir);

            try
            {
                PrecompileScenario(ScriptName);
            }
            catch (Exception ex)
            {
                _compileFailure = ex.Message;
            }
        }
    }

    [GlobalCleanup(Target = nameof(Tsonic_ExecuteOnly))]
    public void CleanupExecuteOnly()
    {
        _compiledLoadContext?.Unload();
        _compiledLoadContext = null;
        _compiledEntrypoint = null;

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        if (!string.IsNullOrWhiteSpace(_tempDir) && Directory.Exists(_tempDir))
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
        if (_compileFailure != null)
        {
            return;
        }

        try
        {
            WriteAppTsForScenario(ScriptName);
            RunProcessOrThrow(_tsonicCommand, "generate src\\App.ts -q", _projectDir);
        }
        catch (Exception ex)
        {
            _compileFailure ??= ex.Message;
        }
    }

    [Benchmark(Description = "tsonic execute (pre-compiled)")]
    public void Tsonic_ExecuteOnly()
    {
        if (_compileFailure != null || _compiledEntrypoint == null)
        {
            return;
        }

        _compiledEntrypoint();
    }

    private void PrecompileScenario(string scenarioKey)
    {
        WriteAppTsForScenario(scenarioKey);

        RunProcessOrThrow(_tsonicCommand, "generate src\\App.ts -q", _projectDir);
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

        _compiledLoadContext = loadContext;
        _compiledEntrypoint = CreateEntrypointInvoker(entryPoint);
    }

    private void WriteAppTsForScenario(string scenarioKey)
    {
        var script = Scripts[scenarioKey];

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
        return ScenarioKeyToScriptName.TryGetValue(scenarioKey, out var scriptName)
            ? scriptName
            : scenarioKey;
    }

    private static void EnsureWorkspaceInitialized()
    {
        if (WorkspaceInitialized)
        {
            return;
        }

        lock (WorkspaceInitLock)
        {
            if (WorkspaceInitialized)
            {
                return;
            }

            // NOTE: BenchmarkDotNet executes each benchmark in its own process. To avoid paying `tsonic init`
            // + `npm install` for every case, we keep a persistent workspace cache under %TEMP%.
            WorkspaceTempDir = Path.Combine(Path.GetTempPath(), "js2il-benchmarks-tsonic-cache");
            WorkspaceDir = Path.Combine(WorkspaceTempDir, "tsonic-workspace");

            var bootstrapTsonic = ResolveTsonicCommand();
            var readyMarkerPath = Path.Combine(WorkspaceDir, ".js2il-tsonic-ready");

            using (var initMutex = new System.Threading.Mutex(false, "JS2IL_TSONIC_WORKSPACE_CACHE_INIT"))
            {
                initMutex.WaitOne();
                try
                {
                    if (!File.Exists(readyMarkerPath))
                    {
                        if (Directory.Exists(WorkspaceDir))
                        {
                            Directory.Delete(WorkspaceDir, recursive: true);
                        }

                        Directory.CreateDirectory(WorkspaceDir);

                        // Use --skip-types because 'tsonic init' currently can fail while trying to npm install.
                        // We explicitly install the required @tsonic/* packages below.
                        RunProcessOrThrow(bootstrapTsonic, "init --skip-types -q", WorkspaceDir);
                        InstallWorkspacePackages(WorkspaceDir);
                        ApplyTsonicWindowsFrontendPathWorkaround(WorkspaceDir);

                        File.WriteAllText(readyMarkerPath, "ready");
                    }
                }
                finally
                {
                    initMutex.ReleaseMutex();
                }
            }

            // Always attempt to patch on Windows (cheap + idempotent).
            ApplyTsonicWindowsFrontendPathWorkaround(WorkspaceDir);

            var localTsonic = Path.Combine(
                WorkspaceDir,
                "node_modules",
                ".bin",
                OperatingSystem.IsWindows() ? "tsonic.cmd" : "tsonic");
            WorkspaceTsonicCommand = File.Exists(localTsonic) ? localTsonic : bootstrapTsonic;

            // Resolve the created project directory (init creates a default project under packages/).
            var packagesDir = Path.Combine(WorkspaceDir, "packages");
            if (!Directory.Exists(packagesDir))
            {
                throw new InvalidOperationException($"tsonic init did not create a packages/ folder under '{WorkspaceDir}'.");
            }

            var projectDirs = Directory.GetDirectories(packagesDir)
                .OrderBy(d => d, StringComparer.Ordinal)
                .ToArray();

            if (projectDirs.Length == 0)
            {
                throw new InvalidOperationException($"tsonic init did not create a default project under '{packagesDir}'.");
            }

            ProjectDir = projectDirs[0];
            AppTsPath = Path.Combine(ProjectDir, "src", "App.ts");
            GeneratedDir = Path.Combine(ProjectDir, "generated");

            LoadBenchmarkScripts();

            WorkspaceInitialized = true;
        }
    }

    private static void LoadBenchmarkScripts()
    {
        if (Scripts.Count > 0)
        {
            return;
        }

        var scriptsDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Scenarios");
        var scriptFiles = Directory.GetFiles(scriptsDir, "*.js")
            .Where(path => !TemporarilyExcludedScriptNames.Contains(Path.GetFileNameWithoutExtension(path)))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToArray();

        for (int i = 0; i < scriptFiles.Length; i++)
        {
            var scriptPath = scriptFiles[i];
            var scriptName = Path.GetFileNameWithoutExtension(scriptPath);
            Scripts[scriptName] = File.ReadAllText(scriptPath);
            ScenarioKeyToScriptName[scriptName] = scriptName;
        }
    }

    private static void InstallWorkspacePackages(string workspaceDir)
    {
        // NOTE: These packages are required for type stubs and CLR bindings. tsonic does not currently
        // ship them as dependencies of the CLI itself.
        const string packages = "tsonic@0.0.63 @tsonic/core@10.0.35 @tsonic/dotnet@10.0.35 @tsonic/globals@10.0.35";
        const string args = "install -D " + packages + " --legacy-peer-deps --silent --no-audit --no-fund";

        if (OperatingSystem.IsWindows())
        {
            RunProcessOrThrow("cmd.exe", "/c npm " + args, workspaceDir);
        }
        else
        {
            RunProcessOrThrow("npm", args, workspaceDir);
        }
    }

    private static void ApplyTsonicWindowsFrontendPathWorkaround(string workspaceDir)
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        var creationJsPath = Path.Combine(workspaceDir, "node_modules", "@tsonic", "frontend", "dist", "program", "creation.js");
        if (!File.Exists(creationJsPath))
        {
            return;
        }

        var text = File.ReadAllText(creationJsPath);
        const string marker = "JS2IL_WINDOWS_PATH_WORKAROUND";
        if (text.Contains(marker, StringComparison.Ordinal))
        {
            return;
        }

        const string oldLine = "        .filter((sf) => !sf.isDeclarationFile && absolutePaths.includes(sf.fileName));";
        if (!text.Contains(oldLine, StringComparison.Ordinal))
        {
            return;
        }

        var newBlock =
            "        .filter((sf) => {\n" +
            "          // " + marker + ": @tsonic/frontend does a Windows path compare using path.resolve() output\\n" +
            "          // (\\\\ separators) against TypeScript sf.fileName (normalized to /).\\n" +
            "          if (sf.isDeclarationFile) return false;\n" +
            "          const norm = (p) => p.replace(/\\\\\\\\/g, \"/\").toLowerCase();\n" +
            "          const sfName = norm(sf.fileName);\n" +
            "          return absolutePaths.some((p) => norm(p) === sfName);\n" +
            "        });";

        text = text.Replace(oldLine, newBlock, StringComparison.Ordinal);
        File.WriteAllText(creationJsPath, text);
    }

    private static string ResolveTsonicCommand()
    {
        // BenchmarkDotNet runs in separate processes; relying on PATH alone is brittle on Windows.
        if (CanRunTool("tsonic"))
        {
            return "tsonic";
        }

        var appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var userNpmBin = Path.Combine(appData, "npm");
        var candidates = new[]
        {
            Path.Combine(userNpmBin, "tsonic.cmd"),
            Path.Combine(userNpmBin, "tsonic.exe"),
            Path.Combine(userNpmBin, "tsonic.bat")
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate) && CanRunTool(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException(
            "tsonic CLI not found or not runnable. Install it with: npm install -g tsonic\n" +
            "Then rerun the benchmarks with: dotnet run -c Release -- --phased --tsonic\n" +
            $"Tried: tsonic, {string.Join(", ", candidates)}");
    }

    private static bool CanRunTool(string command)
    {
        try
        {
            RunProcessOrThrow(command, "--version", Environment.CurrentDirectory);
            return true;
        }
        catch
        {
            return false;
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

        var stdout = new StringBuilder();
        var stderr = new StringBuilder();

        using var process = new Process { StartInfo = psi };
        process.OutputDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                stdout.AppendLine(e.Data);
            }
        };
        process.ErrorDataReceived += (_, e) =>
        {
            if (e.Data != null)
            {
                stderr.AppendLine(e.Data);
            }
        };

        if (!process.Start())
        {
            throw new InvalidOperationException($"failed to start process: {fileName}");
        }

        process.BeginOutputReadLine();
        process.BeginErrorReadLine();
        process.WaitForExit();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException(
                $"command failed (exit {process.ExitCode}): {fileName} {arguments}\n" +
                stdout.ToString() +
                (stderr.Length == 0 ? "" : "\n" + stderr));
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
