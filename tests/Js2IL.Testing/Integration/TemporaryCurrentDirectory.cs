namespace Js2IL.Tests.Integration;

public sealed class TemporaryCurrentDirectory : IDisposable
{
    private readonly string _previousCurrentDirectory;

    public string Path { get; }

    public TemporaryCurrentDirectory()
    {
        _previousCurrentDirectory = Environment.CurrentDirectory;
        Path = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Js2IL.Tests", "Integration", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(Path);
        Environment.CurrentDirectory = Path;
    }

    public void Dispose()
    {
        Environment.CurrentDirectory = _previousCurrentDirectory;
        try
        {
            Directory.Delete(Path, recursive: true);
        }
        catch
        {
        }
    }
}
