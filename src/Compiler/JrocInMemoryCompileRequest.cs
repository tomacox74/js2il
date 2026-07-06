namespace Jroc;

public sealed record JrocInMemoryCompileRequest(string EntryFilePath)
{
    public string? SourceText { get; init; }

    public IFileSystem? FileSystem { get; init; }

    public string? RootModuleIdOverride { get; init; }

    public bool EmitPdb { get; init; }

    public bool Verbose { get; init; }

    public string? DiagnosticFilePath { get; init; }

    public bool AnalyzeUnused { get; init; }

    public bool GenerateModuleExportContracts { get; init; } = true;

    public JavaScriptRuntime.HostRuntimeIntrinsicDescriptors HostRuntimeIntrinsics { get; init; } =
        JavaScriptRuntime.HostRuntimeIntrinsicDescriptors.Empty;
}
