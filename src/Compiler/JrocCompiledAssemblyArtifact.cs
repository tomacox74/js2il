namespace Jroc;

public sealed record JrocCompiledAssemblyArtifact(
    string AssemblyName,
    byte[] PeBytes,
    byte[]? PdbBytes,
    IReadOnlyList<string> ModuleIds);
