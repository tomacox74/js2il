using System.Text;

namespace Js2IL.Tests;

/// <summary>
/// Test implementation of ILogger that captures all output for test verification.
/// </summary>
public class TestLogger : ILogger
{
    private readonly StringBuilder _output = new();
    private readonly StringBuilder _warnings = new();
    private readonly StringBuilder _errors = new();

    public string Output => _output.ToString();
    public string Warnings => _warnings.ToString();
    public string Errors => _errors.ToString();

    public void WriteLine(string message)
    {
        _output.AppendLine(message);
    }

    public void WriteLine()
    {
        _output.AppendLine();
    }

    public void WriteLineWarning(string message)
    {
        _warnings.AppendLine(message);
    }

    public void WriteLineError(string message)
    {
        _errors.AppendLine(message);
    }

    public void Clear()
    {
        _output.Clear();
        _warnings.Clear();
        _errors.Clear();
    }
}
