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
        IEnvironment _environment;

        public Process(IEnvironment environment)
        {
            _environment = environment;
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
