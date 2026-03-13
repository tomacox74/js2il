namespace Js2IL;

/// <summary>
/// Default implementation of IFileSystem that uses System.IO.File.
/// </summary>
public class FileSystem : IFileSystem
{
    public string ReadAllText(string path) => File.ReadAllText(path);

    public bool FileExists(string path) => File.Exists(path);
}
