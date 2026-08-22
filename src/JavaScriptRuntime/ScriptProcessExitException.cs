using System.ComponentModel;

namespace JavaScriptRuntime;

internal sealed class ScriptProcessExitException : Exception;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class ScriptProcessExitControl
{
    private static readonly AsyncLocal<ScriptProcessExitException?> PendingExit = new();

    internal static void RequestExit()
    {
        var exception = new ScriptProcessExitException();
        PendingExit.Value = exception;
        throw exception;
    }

    public static void ThrowIfRequested()
    {
        if (PendingExit.Value is { } exception)
        {
            throw exception;
        }
    }

    internal static void Clear()
    {
        PendingExit.Value = null;
    }
}
