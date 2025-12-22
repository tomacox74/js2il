namespace Js2IL;

/// <summary>
/// Abstraction for file system operations.
/// </summary>
public interface IFileSystem
{
    /// <summary>
    /// Reads all text from a file.
    /// </summary>
    string ReadAllText(string path);

    /// <summary>
    /// Checks if a file exists.
    /// </summary>
    bool FileExists(string path);
}
