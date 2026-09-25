namespace Jroc;

/// <summary>A source file to include as an independent entry in a compiled assembly.</summary>
public sealed record JrocCompileEntry(string EntryFilePath, string? RootModuleIdOverride = null);
