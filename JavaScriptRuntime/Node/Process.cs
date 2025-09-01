using System;

namespace JavaScriptRuntime.Node
{
    /// <summary>
    /// Minimal Node-like process object. Currently exposes writable exitCode
    /// that mirrors the host process Environment.ExitCode.
    /// </summary>
    [NodeModule("process")]
    public sealed class Process
    {
        /// <summary>
        /// Matches Node's writable process.exitCode and mirrors the host process exit code.
        /// Getter returns <see cref="Environment.ExitCode"/>; setter updates it.
        /// </summary>
        public int exitCode
        {
            get => JavaScriptRuntime.EnvironmentProvider.Current.ExitCode;
            set => JavaScriptRuntime.EnvironmentProvider.Current.ExitCode = value;
        }
    }
}
