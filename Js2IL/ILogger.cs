namespace Js2IL;

public interface ILogger
{
    void WriteLine(string message);
    void WriteLine();
    void WriteLineWarning(string message);
    void WriteLineError(string message);
}
