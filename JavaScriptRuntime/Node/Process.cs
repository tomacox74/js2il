using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Runtime.InteropServices;

namespace JavaScriptRuntime.Node
{
    /// <summary>
    /// Minimal Node-like process object. Currently exposes writable exitCode
    /// that mirrors the host process Environment.ExitCode.
    /// </summary>
    [NodeModule("process")]
    public sealed class Process
    {
        private static readonly Lazy<string> _platform = new(DetectPlatform);
        IEnvironment _environment;
        private readonly Lazy<object> _versions;
        private readonly Lazy<object> _env;

        public Process(IEnvironment environment)
        {
            _environment = environment;
            _versions = new Lazy<object>(CreateVersions);
            _env = new Lazy<object>(CreateEnvSnapshot);
        }

        /// <summary>
        /// Matches Node's writable process.exitCode (JavaScript number). Internally mirrors host Environment.ExitCode.
        /// Getter returns the current exit code as a double; setter accepts a double and truncates to int.
        /// </summary>
        public double exitCode
        {
            get => (double)_environment.ExitCode;
            set => _environment.ExitCode = (int)value;
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
                    var scriptFile = JavaScriptRuntime.CommonJS.ModuleContext.CreateModuleContext().__filename;

                    // Node semantics:
                    //   argv[0] = execPath (node)
                    //   argv[1] = script path
                    //   argv[2..] = user args
                    // Our host may provide args as either:
                    //   [dotnet, script.dll, ...]
                    //   [script.dll, ...]
                    // Normalize both forms.
                    if (args != null && args.Length > 1)
                    {
                        // If the host provides execPath (dotnet) + script, keep execPath.
                        // Otherwise, treat args[0] as script and preserve all user args.
                        var first = args[0] ?? string.Empty;
                        var fileName = string.Empty;
                        try
                        {
                            fileName = System.IO.Path.GetFileName(first);
                        }
                        catch { }

                        bool hasExecPath = fileName.Equals("dotnet", StringComparison.OrdinalIgnoreCase)
                            || fileName.Equals("dotnet.exe", StringComparison.OrdinalIgnoreCase);

                        if (hasExecPath)
                        {
                            var normalized = new object[args.Length];
                            normalized[0] = args[0];
                            normalized[1] = scriptFile;
                            for (int i = 2; i < args.Length; i++)
                            {
                                normalized[i] = args[i];
                            }
                            return new JavaScriptRuntime.Array(normalized);
                        }

                        // Host omitted execPath: args = [script, ...userArgs]
                        // Normalize to ["dotnet", script, ...userArgs]
                        var outArgs = new object[args.Length + 1];
                        outArgs[0] = "dotnet";
                        outArgs[1] = scriptFile;
                        for (int i = 1; i < args.Length; i++)
                        {
                            outArgs[i + 1] = args[i];
                        }
                        return new JavaScriptRuntime.Array(outArgs);
                    }

                    if (args != null && args.Length == 1)
                    {
                        // Host provided only the script path.
                        return new JavaScriptRuntime.Array(new object[] { "dotnet", scriptFile });
                    }
                }
                catch { }

                // Fallback to Node-like argv with just execPath + script
                return new JavaScriptRuntime.Array(new object[] { "dotnet", JavaScriptRuntime.CommonJS.ModuleContext.CreateModuleContext().__filename });
            }
        }

        public string cwd()
        {
            try
            {
                return System.Environment.CurrentDirectory;
            }
            catch
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Minimal Node-compatible platform identifier.
        /// Returns values like "win32", "linux", or "darwin".
        /// </summary>
        public string platform
        {
            get => _platform.Value;
        }

        /// <summary>
        /// Minimal process.versions object with Node version identity.
        /// </summary>
        public object versions
        {
            get => _versions.Value;
        }

        /// <summary>
        /// Snapshot of host environment variables as a JS object.
        /// </summary>
        public object env
        {
            get => _env.Value;
        }

        private static string DetectPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return "win32";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return "darwin";
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return "linux";
            }

            return "unknown";
        }

        private static object CreateVersions()
        {
            var result = new ExpandoObject();
            var dict = (IDictionary<string, object?>)result;
            dict["node"] = "22.0.0";
            return result;
        }

        private static object CreateEnvSnapshot()
        {
            var result = new ExpandoObject();
            var dict = (IDictionary<string, object?>)result;

            foreach (DictionaryEntry entry in System.Environment.GetEnvironmentVariables())
            {
                var key = entry.Key?.ToString();
                if (string.IsNullOrWhiteSpace(key))
                {
                    continue;
                }

                dict[key] = entry.Value?.ToString() ?? string.Empty;
            }

            return result;
        }

        /// <summary>
        /// Changes the current working directory.
        /// </summary>
        public object? chdir(object? directory)
        {
            if (directory is null || directory is JsNull)
            {
                throw new TypeError("The \"directory\" argument must be of type string.");
            }

            var target = DotNet2JSConversions.ToString(directory);
            if (string.IsNullOrWhiteSpace(target))
            {
                throw new TypeError("The \"directory\" argument must be a non-empty string.");
            }

            try
            {
                System.Environment.CurrentDirectory = target;
                return null;
            }
            catch (Exception ex)
            {
                throw new Error(ex.Message);
            }
        }

        /// <summary>
        /// Queues a callback to run on the next turn, before later scheduled immediates.
        /// </summary>
        public object? nextTick(object callback, params object[] args)
        {
            var tickArgs = args ?? System.Array.Empty<object>();
            _ = GlobalThis.setImmediate(callback, tickArgs);
            return null;
        }

        /// <summary>
        /// Immediately terminates the current process with the specified exit code.
        /// Overload without arguments uses the current Environment.ExitCode.
        /// </summary>
        public void exit()
        {
            _environment.Exit();
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
            _environment.ExitCode = ec;
            _environment.Exit(ec);
        }
    }
}
