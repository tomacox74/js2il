namespace Js2IL;

/// <summary>
/// Optional extension for <see cref="IFileSystem"/> implementations that can provide a
/// stable, on-disk source path for a logical/virtual file path.
/// </summary>
public interface ISourceFilePathResolver
{
    /// <summary>
    /// Returns the preferred source file path to be recorded in debug symbols (PDB)
    /// for the provided logical path.
    /// </summary>
    bool TryGetSourceFilePath(string logicalPath, out string sourceFilePath);
}
