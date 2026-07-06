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
            var exports = JsEngine.LoadModule(loadedAssembly.Assembly, resolvedModuleId, options);
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

        var options = new CompilerOptions
        {
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
        var artifact = compiler.CompileToArtifact(request.EntryFilePath, request.RootModuleIdOverride);
        if (artifact is not null)
        {
            return artifact;
        }

        throw new InvalidOperationException(BuildCompilationFailureMessage(capturingOutput));
    }

    private static IFileSystem CreateEffectiveFileSystem(JrocInMemoryCompileRequest request)
    {
        if (request.SourceText is null)
        {
            return request.FileSystem ?? new FileSystem();
        }

        return new OverlayFileSystem(
            request.FileSystem ?? new FileSystem(),
            request.EntryFilePath,
            request.SourceText);
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

        if (artifact.ModuleIds.Count == 1)
        {
            return artifact.ModuleIds[0];
        }

        var matchedRootOverrideModuleId = MatchPublishedModuleId(artifact, request.RootModuleIdOverride);
        if (!string.IsNullOrWhiteSpace(matchedRootOverrideModuleId))
        {
            return matchedRootOverrideModuleId;
        }

        throw new InvalidOperationException(
            "Unable to infer the module id for compile-and-load. " +
            "Pass moduleId explicitly, set RootModuleIdOverride on the compile request, " +
            $"or compile an artifact with a single module id. Available module ids: {string.Join(", ", artifact.ModuleIds)}");
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

    private sealed class OverlayFileSystem(IFileSystem inner, string overlayPath, string overlayContent) : IFileSystem
    {
        private readonly string _overlayPath = NormalizePath(overlayPath);
        private readonly byte[] _overlayBytes = Encoding.UTF8.GetBytes(overlayContent);

        public string ReadAllText(string path)
        {
            return IsOverlayPath(path)
                ? overlayContent
                : inner.ReadAllText(path);
        }

        public byte[] ReadAllBytes(string path)
        {
            return IsOverlayPath(path)
                ? _overlayBytes
                : inner.ReadAllBytes(path);
        }

        public bool FileExists(string path)
        {
            return IsOverlayPath(path) || inner.FileExists(path);
        }

        private bool IsOverlayPath(string path) => string.Equals(_overlayPath, NormalizePath(path), StringComparison.OrdinalIgnoreCase);

        private static string NormalizePath(string path)
        {
            var normalized = path.Replace('/', Path.DirectorySeparatorChar);
            return Path.GetFullPath(normalized);
        }
    }
}
