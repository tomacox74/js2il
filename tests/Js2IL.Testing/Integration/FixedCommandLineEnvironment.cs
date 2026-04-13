using JavaScriptRuntime;

namespace Js2IL.Tests.Integration;

public sealed class FixedCommandLineEnvironment : IEnvironment
{
    private readonly string[] _args;

    public FixedCommandLineEnvironment(params string[] args)
    {
        _args = args;
    }

    public int ExitCode { get; set; }

    public string[] GetCommandLineArgs() => _args;

    public void Exit(int code)
    {
        ExitCode = code;
    }

    public void Exit()
    {
    }
}
