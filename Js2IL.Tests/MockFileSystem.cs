namespace Js2IL.Tests;

/// <summary>
/// Mock file system implementation for testing that stores files in memory.
/// </summary>
public class MockFileSystem : IFileSystem
	, ISourceFilePathResolver
{
    private readonly Dictionary<string, string> _files = new(StringComparer.OrdinalIgnoreCase);

    private readonly Dictionary<string, string> _sourceFilePaths = new(StringComparer.OrdinalIgnoreCase);

    private static string NormalizePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return path;
        }

        // Module resolution may produce paths with either '/' or '\\' separators.
        // Normalize so lookups consistently hit the in-memory store.
        var normalized = path.Replace('/', Path.DirectorySeparatorChar);

        try
        {
            normalized = Path.GetFullPath(normalized);
        }
        catch
        {
            // If GetFullPath fails (unexpected), fall back to separator-normalized string.
        }

        return normalized;
    }

    /// <summary>
    /// Adds a file to the mock file system.
    /// </summary>
    public void AddFile(string path, string content)
    {
        _files[NormalizePath(path)] = content;
    }

    /// <summary>
    /// Adds a file to the mock file system, with an optional on-disk source path to be used for debugging symbols.
    /// </summary>
    public void AddFile(string path, string content, string? sourceFilePath)
    {
        var normalized = NormalizePath(path);
        _files[normalized] = content;

        if (!string.IsNullOrWhiteSpace(sourceFilePath))
        {
            _sourceFilePaths[normalized] = sourceFilePath;
        }
    }

    public bool TryGetSourceFilePath(string logicalPath, out string sourceFilePath)
    {
        var normalized = NormalizePath(logicalPath);
        return _sourceFilePaths.TryGetValue(normalized, out sourceFilePath!);
    }

    /// <summary>
    /// Reads all text from a file. Falls back to real file system if file not found in mock.
    /// </summary>
    public string ReadAllText(string path)
    {
        var normalized = NormalizePath(path);
        if (_files.TryGetValue(normalized, out var content))
        {
            return content;
        }
        // Fall back to real file system for other files
        return File.ReadAllText(normalized);
    }

    /// <summary>
    /// Checks if a file exists in mock or real file system.
    /// </summary>
    public bool FileExists(string path)
    {
        var normalized = NormalizePath(path);
        if (_files.ContainsKey(normalized))
        {
            return true;
        }
        return File.Exists(normalized);
    }
}
