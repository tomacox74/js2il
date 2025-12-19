namespace Js2IL.Tests;

/// <summary>
/// Mock file system implementation for testing that stores files in memory.
/// </summary>
public class MockFileSystem : IFileSystem
{
    private readonly Dictionary<string, string> _files = new();

    /// <summary>
    /// Adds a file to the mock file system.
    /// </summary>
    public void AddFile(string path, string content)
    {
        _files[path] = content;
    }

    /// <summary>
    /// Reads all text from a file. Falls back to real file system if file not found in mock.
    /// </summary>
    public string ReadAllText(string path)
    {
        if (_files.TryGetValue(path, out var content))
        {
            return content;
        }
        // Fall back to real file system for other files
        return File.ReadAllText(path);
    }

    /// <summary>
    /// Checks if a file exists in mock or real file system.
    /// </summary>
    public bool FileExists(string path)
    {
        if (_files.ContainsKey(path))
        {
            return true;
        }
        return File.Exists(path);
    }
}
