using DiagnosticsProcess = System.Diagnostics.Process;
using DiagnosticsProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace JavaScriptRuntime.Node;

public interface IChildProcessLauncher
{
    DiagnosticsProcess Start(ChildProcessLaunchRequest request);
}

public sealed class ChildProcessLaunchRequest
{
    public required string CompiledAssemblyPath { get; init; }

    public required string EntryModule { get; init; }

    public required IReadOnlyList<string> ModuleArguments { get; init; }

    public required bool HostedParent { get; init; }

    public required DiagnosticsProcessStartInfo StartInfo { get; init; }
}

internal sealed class DefaultChildProcessLauncher : IChildProcessLauncher
{
    public DiagnosticsProcess Start(ChildProcessLaunchRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.StartInfo);

        return DiagnosticsProcess.Start(request.StartInfo)
            ?? throw new Error($"Failed to start process '{request.StartInfo.FileName}'.");
    }
}
