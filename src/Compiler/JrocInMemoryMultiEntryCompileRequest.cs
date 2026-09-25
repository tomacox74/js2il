namespace Jroc;

/// <summary>An independent entry source, with optional inline text and logical module-id alias.</summary>
public sealed record JrocInMemoryEntrySource(
    string EntryFilePath,
    string? SourceText = null,
    string? RootModuleIdOverride = null);

/// <summary>Compiles several independent entry sources into one in-memory assembly.</summary>
public sealed record JrocInMemoryMultiEntryCompileRequest(IReadOnlyList<JrocInMemoryEntrySource> Entries)
{
    public string? AssemblyName { get; init; }

    public string? DefaultEntryFilePath { get; init; }

    public IFileSystem? FileSystem { get; init; }

    public bool EmitPdb { get; init; }

    public bool Verbose { get; init; }

    public string? DiagnosticFilePath { get; init; }

    public bool AnalyzeUnused { get; init; }

    public bool GenerateModuleExportContracts { get; init; } = true;

    public JavaScriptRuntime.HostRuntimeIntrinsicDescriptors HostRuntimeIntrinsics { get; init; } =
        JavaScriptRuntime.HostRuntimeIntrinsicDescriptors.Empty;
}
