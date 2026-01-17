using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Runtime.InteropServices;
using System.Text;
using JavaScriptRuntime;

namespace JavaScriptRuntime.Node
{
    [NodeModule("child_process")]
    public sealed class ChildProcess
    {
        public object spawnSync(object command, object? args = null, object? options = null)
        {
            var commandText = command?.ToString() ?? string.Empty;
            var argList = CoerceArgs(args);

            var cwd = TryGetStringOption(options, "cwd");
            var shell = TryGetBoolOption(options, "shell");
            var stdio = TryGetOption(options, "stdio");

            bool inherit = IsInheritStdio(stdio);

            try
            {
                var psi = BuildStartInfo(commandText, argList, cwd, shell, redirectOutput: !inherit);

                using var p = System.Diagnostics.Process.Start(psi) ?? throw new Error("Failed to start process.");

                string? stdout = null;
                string? stderr = null;

                if (!inherit)
                {
                    stdout = p.StandardOutput.ReadToEnd();
                    stderr = p.StandardError.ReadToEnd();
                }

                p.WaitForExit();

                dynamic result = new ExpandoObject();
                result.status = (double)p.ExitCode;
                result.stdout = stdout;
                result.stderr = stderr;
                return result;
            }
            catch (Exception ex)
            {
                dynamic result = new ExpandoObject();
                result.status = (double)(-1);
                result.stdout = null;
                result.stderr = ex.Message;
                result.error = ex;
                return result;
            }
        }

        public object execSync(object command, object? options = null)
        {
            var commandText = command?.ToString() ?? string.Empty;
            var cwd = TryGetStringOption(options, "cwd");
            var encoding = TryGetStringOption(options, "encoding");
            var stdio = TryGetOption(options, "stdio");

            bool inherit = IsInheritStdio(stdio);

            var psi = BuildStartInfo(commandText, args: System.Array.Empty<string>(), cwd, shell: true, redirectOutput: !inherit);

            using var p = System.Diagnostics.Process.Start(psi) ?? throw new Error("Failed to start process.");

            string stdout = string.Empty;
            string stderr = string.Empty;

            if (!inherit)
            {
                stdout = p.StandardOutput.ReadToEnd();
                stderr = p.StandardError.ReadToEnd();
            }

            p.WaitForExit();

            if (p.ExitCode != 0)
            {
                throw new ExecSyncError(commandText, p.ExitCode, stdout, stderr);
            }

            // For now we only support string output; Node returns Buffer when no encoding is provided.
            // Our internal scripts pass encoding: 'utf8'.
            _ = encoding;
            return stdout;
        }

        public sealed class ExecSyncError : Error
        {
            public ExecSyncError(string command, int exitCode, string stdout, string stderr)
                : base($"Command failed: {command} (exit code {exitCode})")
            {
                status = (double)exitCode;
                this.stdout = stdout;
                this.stderr = stderr;
            }

            // Lowercase to match Node-ish shape
            public double status { get; }
            public object stdout { get; }
            public object stderr { get; }
        }

        private static ProcessStartInfo BuildStartInfo(string command, IReadOnlyList<string> args, string? cwd, bool shell, bool redirectOutput)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardOutput = redirectOutput,
                RedirectStandardError = redirectOutput,
                RedirectStandardInput = false,
                CreateNoWindow = true,
            };

            if (!string.IsNullOrWhiteSpace(cwd))
            {
                psi.WorkingDirectory = cwd;
            }

            if (shell)
            {
                var full = BuildShellCommand(command, args);

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    psi.FileName = "cmd.exe";
                    psi.ArgumentList.Add("/c");
                    psi.ArgumentList.Add(full);
                }
                else
                {
                    psi.FileName = "/bin/sh";
                    psi.ArgumentList.Add("-c");
                    psi.ArgumentList.Add(full);
                }

                return psi;
            }

            psi.FileName = command;
            foreach (var a in args)
            {
                psi.ArgumentList.Add(a ?? string.Empty);
            }

            return psi;
        }

        private static string BuildShellCommand(string command, IReadOnlyList<string> args)
        {
            var sb = new StringBuilder();
            sb.Append(command);
            for (int i = 0; i < args.Count; i++)
            {
                sb.Append(' ');
                sb.Append(QuoteArg(args[i] ?? string.Empty));
            }
            return sb.ToString();
        }

        private static string QuoteArg(string arg)
        {
            if (arg.Length == 0) return "\"\"";
            if (arg.IndexOfAny(new[] { ' ', '\t', '"' }) < 0) return arg;
            return "\"" + arg.Replace("\"", "\\\"") + "\"";
        }

        private static List<string> CoerceArgs(object? args)
        {
            var list = new List<string>();

            if (args == null)
            {
                return list;
            }

            if (args is JavaScriptRuntime.Array arr)
            {
                for (int i = 0; i < (int)arr.length; i++)
                {
                    list.Add(arr[i]?.ToString() ?? string.Empty);
                }

                return list;
            }

            if (args is object[] oa)
            {
                for (int i = 0; i < oa.Length; i++)
                {
                    list.Add(oa[i]?.ToString() ?? string.Empty);
                }

                return list;
            }

            // Fallback: treat as a single argument
            list.Add(args.ToString() ?? string.Empty);
            return list;
        }

        private static object? TryGetOption(object? options, string name)
        {
            if (options == null) return null;

            try
            {
                if (options is ExpandoObject exp)
                {
                    var dict = (System.Collections.Generic.IDictionary<string, object?>)exp;
                    if (dict.TryGetValue(name, out var val)) return val;
                }

                return JavaScriptRuntime.Object.GetProperty(options, name);
            }
            catch
            {
                return null;
            }
        }

        private static string? TryGetStringOption(object? options, string name)
        {
            var v = TryGetOption(options, name);
            return v?.ToString();
        }

        private static bool TryGetBoolOption(object? options, string name)
        {
            var v = TryGetOption(options, name);
            if (v == null) return false;
            try
            {
                return JavaScriptRuntime.TypeUtilities.ToBoolean(v);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsInheritStdio(object? stdio)
        {
            if (stdio == null) return false;
            if (stdio is string s)
            {
                return s.Equals("inherit", StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
