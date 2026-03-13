using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace Js2IL.SDK.BuildTasks;

internal sealed class MsBuildCompilerOutput(TaskLoggingHelper log, string sourcePath) : ICompilerOutput
{
    private readonly string _sourceLabel = Path.GetFileName(sourcePath);

    public void WriteLine(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            log.LogMessage(MessageImportance.Low, string.Empty);
            return;
        }

        log.LogMessage(MessageImportance.High, $"[{_sourceLabel}] {message}");
    }

    public void WriteLine()
    {
        log.LogMessage(MessageImportance.Low, string.Empty);
    }

    public void WriteLineWarning(string message)
    {
        log.LogWarning(
            subcategory: null,
            warningCode: null,
            helpKeyword: null,
            file: sourcePath,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            message: TrimKnownPrefix(message, "Warning:"));
    }

    public void WriteLineError(string message)
    {
        log.LogError(
            subcategory: null,
            errorCode: null,
            helpKeyword: null,
            file: sourcePath,
            lineNumber: 0,
            columnNumber: 0,
            endLineNumber: 0,
            endColumnNumber: 0,
            message: TrimKnownPrefix(message, "Error:"));
    }

    private static string TrimKnownPrefix(string message, string prefix)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return string.Empty;
        }

        return message.StartsWith(prefix, StringComparison.OrdinalIgnoreCase)
            ? message[prefix.Length..].TrimStart()
            : message;
    }
}
