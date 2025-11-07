using System;
using System.Linq;

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
        /// Matches Node's writable process.exitCode (JavaScript number). Internally mirrors host Environment.ExitCode.
        /// Getter returns the current exit code as a double; setter accepts a double and truncates to int.
        /// </summary>
        public double exitCode
        {
            get => (double)JavaScriptRuntime.EnvironmentProvider.Current.ExitCode;
            set => JavaScriptRuntime.EnvironmentProvider.Current.ExitCode = (int)value;
        }

        /// <summary>
        /// Minimal process.argv derived from the active environment. argv[0] is normalized to the current script filename.
        /// </summary>
        public JavaScriptRuntime.Array argv
        {
            get
            {
                try
                {
                    var args = JavaScriptRuntime.EnvironmentProvider.GetProcessArgs();
                    if (args != null && args.Length > 0)
                    {
                        var normalized = (string[])args.Clone();
                        normalized[0] = JavaScriptRuntime.GlobalVariables.__filename;
                        return new JavaScriptRuntime.Array(normalized.Select(s => (object)s));
                    }
                }
                catch { }

                // Fallback to single entry with script filename
                return new JavaScriptRuntime.Array(new object[] { JavaScriptRuntime.GlobalVariables.__filename });
            }
        }

        /// <summary>
        /// Immediately terminates the current process with the specified exit code.
        /// Overload without arguments uses the current Environment.ExitCode.
        /// </summary>
        public void exit()
        {
            // Record the current exit code for deterministic testing when termination is suppressed
            try { JavaScriptRuntime.EnvironmentProvider.LastExitCodeSet = JavaScriptRuntime.EnvironmentProvider.Current.ExitCode; } catch { }
            JavaScriptRuntime.EnvironmentProvider.Current.Exit();
        }

        /// <summary>
        /// Immediately terminates the current process with the provided code (coerced to int).
        /// </summary>
        public void exit(object? code)
        {
            int ec;
            try
            {
                ec = (int)JavaScriptRuntime.TypeUtilities.ToNumber(code);
            }
            catch
            {
                ec = 0;
            }
            // Record explicitly provided exit code
            try { JavaScriptRuntime.EnvironmentProvider.LastExitCodeSet = ec; } catch { }
            JavaScriptRuntime.EnvironmentProvider.Current.ExitCode = ec;
            JavaScriptRuntime.EnvironmentProvider.Current.Exit(ec);
        }
    }
}
