namespace Js2IL;

public interface ICompilerOutput
{
    void WriteLine(string message);
    void WriteLine();
    void WriteLineWarning(string message);
    void WriteLineError(string message);
}

// Backward-compatible alias while callers migrate.
public interface ICompilerUx : ICompilerOutput
{
}

// Backward-compatible alias while callers migrate.
public interface ILogger : ICompilerOutput
{
}
