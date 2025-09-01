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
        private JavaScriptRuntime.Array _argv = new JavaScriptRuntime.Array();

        /// <summary>
        /// Matches Node's writable process.exitCode (JavaScript number). Internally mirrors host Environment.ExitCode.
        /// Getter returns the current exit code as a double; setter accepts a double and truncates to int.
        /// </summary>
        public double exitCode
        {
            get => (double)JavaScriptRuntime.EnvironmentProvider.Current.ExitCode;
            set => JavaScriptRuntime.EnvironmentProvider.Current.ExitCode = (int)value;
        }

        /// <summary>
        /// Minimal process.argv. Initialized by the runtime when a module context is set.
        /// </summary>
        public JavaScriptRuntime.Array argv => _argv;

        /// <summary>
        /// Initialize argv values.
        /// </summary>
        public void SetArgv(params string[] items)
        {
            var arr = new JavaScriptRuntime.Array(items?.Select(s => (object)s) ?? System.Array.Empty<object>());
            _argv = arr;
        }
    }
}
