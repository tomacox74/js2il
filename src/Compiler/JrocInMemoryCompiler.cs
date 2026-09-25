using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Jroc.Runtime;

namespace Jroc;

public static class JrocInMemoryCompiler
{
    /// <summary>
    /// Compiles JavaScript in memory, loads the generated assembly into a collectible load context,
    /// evaluates the target module, and returns typed exports for host interaction.
    /// </summary>
    /// <remarks>
    /// This API does not write the generated assembly to disk and does not infer a launchable path from
    /// <see cref="Assembly.Location"/>. When hosted code may call <c>child_process.fork(...)</c>, pass
    /// <see cref="JsModuleLoadOptions.CompiledAssemblyPath"/> explicitly so the child process knows which
    /// compiled assembly to launch.
    /// </remarks>
    public static JrocInMemoryModule<TExports> CompileAndLoadModule<TExports>(
        JrocInMemoryCompileRequest request,
        string? moduleId = null,
        JsModuleLoadOptions? options = null)
        where TExports : class
    {
        var artifact = Compile(request);
        var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);

        try
        {
            var resolvedModuleId = ResolveTypedModuleId<TExports>(request, artifact, moduleId);
            var exports = JsEngine.LoadModule<TExports>(loadedAssembly.Assembly, resolvedModuleId, options);
            return new JrocInMemoryModule<TExports>(loadedAssembly, exports);
        }
        catch
        {
            loadedAssembly.Dispose();
            throw;
        }
    }

    /// <summary>
    /// Compiles JavaScript in memory, loads the generated assembly into a collectible load context,
    /// evaluates the target module, and returns a dynamic/reflection-friendly exports proxy.
    /// </summary>
    /// <remarks>
    /// This API does not write the generated assembly to disk and does not infer a launchable path from
    /// <see cref="Assembly.Location"/>. When hosted code may call <c>child_process.fork(...)</c>, pass
    /// <see cref="JsModuleLoadOptions.CompiledAssemblyPath"/> explicitly so the child process knows which
    /// compiled assembly to launch.
    /// </remarks>
    public static JrocInMemoryModule CompileAndLoadModule(
        JrocInMemoryCompileRequest request,
        string? moduleId = null,
        JsModuleLoadOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(request);

        var artifact = Compile(request);
        var loadedAssembly = JrocInMemoryAssemblyLoader.Load(artifact);

        try
        {
            var resolvedModuleId = ResolveModuleId(request, artifact, moduleId);
            var exports = JsEngine.LoadDynamicModule(
                loadedAssembly.Assembly,
                resolvedModuleId,
                options);
            return new JrocInMemoryModule(loadedAssembly, exports, exports);
        }
        catch
        {
            loadedAssembly.Dispose();
            throw;
        }
    }

    public static JrocCompiledAssemblyArtifact Compile(
        JrocInMemoryCompileRequest request,
        ICompilerOutput? compilerOutput = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentException.ThrowIfNullOrWhiteSpace(request.EntryFilePath);

        return Compile(new JrocInMemoryMultiEntryCompileRequest(
            [new JrocInMemoryEntrySource(request.EntryFilePath, request.SourceText, request.RootModuleIdOverride)])
        {
            AssemblyName = request.AssemblyName,
            FileSystem = request.FileSystem,
            EmitPdb = request.EmitPdb,
            Verbose = request.Verbose,
            DiagnosticFilePath = request.DiagnosticFilePath,
            AnalyzeUnused = request.AnalyzeUnused,
            GenerateModuleExportContracts = request.GenerateModuleExportContracts,
            HostRuntimeIntrinsics = request.HostRuntimeIntrinsics
        }, compilerOutput);
    }

    /// <summary>
    /// Compiles independent entry sources into one loadable artifact without writing the assembly or PDB to disk.
    /// The first entry is the default unless <see cref="JrocInMemoryMultiEntryCompileRequest.DefaultEntryFilePath"/> is set.
    /// </summary>
    public static JrocCompiledAssemblyArtifact Compile(
        JrocInMemoryMultiEntryCompileRequest request,
        ICompilerOutput? compilerOutput = null)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Entries);
        if (request.Entries.Count == 0)
        {
            throw new ArgumentException("At least one entry source is required.", nameof(request));
        }

        for (var index = 0; index < request.Entries.Count; index++)
        {
            if (request.Entries[index] is null || string.IsNullOrWhiteSpace(request.Entries[index].EntryFilePath))
            {
                throw new ArgumentException($"Entry source at index {index} must have a nonempty file path.", nameof(request));
            }
        }

        var options = new CompilerOptions
        {
            AssemblyName = request.AssemblyName,
            EmitPdb = request.EmitPdb,
            Verbose = request.Verbose,
            DiagnosticFilePath = request.DiagnosticFilePath,
            AnalyzeUnused = request.AnalyzeUnused,
            GenerateModuleExportContracts = request.GenerateModuleExportContracts,
            HostRuntimeIntrinsics = request.HostRuntimeIntrinsics
        };

        var effectiveFileSystem = CreateEffectiveFileSystem(request);
        var capturingOutput = new CapturingCompilerOutput(compilerOutput);
        using var services = CompilerServices.BuildServiceProvider(
            options,
            fileSystem: effectiveFileSystem,
            compilerOutput: capturingOutput);

        var compiler = services.GetRequiredService<Compiler>();
        var artifact = compiler.CompileToArtifact(
            request.Entries.Select(entry => new JrocCompileEntry(entry.EntryFilePath, entry.RootModuleIdOverride)).ToArray(),
            request.DefaultEntryFilePath);
        if (artifact is not null)
        {
            return artifact;
        }

        throw new InvalidOperationException(BuildCompilationFailureMessage(capturingOutput));
    }

    private static IFileSystem CreateEffectiveFileSystem(JrocInMemoryMultiEntryCompileRequest request)
    {
        if (!request.Entries.Any(entry => entry.SourceText is not null))
        {
            return request.FileSystem ?? new FileSystem();
        }

        return new OverlayFileSystem(
            request.FileSystem ?? new FileSystem(),
            request.Entries);
    }

    private static string BuildCompilationFailureMessage(CapturingCompilerOutput compilerOutput)
    {
        var message = new StringBuilder("Compilation failed.");

        if (!string.IsNullOrWhiteSpace(compilerOutput.Errors))
        {
            message.AppendLine()
                .AppendLine("Errors:")
                .Append(compilerOutput.Errors.TrimEnd());
        }

        if (!string.IsNullOrWhiteSpace(compilerOutput.Warnings))
        {
            message.AppendLine()
                .AppendLine("Warnings:")
                .Append(compilerOutput.Warnings.TrimEnd());
        }

        if (!string.IsNullOrWhiteSpace(compilerOutput.Output))
        {
            message.AppendLine()
                .AppendLine("Output:")
                .Append(compilerOutput.Output.TrimEnd());
        }

        return message.ToString();
    }

    private static string ResolveTypedModuleId<TExports>(
        JrocInMemoryCompileRequest request,
        JrocCompiledAssemblyArtifact artifact,
        string? explicitModuleId)
        where TExports : class
    {
        var matchedExplicitModuleId = MatchPublishedModuleId(artifact, explicitModuleId);
        if (!string.IsNullOrWhiteSpace(matchedExplicitModuleId))
        {
            return matchedExplicitModuleId;
        }

        var contractType = typeof(TExports);
        var moduleAttribute = contractType.GetCustomAttributes(typeof(JsModuleAttribute), inherit: false)
            .OfType<JsModuleAttribute>()
            .FirstOrDefault();
        var matchedAttributeModuleId = MatchPublishedModuleId(artifact, moduleAttribute?.ModuleId);
        if (!string.IsNullOrWhiteSpace(matchedAttributeModuleId))
        {
            return matchedAttributeModuleId;
        }

        return ResolveModuleId(request, artifact, explicitModuleId: null);
    }

    private static string ResolveModuleId(
        JrocInMemoryCompileRequest request,
        JrocCompiledAssemblyArtifact artifact,
        string? explicitModuleId)
    {
        var matchedExplicitModuleId = MatchPublishedModuleId(artifact, explicitModuleId);
        if (!string.IsNullOrWhiteSpace(matchedExplicitModuleId))
        {
            return matchedExplicitModuleId;
        }

        if (!string.IsNullOrWhiteSpace(artifact.EntryModuleId))
        {
            return artifact.EntryModuleId;
        }

        throw new InvalidOperationException(
            "The compiled artifact does not identify an entry module. " +
            "Pass moduleId explicitly when loading an artifact emitted by an older compiler.");
    }

    private static string? MatchPublishedModuleId(JrocCompiledAssemblyArtifact artifact, string? candidate)
    {
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return null;
        }

        var normalizedCandidate = NormalizeModuleId(candidate);
        return artifact.ModuleIds.FirstOrDefault(moduleId =>
            string.Equals(NormalizeModuleId(moduleId), normalizedCandidate, StringComparison.OrdinalIgnoreCase));
    }

    private static string NormalizeModuleId(string moduleId)
    {
        var normalized = moduleId.Trim().Replace('\\', '/');
        if (normalized.StartsWith("./", StringComparison.Ordinal))
        {
            normalized = normalized[2..];
        }

        if (normalized.StartsWith("/", StringComparison.Ordinal))
        {
            normalized = normalized[1..];
        }

        if (normalized.EndsWith(".js", StringComparison.OrdinalIgnoreCase)
            || normalized.EndsWith(".mjs", StringComparison.OrdinalIgnoreCase)
            || normalized.EndsWith(".cjs", StringComparison.OrdinalIgnoreCase))
        {
            normalized = Path.ChangeExtension(normalized.Replace('/', Path.DirectorySeparatorChar), null) ?? normalized;
            normalized = normalized.Replace('\\', '/');
        }

        return normalized;
    }

    private sealed class CapturingCompilerOutput(ICompilerOutput? inner) : ICompilerOutput
    {
        private readonly StringBuilder _output = new();
        private readonly StringBuilder _warnings = new();
        private readonly StringBuilder _errors = new();

        public string Output => _output.ToString();

        public string Warnings => _warnings.ToString();

        public string Errors => _errors.ToString();

        public void WriteLine(string message)
        {
            _output.AppendLine(message);
            inner?.WriteLine(message);
        }

        public void WriteLine()
        {
            _output.AppendLine();
            inner?.WriteLine();
        }

        public void WriteLineWarning(string message)
        {
            _warnings.AppendLine(message);
            inner?.WriteLineWarning(message);
        }

        public void WriteLineError(string message)
        {
            _errors.AppendLine(message);
            inner?.WriteLineError(message);
        }
    }

    private sealed class OverlayFileSystem : IFileSystem, ISourceFilePathResolver
    {
        private readonly IFileSystem _inner;
        private readonly Dictionary<string, string> _overlays = new(
            OperatingSystem.IsWindows() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);

        public OverlayFileSystem(IFileSystem inner, IReadOnlyList<JrocInMemoryEntrySource> entries)
        {
            _inner = inner;
            foreach (var entry in entries.Where(entry => entry.SourceText is not null))
            {
                var path = NormalizePath(entry.EntryFilePath);
                if (!_overlays.TryAdd(path, entry.SourceText!))
                {
                    throw new ArgumentException($"Entry source '{entry.EntryFilePath}' duplicates an inline source at '{path}'.");
                }
            }
        }

        public string ReadAllText(string path)
        {
            return _overlays.TryGetValue(NormalizePath(path), out var text)
                ? text
                : _inner.ReadAllText(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return _overlays.TryGetValue(NormalizePath(path), out var text)
                ? Encoding.UTF8.GetBytes(text)
                : _inner.ReadAllBytes(path);
        }

        public bool FileExists(string path)
        {
            return _overlays.ContainsKey(NormalizePath(path)) || _inner.FileExists(path);
        }

        public bool TryGetSourceFilePath(string logicalPath, out string sourceFilePath)
        {
            if (!_overlays.ContainsKey(NormalizePath(logicalPath))
                && _inner is ISourceFilePathResolver resolver)
            {
                return resolver.TryGetSourceFilePath(logicalPath, out sourceFilePath);
            }

            sourceFilePath = string.Empty;
            return false;
        }

        private static string NormalizePath(string path)
        {
            var normalized = path.Replace('/', Path.DirectorySeparatorChar);
            return Path.GetFullPath(normalized);
        }
    }
}
