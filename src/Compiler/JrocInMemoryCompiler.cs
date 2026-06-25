using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Jroc;

public static class JrocInMemoryCompiler
{
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
            GenerateModuleExportContracts = request.GenerateModuleExportContracts
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
