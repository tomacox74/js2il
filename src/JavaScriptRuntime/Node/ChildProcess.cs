using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;
using DiagnosticsProcess = System.Diagnostics.Process;
using DiagnosticsProcessStartInfo = System.Diagnostics.ProcessStartInfo;

namespace JavaScriptRuntime.Node
{
    [NodeModule("child_process")]
    public sealed class ChildProcess
    {
        private IIOScheduler? _ioScheduler;
        private NodeSchedulerState? _nodeScheduler;

        private IIOScheduler IoScheduler => _ioScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<IIOScheduler>()
                ?? throw new InvalidOperationException("IIOScheduler is not available for child_process.");

        private NodeSchedulerState NodeScheduler => _nodeScheduler
            ??= GlobalThis.ServiceProvider?.Resolve<NodeSchedulerState>()
                ?? throw new InvalidOperationException("NodeSchedulerState is not available for child_process.");

        public object spawnSync(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"file\" argument must be a non-empty string");
            }

            var command = srcArgs[0];
            object? commandArgs = null;
            object? options = null;

            if (srcArgs.Length > 1)
            {
                if (IsArgumentList(srcArgs[1]))
                {
                    commandArgs = srcArgs[1];
                }
                else
                {
                    options = srcArgs[1];
                }
            }

            if (srcArgs.Length > 2)
            {
                options = srcArgs[2];
            }

            return spawnSync(command, commandArgs, options);
        }

        public object spawnSync(object command, object? args = null, object? options = null)
        {
            var commandText = command?.ToString() ?? string.Empty;
            var argList = CoerceArgs(args);
            var cwd = TryGetStringOption(options, "cwd");
            var shell = TryGetBoolOption(options, "shell");
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.SyncDefault, allowIpc: false, apiName: "child_process.spawnSync");

            try
            {
                var psi = BuildStartInfo(commandText, argList, cwd, shell, stdio);

                using var p = DiagnosticsProcess.Start(psi) ?? throw new Error("Failed to start process.");
                if (stdio.StdinMode == StdioMode.Ignore)
                {
                    p.StandardInput.Close();
                }

                var completion = WaitForProcessCompletionSync(p, stdio);

                dynamic result = new ExpandoObject();
                result.status = (double)completion.ExitCode;
                result.stdout = stdio.StdoutMode == StdioMode.Pipe ? completion.Stdout : null;
                result.stderr = stdio.StderrMode == StdioMode.Pipe ? completion.Stderr : null;
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

        public object spawn(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"file\" argument must be a non-empty string");
            }

            var command = srcArgs[0];
            object? commandArgs = null;
            object? options = null;

            if (srcArgs.Length > 1)
            {
                if (IsArgumentList(srcArgs[1]))
                {
                    commandArgs = srcArgs[1];
                }
                else
                {
                    options = srcArgs[1];
                }
            }

            if (srcArgs.Length > 2)
            {
                options = srcArgs[2];
            }

            return spawn(command, commandArgs, options);
        }

        public object spawn(object command, object? args = null, object? options = null)
            => StartChildProcess(command, args, options, shellOverride: null, callback: null, suppressUnhandledError: false);

        public object execSync(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"command\" argument must be a non-empty string");
            }

            var command = srcArgs[0];
            var options = srcArgs.Length > 1 ? srcArgs[1] : null;
            return execSync(command, options);
        }

        public object execSync(object command, object? options = null)
        {
            var commandText = command?.ToString() ?? string.Empty;
            var cwd = TryGetStringOption(options, "cwd");
            var encoding = TryGetStringOption(options, "encoding");
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.SyncDefault, allowIpc: false, apiName: "child_process.execSync");

            var psi = BuildStartInfo(commandText, args: System.Array.Empty<string>(), cwd, shell: true, stdio);

            using var p = DiagnosticsProcess.Start(psi) ?? throw new Error("Failed to start process.");
            if (stdio.StdinMode == StdioMode.Ignore)
            {
                p.StandardInput.Close();
            }

            var completion = WaitForProcessCompletionSync(p, stdio);
            var stdout = completion.Stdout;
            var stderr = completion.Stderr;

            if (completion.ExitCode != 0)
            {
                throw new ChildProcessError(commandText, completion.ExitCode, stdout, stderr);
            }

            // For now we only support string output; Node returns Buffer when no encoding is provided.
            // Our internal scripts pass encoding: 'utf8' when they need a specific encoding.
            _ = encoding;
            return stdout;
        }

        public object exec(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"command\" argument must be a non-empty string");
            }

            var command = srcArgs[0];
            object? options = null;
            object? callback = null;

            if (srcArgs.Length > 1)
            {
                if (srcArgs[1] is Delegate)
                {
                    callback = srcArgs[1];
                }
                else
                {
                    options = srcArgs[1];
                }
            }

            if (srcArgs.Length > 2)
            {
                callback = srcArgs[2];
            }

            return exec(command, options, callback);
        }

        public object exec(object command, object? options = null, object? callback = null)
        {
            var cb = ValidateCallback(callback);
            return StartChildProcess(command, args: null, options, shellOverride: true, callback: cb, suppressUnhandledError: cb != null);
        }

        public object execFile(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"file\" argument must be a non-empty string");
            }

            var file = srcArgs[0];
            object? fileArgs = null;
            object? options = null;
            object? callback = null;

            if (srcArgs.Length > 1)
            {
                if (srcArgs[1] is Delegate)
                {
                    callback = srcArgs[1];
                }
                else if (IsArgumentList(srcArgs[1]))
                {
                    fileArgs = srcArgs[1];
                }
                else
                {
                    options = srcArgs[1];
                }
            }

            if (srcArgs.Length > 2)
            {
                if (srcArgs[2] is Delegate)
                {
                    callback = srcArgs[2];
                }
                else
                {
                    options = srcArgs[2];
                }
            }

            if (srcArgs.Length > 3)
            {
                callback = srcArgs[3];
            }

            return execFile(file, fileArgs, options, callback);
        }

        public object execFile(object file, object? args = null, object? options = null, object? callback = null)
        {
            var cb = ValidateCallback(callback);
            return StartChildProcess(file, args, options, shellOverride: false, callback: cb, suppressUnhandledError: cb != null);
        }

        public object fork(object[] args)
        {
            var srcArgs = args ?? System.Array.Empty<object>();
            if (srcArgs.Length == 0)
            {
                throw new TypeError("The \"modulePath\" argument must be a non-empty string");
            }

            var modulePath = srcArgs[0];
            object? moduleArgs = null;
            object? options = null;

            if (srcArgs.Length > 1)
            {
                if (IsArgumentList(srcArgs[1]))
                {
                    moduleArgs = srcArgs[1];
                }
                else
                {
                    options = srcArgs[1];
                }
            }

            if (srcArgs.Length > 2)
            {
                options = srcArgs[2];
            }

            return fork(modulePath, moduleArgs, options);
        }

        public object fork(object modulePath, object? args = null, object? options = null)
        {
            var entryModule = modulePath?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(entryModule))
            {
                throw new TypeError("The \"modulePath\" argument must be a non-empty string");
            }

            if (IsHostedRuntime())
            {
                throw new Error("child_process.fork is not supported when running under JsEngine hosting yet. See issue #914 for hosted fork support.");
            }

            if (TryGetBoolOption(options, "detached"))
            {
                throw new Error("child_process.fork does not support detached child processes in the current runtime.");
            }

            var serialization = TryGetStringOption(options, "serialization");
            if (!string.IsNullOrWhiteSpace(serialization)
                && !serialization.Equals("json", StringComparison.OrdinalIgnoreCase))
            {
                throw new Error("child_process.fork only supports JSON message serialization in the current runtime.");
            }

            var serviceProvider = GlobalThis.ServiceProvider
                ?? throw new InvalidOperationException("GlobalThis.ServiceProvider is not available for child_process.fork.");
            var assemblyPath = CommonJS.ModuleContext.CreateModuleContext(serviceProvider).__filename;
            if (string.IsNullOrWhiteSpace(assemblyPath) || !File.Exists(assemblyPath))
            {
                throw new Error("child_process.fork requires the current compiled assembly path to be available.");
            }

            var childArgs = new List<string>(capacity: 1 + CoerceArgs(args).Count)
            {
                assemblyPath
            };
            childArgs.AddRange(CoerceArgs(args));

            var cwd = TryGetStringOption(options, "cwd");
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.ForkDefault, allowIpc: true, apiName: "child_process.fork");
            if (!stdio.IpcEnabled)
            {
                throw new Error("child_process.fork requires an IPC channel. Include 'ipc' in stdio[3] or omit stdio to use the default fork configuration.");
            }

            var child = new ChildProcessHandle(stdio, QueueImmediate);
            var envOverrides = ParseEnvironmentOverrides(TryGetOption(options, "env"));
            envOverrides[ChildProcessRuntimeOptions.ForkEntryModuleEnvVar] = entryModule;
            var ipcToken = CreateIpcAuthenticationToken();

            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var ipcPort = ((IPEndPoint)listener.LocalEndpoint).Port;
            envOverrides[ChildProcessRuntimeOptions.ForkIpcPortEnvVar] = ipcPort.ToString();
            envOverrides[ChildProcessRuntimeOptions.ForkIpcTokenEnvVar] = ipcToken;

            try
            {
                var psi = BuildStartInfo(
                    command: "dotnet",
                    args: childArgs,
                    cwd,
                    shell: false,
                    stdio,
                    envOverrides);

                var completion = CreateCompletionPromise(child, assemblyPath, callback: null, suppressUnhandledError: false);
                var process = DiagnosticsProcess.Start(psi) ?? throw new Error("Failed to start process.");
                try
                {
                    child.Attach(process);
                    var ipcChannel = ChildProcessIpcChannel.CreateServer(listener, ipcToken, QueueImmediate, IoScheduler);
                    child.AttachIpc(ipcChannel);
                    ipcChannel.WaitUntilConnected(process);
                }
                catch
                {
                    try
                    {
                        listener.Stop();
                    }
                    catch
                    {
                    }
                    process.Dispose();
                    throw;
                }

                IoScheduler.BeginIo();
                try
                {
                    _ = CompleteChildProcessAsync(process, stdio, completion);
                }
                catch (Exception asyncStartEx)
                {
                    IoScheduler.EndIo(
                        completion,
                        asyncStartEx as Error ?? new Error(asyncStartEx.Message, asyncStartEx),
                        isError: true);
                }
            }
            catch
            {
                child.disconnect();
                throw;
            }

            return child;
        }

        public sealed class ChildProcessError : Error
        {
            public ChildProcessError(string command, int exitCode, string stdout, string stderr)
                : base($"Command failed: {command} (exit code {exitCode})")
            {
                status = (double)exitCode;
                code = (double)exitCode;
                cmd = command;
                this.stdout = stdout;
                this.stderr = stderr;
            }

            // Lowercase to match Node-ish shape
            public double status { get; }

            public double code { get; }

            public string cmd { get; }

            public object stdout { get; }

            public object stderr { get; }
        }

        private object StartChildProcess(object command, object? args, object? options, bool? shellOverride, Delegate? callback, bool suppressUnhandledError)
        {
            var commandText = command?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(commandText))
            {
                throw new TypeError("The \"file\" argument must be a non-empty string");
            }

            var argList = CoerceArgs(args);
            var cwd = TryGetStringOption(options, "cwd");
            var shell = shellOverride ?? TryGetBoolOption(options, "shell");
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.AsyncDefault, allowIpc: false, apiName: "child_process.spawn/exec");
            var envOverrides = ParseEnvironmentOverrides(TryGetOption(options, "env"));
            var child = new ChildProcessHandle(stdio);
            var completion = CreateCompletionPromise(child, commandText, callback, suppressUnhandledError);

            try
            {
                var psi = BuildStartInfo(commandText, argList, cwd, shell, stdio, envOverrides);
                var process = DiagnosticsProcess.Start(psi) ?? throw new Error("Failed to start process.");
                try
                {
                    child.Attach(process);
                }
                catch
                {
                    process.Dispose();
                    throw;
                }

                IoScheduler.BeginIo();
                try
                {
                    _ = CompleteChildProcessAsync(process, stdio, completion);
                }
                catch (Exception asyncStartEx)
                {
                    IoScheduler.EndIo(
                        completion,
                        asyncStartEx as Error ?? new Error(asyncStartEx.Message, asyncStartEx),
                        isError: true);
                }
            }
            catch (Exception ex)
            {
                var reason = ex as Error ?? new Error(ex.Message, ex);
                QueueImmediate(() =>
                {
                    child.CompleteFailure(reason, suppressUnhandledError);
                    if (callback != null)
                    {
                        InvokeExecCallback(callback, reason, string.Empty, string.Empty);
                    }
                });
            }

            return child;
        }

        private async Task CompleteChildProcessAsync(DiagnosticsProcess process, StdioConfiguration stdio, PromiseWithResolvers promiseWithResolvers)
        {
            try
            {
                Task<string>? stdoutTask = StartTextReadTask(process.StandardOutput, stdio.StdoutMode);
                Task<string>? stderrTask = StartTextReadTask(process.StandardError, stdio.StderrMode);
                var completionTasks = new List<Task>(capacity: 3)
                {
                    process.WaitForExitAsync(),
                };

                if (stdoutTask != null)
                {
                    completionTasks.Add(stdoutTask);
                }

                if (stderrTask != null)
                {
                    completionTasks.Add(stderrTask);
                }

                await Task.WhenAll(completionTasks).ConfigureAwait(false);

                var stdout = stdoutTask != null ? stdoutTask.Result : string.Empty;
                var stderr = stderrTask != null ? stderrTask.Result : string.Empty;

                IoScheduler.EndIo(
                    promiseWithResolvers,
                    new ProcessCompletionResult(process.ExitCode, stdout, stderr),
                    isError: false);
            }
            catch (Exception ex)
            {
                IoScheduler.EndIo(
                    promiseWithResolvers,
                    ex as Error ?? new Error(ex.Message, ex),
                    isError: true);
            }
            finally
            {
                try
                {
                    process.Dispose();
                }
                catch
                {
                    // Ignore cleanup failures.
                }
            }
        }

        private PromiseWithResolvers CreateCompletionPromise(ChildProcessHandle child, string commandText, Delegate? callback, bool suppressUnhandledError)
        {
            JsFunc1 resolve = (scopes, newTarget, value) =>
            {
                if (value is not ProcessCompletionResult completion)
                {
                    throw new InvalidOperationException("child_process completion did not supply a ProcessCompletionResult.");
                }

                child.CompleteSuccess(completion);

                if (callback != null)
                {
                    var error = completion.ExitCode == 0
                        ? (object)JsNull.Null
                        : new ChildProcessError(commandText, completion.ExitCode, completion.Stdout, completion.Stderr);

                    InvokeExecCallback(callback, error, completion.Stdout, completion.Stderr);
                }

                return null;
            };

            JsFunc1 reject = (scopes, newTarget, reason) =>
            {
                child.CompleteFailure(reason, suppressUnhandledError);

                if (callback != null)
                {
                    InvokeExecCallback(callback, reason, string.Empty, string.Empty);
                }

                return null;
            };

            return new PromiseWithResolvers(new Promise(), resolve, reject);
        }

        private static void InvokeExecCallback(Delegate callback, object? error, string stdout, string stderr)
        {
            var errArg = error ?? JsNull.Null;
            Closure.InvokeWithArgs(callback, RuntimeServices.EmptyScopes, errArg, stdout, stderr);
        }

        private static ProcessCompletionResult WaitForProcessCompletionSync(DiagnosticsProcess process, StdioConfiguration stdio)
        {
            Task<string>? stdoutTask = StartTextReadTask(process.StandardOutput, stdio.StdoutMode);
            Task<string>? stderrTask = StartTextReadTask(process.StandardError, stdio.StderrMode);

            if (stdoutTask != null && stderrTask != null)
            {
                Task.WaitAll(stdoutTask, stderrTask);
            }
            else if (stdoutTask != null)
            {
                stdoutTask.Wait();
            }
            else if (stderrTask != null)
            {
                stderrTask.Wait();
            }
            else
            {
                process.WaitForExit();
            }

            process.WaitForExit();

            return new ProcessCompletionResult(
                process.ExitCode,
                stdoutTask?.Result ?? string.Empty,
                stderrTask?.Result ?? string.Empty);
        }

        private static Task<string>? StartTextReadTask(StreamReader reader, StdioMode mode)
        {
            return mode switch
            {
                StdioMode.Pipe => reader.ReadToEndAsync(),
                StdioMode.Ignore => DrainTextReaderAsync(reader),
                _ => null,
            };
        }

        private static async Task<string> DrainTextReaderAsync(TextReader reader)
        {
            var buffer = new char[1024];
            while (await reader.ReadAsync(buffer, 0, buffer.Length).ConfigureAwait(false) > 0)
            {
            }

            return string.Empty;
        }

        private void QueueImmediate(Action action)
        {
            NodeNetworkingCommon.ScheduleImmediateOnEventLoop(NodeScheduler, action);
        }

        private static bool IsHostedRuntime()
        {
            return GlobalThis.ServiceProvider != null
                && GlobalThis.ServiceProvider.TryResolve<RuntimeExecutionContext>(out var executionContext)
                && executionContext != null
                && executionContext.IsHosted;
        }

        private static string CreateIpcAuthenticationToken()
        {
            Span<byte> tokenBytes = stackalloc byte[32];
            RandomNumberGenerator.Fill(tokenBytes);
            return Convert.ToHexString(tokenBytes);
        }

        private static DiagnosticsProcessStartInfo BuildStartInfo(
            string command,
            IReadOnlyList<string> args,
            string? cwd,
            bool shell,
            StdioConfiguration stdio,
            IReadOnlyDictionary<string, string?>? envOverrides = null)
        {
            var psi = new DiagnosticsProcessStartInfo
            {
                UseShellExecute = false,
                RedirectStandardInput = stdio.StdinMode != StdioMode.Inherit,
                RedirectStandardOutput = stdio.StdoutMode != StdioMode.Inherit,
                RedirectStandardError = stdio.StderrMode != StdioMode.Inherit,
                CreateNoWindow = true,
            };

            if (!string.IsNullOrWhiteSpace(cwd))
            {
                psi.WorkingDirectory = cwd;
            }

            if (envOverrides != null)
            {
                foreach (var kvp in envOverrides)
                {
                    if (string.IsNullOrWhiteSpace(kvp.Key))
                    {
                        continue;
                    }

                    if (kvp.Value == null)
                    {
                        _ = psi.Environment.Remove(kvp.Key);
                    }
                    else
                    {
                        psi.Environment[kvp.Key] = kvp.Value;
                    }
                }
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

            if (args == null || args is JsNull)
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
            if (options == null || options is JsNull) return null;

            try
            {
                if (options is ExpandoObject exp)
                {
                    var dict = (IDictionary<string, object?>)exp;
                    if (dict.TryGetValue(name, out var val)) return val;
                }

                return ObjectRuntime.GetProperty(options, name);
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
            if (v == null || v is JsNull) return false;
            try
            {
                return TypeUtilities.ToBoolean(v);
            }
            catch
            {
                return false;
            }
        }

        private static Delegate? ValidateCallback(object? callback)
        {
            if (callback == null || callback is JsNull)
            {
                return null;
            }

            if (callback is Delegate del)
            {
                return del;
            }

            throw new TypeError("The \"callback\" argument must be of type function");
        }

        private static bool IsArgumentList(object? value)
            => value is JavaScriptRuntime.Array || value is object[];

        private static Dictionary<string, string?> ParseEnvironmentOverrides(object? env)
        {
            var result = new Dictionary<string, string?>(StringComparer.Ordinal);
            if (env == null || env is JsNull)
            {
                return result;
            }

            if (env is IDictionary<string, object?> dictionary)
            {
                foreach (var kvp in dictionary)
                {
                    result[kvp.Key] = NormalizeEnvironmentValue(kvp.Value);
                }

                return result;
            }

            if (JavaScriptRuntime.Object.GetEnumerableKeys(env) is JavaScriptRuntime.Array keys)
            {
                for (int i = 0; i < (int)keys.length; i++)
                {
                    var key = keys[i]?.ToString();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    result[key] = NormalizeEnvironmentValue(ObjectRuntime.GetProperty(env, key));
                }
            }

            return result;
        }

        private static string? NormalizeEnvironmentValue(object? value)
        {
            if (value == null || value is JsNull)
            {
                return null;
            }

            return value.ToString();
        }

        private static StdioConfiguration ParseStdioConfiguration(object? stdio, StdioConfiguration defaults, bool allowIpc, string apiName)
        {
            if (stdio == null || stdio is JsNull)
            {
                return defaults;
            }

            if (stdio is string text)
            {
                if (text.Equals("inherit", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioConfiguration.InheritAll;
                }

                if (text.Equals("pipe", StringComparison.OrdinalIgnoreCase))
                {
                    return defaults;
                }

                if (allowIpc && text.Equals("ipc", StringComparison.OrdinalIgnoreCase))
                {
                    return defaults.WithIpc(true);
                }

                if (text.Equals("ignore", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioConfiguration.IgnoreAll;
                }
            }

            if (TryCoerceArray(stdio, out var slots))
            {
                if (!allowIpc)
                {
                    EnsureOnlySupportedStdioSlots(slots, apiName, firstUnsupportedIndex: 3);
                }

                return new StdioConfiguration(
                    ParseSlotMode(slots, 0, defaults.StdinMode, apiName),
                    ParseSlotMode(slots, 1, defaults.StdoutMode, apiName),
                    ParseSlotMode(slots, 2, defaults.StderrMode, apiName),
                    allowIpc && ParseIpcSlot(slots, defaults.IpcEnabled, apiName));
            }

            return defaults;
        }

        private static bool TryCoerceArray(object? value, out object?[] slots)
        {
            if (value is object[] array)
            {
                slots = array;
                return true;
            }

            if (value is JavaScriptRuntime.Array jsArray)
            {
                slots = new object?[checked((int)jsArray.length)];
                for (int i = 0; i < slots.Length; i++)
                {
                    slots[i] = jsArray[i];
                }

                return true;
            }

            slots = System.Array.Empty<object?>();
            return false;
        }

        private static StdioMode ParseSlotMode(object?[] slots, int index, StdioMode defaultValue, string apiName)
        {
            if (index >= slots.Length)
            {
                return defaultValue;
            }

            var slot = slots[index];
            if (slot == null || slot is JsNull)
            {
                return defaultValue;
            }

            if (slot is string text)
            {
                if (text.Equals("pipe", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioMode.Pipe;
                }

                if (text.Equals("inherit", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioMode.Inherit;
                }

                if (text.Equals("ignore", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioMode.Ignore;
                }
            }

            throw new Error($"{apiName} only supports stdio values 'pipe', 'inherit', and 'ignore' for slots 0-2 in the current runtime.");
        }

        private static bool ParseIpcSlot(object?[] slots, bool defaultValue, string apiName)
        {
            if (slots.Length <= 3)
            {
                return defaultValue;
            }

            var slot = slots[3];
            if (slot == null || slot is JsNull)
            {
                return defaultValue;
            }

            if (slot is string text && text.Equals("ipc", StringComparison.OrdinalIgnoreCase))
            {
                EnsureOnlySupportedStdioSlots(slots, apiName, firstUnsupportedIndex: 4);
                return true;
            }

            throw new Error($"{apiName} only supports 'ipc' as stdio[3] in the current runtime.");
        }

        private static void EnsureOnlySupportedStdioSlots(object?[] slots, string apiName, int firstUnsupportedIndex)
        {
            for (int i = firstUnsupportedIndex; i < slots.Length; i++)
            {
                var slot = slots[i];
                if (slot == null || slot is JsNull)
                {
                    continue;
                }

                throw new Error(firstUnsupportedIndex <= 3
                    ? $"{apiName} only supports stdio slots 0-2 in the current runtime."
                    : $"{apiName} only supports stdio slots 0-2 plus an optional IPC slot at index 3 in the current runtime.");
            }
        }

        internal enum StdioMode
        {
            Inherit,
            Pipe,
            Ignore,
        }

        internal sealed class ProcessCompletionResult
        {
            public ProcessCompletionResult(int exitCode, string stdout, string stderr)
            {
                ExitCode = exitCode;
                Stdout = stdout;
                Stderr = stderr;
            }

            public int ExitCode { get; }

            public string Stdout { get; }

            public string Stderr { get; }
        }

        internal sealed class StdioConfiguration
        {
            public static readonly StdioConfiguration SyncDefault = new(StdioMode.Inherit, StdioMode.Pipe, StdioMode.Pipe, ipcEnabled: false);
            public static readonly StdioConfiguration AsyncDefault = new(StdioMode.Pipe, StdioMode.Pipe, StdioMode.Pipe, ipcEnabled: false);
            public static readonly StdioConfiguration ForkDefault = new(StdioMode.Pipe, StdioMode.Pipe, StdioMode.Pipe, ipcEnabled: true);
            public static readonly StdioConfiguration InheritAll = new(StdioMode.Inherit, StdioMode.Inherit, StdioMode.Inherit, ipcEnabled: false);
            public static readonly StdioConfiguration IgnoreAll = new(StdioMode.Ignore, StdioMode.Ignore, StdioMode.Ignore, ipcEnabled: false);

            public StdioConfiguration(StdioMode stdinMode, StdioMode stdoutMode, StdioMode stderrMode, bool ipcEnabled)
            {
                StdinMode = stdinMode;
                StdoutMode = stdoutMode;
                StderrMode = stderrMode;
                IpcEnabled = ipcEnabled;
            }

            public StdioMode StdinMode { get; }

            public StdioMode StdoutMode { get; }

            public StdioMode StderrMode { get; }

            public bool IpcEnabled { get; }

            public StdioConfiguration WithIpc(bool enabled)
                => new(StdinMode, StdoutMode, StderrMode, enabled);
        }

        public sealed class ChildProcessHandle : EventEmitter
        {
            private readonly Readable? _stdoutReadable;
            private readonly Readable? _stderrReadable;
            private readonly ChildProcessWritable? _stdinWritable;
            private readonly Action<Action>? _queueImmediate;
            private DiagnosticsProcess? _process;
            private ChildProcessIpcChannel? _ipcChannel;
            private bool _ipcHandlersAttached;
            private bool _completed;
            private string? _terminationSignal;

            internal ChildProcessHandle(StdioConfiguration stdio, Action<Action>? queueImmediate = null)
            {
                _queueImmediate = queueImmediate;
                _stdinWritable = stdio.StdinMode == StdioMode.Pipe ? new ChildProcessWritable() : null;
                _stdoutReadable = stdio.StdoutMode == StdioMode.Pipe ? new Readable() : null;
                _stderrReadable = stdio.StderrMode == StdioMode.Pipe ? new Readable() : null;

                stdin = _stdinWritable != null ? _stdinWritable : JsNull.Null;
                stdout = _stdoutReadable != null ? _stdoutReadable : JsNull.Null;
                stderr = _stderrReadable != null ? _stderrReadable : JsNull.Null;
                pid = JsNull.Null;
                exitCode = JsNull.Null;
                connected = false;
                killed = false;
            }

            public object stdin { get; }

            public object stdout { get; }

            public object stderr { get; }

            public object pid { get; private set; }

            public object exitCode { get; private set; }

            public bool connected { get; private set; }

            public bool killed { get; private set; }

            internal void Attach(DiagnosticsProcess process)
            {
                _process = process;
                pid = (double)process.Id;
                _stdinWritable?.Attach(process);

                if (_stdinWritable == null && process.StartInfo.RedirectStandardInput)
                {
                    try
                    {
                        process.StandardInput.Close();
                    }
                    catch
                    {
                        // Ignore best-effort stdin shutdown for ignored stdio.
                    }
                }
            }

            internal void AttachIpc(ChildProcessIpcChannel channel)
            {
                _ipcChannel = channel;
                channel.MessageReceived += HandleIpcMessage;
                channel.Disconnected += HandleIpcDisconnect;
                channel.Error += HandleIpcError;
                _ipcHandlersAttached = true;
                connected = true;
                channel.Start();
            }

            private void HandleIpcMessage(object? payload)
            {
                emit("message", payload);
            }

            private void HandleIpcDisconnect()
            {
                if (!connected)
                {
                    DetachIpcHandlers();
                    return;
                }

                connected = false;
                DetachIpcHandlers();
                DisposeIpcChannel();
                emit("disconnect");
            }

            private void HandleIpcError(Exception ex)
            {
                emit("error", ex as Error ?? new Error(ex.Message, ex));
            }

            private void DetachIpcHandlers()
            {
                if (!_ipcHandlersAttached || _ipcChannel == null)
                {
                    return;
                }

                _ipcChannel.MessageReceived -= HandleIpcMessage;
                _ipcChannel.Disconnected -= HandleIpcDisconnect;
                _ipcChannel.Error -= HandleIpcError;
                _ipcHandlersAttached = false;
            }

            private void DisposeIpcChannel()
            {
                try
                {
                    _ipcChannel?.Dispose();
                }
                catch
                {
                }
            }

            private void EnsureIpcDisconnectedBeforeCompletion()
            {
                if (_ipcChannel == null)
                {
                    return;
                }

                var wasConnected = connected;
                connected = false;
                DetachIpcHandlers();
                DisposeIpcChannel();

                if (wasConnected)
                {
                    emit("disconnect");
                }
            }

            internal void CompleteSuccess(ProcessCompletionResult completion)
            {
                if (_completed)
                {
                    return;
                }

                _completed = true;
                exitCode = _terminationSignal != null ? JsNull.Null : (double)completion.ExitCode;

                if (_stdoutReadable != null)
                {
                    if (completion.Stdout.Length > 0)
                    {
                        _stdoutReadable.push(completion.Stdout);
                    }

                    _stdoutReadable.push(null);
                }

                if (_stderrReadable != null)
                {
                    if (completion.Stderr.Length > 0)
                    {
                        _stderrReadable.push(completion.Stderr);
                    }

                    _stderrReadable.push(null);
                }

                _stdinWritable?.CloseQuietly();
                EnsureIpcDisconnectedBeforeCompletion();

                var exitArg = _terminationSignal != null ? JsNull.Null : (object)(double)completion.ExitCode;
                var signalArg = _terminationSignal ?? (object)JsNull.Null;
                emit("exit", exitArg, signalArg);
                emit("close", exitArg, signalArg);
            }

            internal void CompleteFailure(object? reason, bool suppressUnhandledError)
            {
                if (_completed)
                {
                    return;
                }

                _completed = true;
                exitCode = JsNull.Null;

                _stdoutReadable?.push(null);
                _stderrReadable?.push(null);
                _stdinWritable?.CloseQuietly();
                EnsureIpcDisconnectedBeforeCompletion();

                ExceptionDispatchInfo? captured = null;
                var shouldEmitError = !suppressUnhandledError || listenerCount("error") > 0;

                if (shouldEmitError)
                {
                    try
                    {
                        emit("error", reason);
                    }
                    catch (Exception ex)
                    {
                        captured = ExceptionDispatchInfo.Capture(ex);
                    }
                }

                emit("close", JsNull.Null, JsNull.Null);
                captured?.Throw();
            }

            public bool kill()
                => kill(null);

            public bool kill(object? signal)
            {
                var normalizedSignal = NormalizeSignal(signal);

                try
                {
                    if (_process == null || _process.HasExited)
                    {
                        return false;
                    }

                    _terminationSignal = normalizedSignal;
                    killed = true;
                    _process.Kill();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            public bool send(object? message)
            {
                if (_ipcChannel == null)
                {
                    throw new Error("child.send() is only available for processes started with an IPC channel.");
                }

                return _ipcChannel.Send(message);
            }

            public void disconnect()
            {
                _ipcChannel?.Disconnect();
            }

            private static string NormalizeSignal(object? signal)
            {
                if (signal == null || signal is JsNull)
                {
                    return "SIGTERM";
                }

                var text = signal.ToString();
                if (string.IsNullOrWhiteSpace(text))
                {
                    return "SIGTERM";
                }

                if (text.Equals("SIGTERM", StringComparison.OrdinalIgnoreCase))
                {
                    return "SIGTERM";
                }

                if (text.Equals("SIGKILL", StringComparison.OrdinalIgnoreCase))
                {
                    return "SIGKILL";
                }

                if (text.Equals("SIGINT", StringComparison.OrdinalIgnoreCase))
                {
                    return "SIGINT";
                }

                throw new Error($"child.kill('{text}') is not supported in the current runtime. Supported signals are SIGTERM, SIGKILL, and SIGINT.");
            }
        }

        private sealed class ChildProcessWritable : Writable
        {
            private DiagnosticsProcess? _process;

            internal void Attach(DiagnosticsProcess process)
            {
                _process = process;
            }

            internal void CloseQuietly()
            {
                try
                {
                    if (_process != null && !_process.HasExited && _process.StartInfo.RedirectStandardInput)
                    {
                        _process.StandardInput.Close();
                    }
                }
                catch
                {
                    // Ignore cleanup failures.
                }
            }

            public override void end()
            {
                CloseQuietly();
                base.end();
            }

            protected override void InvokeWrite(object? chunk)
            {
                if (_process == null || _process.HasExited || !_process.StartInfo.RedirectStandardInput)
                {
                    throw new Error("child.stdin is not available.");
                }

                var text = chunk switch
                {
                    Buffer buffer => Encoding.UTF8.GetString(buffer.ToByteArray()),
                    byte[] bytes => Encoding.UTF8.GetString(bytes),
                    _ => chunk?.ToString() ?? string.Empty,
                };

                try
                {
                    _process.StandardInput.Write(text);
                    _process.StandardInput.Flush();
                }
                catch (Exception ex) when (ex is InvalidOperationException or ObjectDisposedException or IOException)
                {
                    throw new Error("child.stdin is not available.", ex);
                }
            }
        }
    }

    internal static class ChildProcessRuntimeOptions
    {
        internal const string ForkEntryModuleEnvVar = "JS2IL_CHILD_PROCESS_ENTRY_MODULE";
        internal const string ForkIpcPortEnvVar = "JS2IL_CHILD_PROCESS_IPC_PORT";
        internal const string ForkIpcTokenEnvVar = "JS2IL_CHILD_PROCESS_IPC_TOKEN";
    }

    internal sealed class ChildProcessIpcChannel : IDisposable
    {
        private const string HandshakePrefix = "__js2il_child_process_ipc__:";
        private readonly Action<Action> _queueImmediate;
        private readonly IIOScheduler? _ioScheduler;
        private readonly TaskCompletionSource _connected = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private readonly CancellationTokenSource _disposeCancellation = new();
        private readonly object _sync = new();
        private readonly TcpListener? _listener;
        private readonly int? _clientPort;
        private readonly string? _expectedToken;
        private readonly string? _clientToken;
        private global::System.IO.Stream? _stream;
        private TcpClient? _tcpClient;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private int _disconnectSignaled;
        private int _disposeSignaled;
        private int _started;
        private bool _disposed;

        private ChildProcessIpcChannel(TcpListener listener, string expectedToken, Action<Action> queueImmediate, IIOScheduler? ioScheduler)
        {
            _listener = listener;
            _expectedToken = expectedToken;
            _queueImmediate = queueImmediate;
            _ioScheduler = ioScheduler;
        }

        private ChildProcessIpcChannel(int clientPort, string clientToken, Action<Action> queueImmediate, IIOScheduler? ioScheduler)
        {
            _clientPort = clientPort;
            _clientToken = clientToken;
            _queueImmediate = queueImmediate;
            _ioScheduler = ioScheduler;
        }

        internal event Action<object?>? MessageReceived;
        internal event Action? Disconnected;
        internal event Action<Exception>? Error;

        internal bool Connected => _connected.Task.IsCompletedSuccessfully && Volatile.Read(ref _disposeSignaled) == 0;

        internal static ChildProcessIpcChannel CreateServer(TcpListener listener, string expectedToken, Action<Action> queueImmediate, IIOScheduler? ioScheduler)
            => new(listener, expectedToken, queueImmediate, ioScheduler);

        internal static ChildProcessIpcChannel CreateClient(int port, string clientToken, Action<Action> queueImmediate, IIOScheduler? ioScheduler)
            => new(port, clientToken, queueImmediate, ioScheduler);

        internal void Start()
        {
            if (Interlocked.Exchange(ref _started, 1) != 0)
            {
                return;
            }

            if (_listener != null)
            {
                _ = AcceptServerConnectionAsync(_listener);
                return;
            }

            if (_clientPort.HasValue)
            {
                ConnectClient(_clientPort.Value, _clientToken
                    ?? throw new InvalidOperationException("The child_process IPC client token was not initialized."));
            }
        }

        internal bool Send(object? message)
        {
            EnsureConnected();

            var payload = ChildProcessIpcSerializer.Serialize(message);
            lock (_sync)
            {
                if (_disposed || _writer == null)
                {
                    return false;
                }

                try
                {
                    _writer.WriteLine(payload);
                    _writer.Flush();
                    return true;
                }
                catch (Exception ex) when (ex is IOException or InvalidOperationException or ObjectDisposedException)
                {
                    SignalError(ex);
                    SignalDisconnected();
                    Dispose();
                    return false;
                }
            }
        }

        internal void Disconnect()
        {
            SignalDisconnected();
            Dispose();
        }

        internal void WaitUntilConnected(DiagnosticsProcess process)
        {
            var firstCompleted = Task.WhenAny(_connected.Task, process.WaitForExitAsync()).GetAwaiter().GetResult();
            if (firstCompleted == _connected.Task)
            {
                _connected.Task.GetAwaiter().GetResult();
                return;
            }

            if (_connected.Task.IsCompleted)
            {
                _connected.Task.GetAwaiter().GetResult();
                return;
            }

            var exitCode = process.HasExited ? process.ExitCode : (int?)null;
            if (exitCode.HasValue)
            {
                throw new Error($"child_process.fork child process exited before establishing the IPC channel (exit code {exitCode.Value}).");
            }

            throw new Error("child_process.fork child process exited before establishing the IPC channel.");
        }

        private async Task AcceptServerConnectionAsync(TcpListener listener)
        {
            while (!_disposed)
            {
                TcpClient? candidateClient = null;
                try
                {
                    candidateClient = await listener.AcceptTcpClientAsync(_disposeCancellation.Token).ConfigureAwait(false);
                    _ = AttemptAuthenticateServerClientAsync(candidateClient);
                }
                catch (OperationCanceledException) when (_disposed || _disposeCancellation.IsCancellationRequested)
                {
                    return;
                }
                catch (ObjectDisposedException) when (_disposed || _connected.Task.IsCompleted)
                {
                    return;
                }
                catch (SocketException) when (_disposed || _connected.Task.IsCompleted)
                {
                    return;
                }
                catch (Exception ex)
                {
                    if (!_disposed)
                    {
                        _connected.TrySetException(ex);
                        SignalError(ex);
                        SignalDisconnected();
                    }

                    return;
                }
            }
        }

        private async Task AttemptAuthenticateServerClientAsync(TcpClient? candidateClient)
        {
            global::System.IO.Stream? candidateStream = null;
            StreamReader? candidateReader = null;
            StreamWriter? candidateWriter = null;
            try
            {
                if (candidateClient == null)
                {
                    return;
                }

                candidateStream = candidateClient.GetStream();
                PrepareTextEndpoints(candidateStream, out candidateReader, out candidateWriter);

                var handshake = await candidateReader.ReadLineAsync().WaitAsync(_disposeCancellation.Token).ConfigureAwait(false);
                if (!IsExpectedHandshake(handshake))
                {
                    return;
                }

                if (!TryInitializeConnectedEndpoint(candidateClient, candidateStream, candidateReader, candidateWriter))
                {
                    return;
                }

                candidateClient = null;
                candidateStream = null;
                candidateReader = null;
                candidateWriter = null;
                TryStopListener();
                _ = ReadLoopAsync();
            }
            catch (OperationCanceledException) when (_disposed || _disposeCancellation.IsCancellationRequested)
            {
            }
            catch
            {
            }
            finally
            {
                try
                {
                    candidateWriter?.Dispose();
                }
                catch
                {
                }

                try
                {
                    candidateReader?.Dispose();
                }
                catch
                {
                }

                try
                {
                    candidateStream?.Dispose();
                }
                catch
                {
                }

                try
                {
                    candidateClient?.Dispose();
                }
                catch
                {
                }
            }
        }

        private void ConnectClient(int port, string clientToken)
        {
            TcpClient? tcpClient = null;
            global::System.IO.Stream? stream = null;
            StreamReader? reader = null;
            StreamWriter? writer = null;
            try
            {
                tcpClient = new TcpClient();
                tcpClient.Connect(IPAddress.Loopback, port);
                stream = tcpClient.GetStream();
                PrepareTextEndpoints(stream, out reader, out writer);
                writer.WriteLine(BuildHandshakePayload(clientToken));
                writer.Flush();

                if (!TryInitializeConnectedEndpoint(tcpClient, stream, reader, writer))
                {
                    return;
                }

                tcpClient = null;
                stream = null;
                reader = null;
                writer = null;
                _ = ReadLoopAsync();
            }
            catch
            {
                throw;
            }
            finally
            {
                try
                {
                    writer?.Dispose();
                }
                catch
                {
                }

                try
                {
                    reader?.Dispose();
                }
                catch
                {
                }

                try
                {
                    stream?.Dispose();
                }
                catch
                {
                }

                try
                {
                    tcpClient?.Dispose();
                }
                catch
                {
                }
            }
        }

        private bool TryInitializeConnectedEndpoint(TcpClient tcpClient, global::System.IO.Stream stream, StreamReader reader, StreamWriter writer)
        {
            lock (_sync)
            {
                if (_disposed || _connected.Task.IsCompleted)
                {
                    return false;
                }

                _tcpClient = tcpClient;
                _stream = stream;
                _reader = reader;
                _writer = writer;
                _connected.TrySetResult();
                return true;
            }
        }

        private static void PrepareTextEndpoints(global::System.IO.Stream stream, out StreamReader reader, out StreamWriter writer)
        {
            reader = new StreamReader(stream, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            writer = new StreamWriter(stream, new UTF8Encoding(encoderShouldEmitUTF8Identifier: false), leaveOpen: true)
            {
                AutoFlush = true,
                NewLine = "\n"
            };
        }

        private bool IsExpectedHandshake(string? payload)
        {
            if (string.IsNullOrWhiteSpace(payload) || string.IsNullOrWhiteSpace(_expectedToken))
            {
                return false;
            }

            if (!payload.StartsWith(HandshakePrefix, StringComparison.Ordinal))
            {
                return false;
            }

            return string.Equals(payload.Substring(HandshakePrefix.Length), _expectedToken, StringComparison.Ordinal);
        }

        private static string BuildHandshakePayload(string token)
            => HandshakePrefix + token;

        private void TryStopListener()
        {
            try
            {
                _listener?.Stop();
            }
            catch
            {
            }
        }

        private async Task ReadLoopAsync()
        {
            try
            {
                EnsureConnected();
                while (!_disposed && _reader != null)
                {
                    var line = await _reader.ReadLineAsync().WaitAsync(_disposeCancellation.Token).ConfigureAwait(false);
                    if (line == null)
                    {
                        break;
                    }

                    var payload = ChildProcessIpcSerializer.Deserialize(line);
                    DispatchToEventLoop(payload);
                }
            }
            catch (OperationCanceledException) when (_disposed || _disposeCancellation.IsCancellationRequested)
            {
            }
            catch (Exception ex)
            {
                if (!_disposed)
                {
                    SignalError(ex);
                }
            }
            finally
            {
                SignalDisconnected();
                Dispose();
            }
        }

        private void EnsureConnected()
        {
            _connected.Task.GetAwaiter().GetResult();
        }

        private void DispatchToEventLoop(object? payload)
        {
            if (_ioScheduler != null)
            {
                var dispatch = CreateDispatchPromise();
                _ioScheduler.BeginIo();
                _ioScheduler.EndIo(dispatch, payload, isError: false);
                return;
            }

            _queueImmediate(() => MessageReceived?.Invoke(payload));
        }

        private PromiseWithResolvers CreateDispatchPromise()
        {
            JsFunc1 resolve = (scopes, newTarget, value) =>
            {
                MessageReceived?.Invoke(value);
                return null;
            };

            JsFunc1 reject = (scopes, newTarget, reason) =>
            {
                if (reason is Exception ex)
                {
                    Error?.Invoke(ex);
                }
                else if (reason != null)
                {
                    Error?.Invoke(new Error(reason.ToString() ?? "child_process IPC dispatch failed."));
                }

                return null;
            };

            return new PromiseWithResolvers(new Promise(), resolve, reject);
        }

        private void SignalDisconnected()
        {
            if (Interlocked.Exchange(ref _disconnectSignaled, 1) != 0)
            {
                return;
            }

            _queueImmediate(() => Disconnected?.Invoke());
        }

        private void SignalError(Exception ex)
        {
            _queueImmediate(() => Error?.Invoke(ex));
        }

        public void Dispose()
        {
            if (Interlocked.Exchange(ref _disposeSignaled, 1) != 0)
            {
                return;
            }

            if (!_connected.Task.IsCompleted)
            {
                _connected.TrySetException(new ObjectDisposedException(nameof(ChildProcessIpcChannel)));
            }

            try
            {
                _disposeCancellation.Cancel();
            }
            catch (ObjectDisposedException)
            {
            }

            lock (_sync)
            {
                _disposed = true;
                try
                {
                    _writer?.Dispose();
                }
                catch
                {
                }

                try
                {
                    _reader?.Dispose();
                }
                catch
                {
                }

                try
                {
                    _stream?.Dispose();
                }
                catch
                {
                }

                try
                {
                    _tcpClient?.Dispose();
                }
                catch
                {
                }

                try
                {
                    _listener?.Stop();
                }
                catch
                {
                }
            }

            try
            {
                _disposeCancellation.Dispose();
            }
            catch
            {
            }

            GC.SuppressFinalize(this);
        }
    }

    internal static class ChildProcessIpcSerializer
    {
        internal static string Serialize(object? value)
            => JsonSerializer.Serialize(ToSerializableValue(value));

        internal static object? Deserialize(string payload)
            => JavaScriptRuntime.JSON.Parse(payload);

        private static object? ToSerializableValue(object? value)
        {
            if (value == null || value is JsNull)
            {
                return null;
            }

            switch (value)
            {
                case string or bool or byte or sbyte or short or ushort or int or uint or long or ulong or float or double or decimal:
                    return value;
                case JavaScriptRuntime.Array array:
                    var items = new List<object?>(checked((int)array.length));
                    for (int i = 0; i < (int)array.length; i++)
                    {
                        items.Add(ToSerializableValue(array[i]));
                    }
                    return items;
                case JsObject jsObject:
                    var jsObjectResult = new Dictionary<string, object?>(StringComparer.Ordinal);
                    foreach (var key in jsObject.GetOwnPropertyNames())
                    {
                        jsObjectResult[key] = ToSerializableValue(jsObject[key]);
                    }
                    return jsObjectResult;
                case IDictionary<string, object?> dictionary:
                    var dictResult = new Dictionary<string, object?>(StringComparer.Ordinal);
                    foreach (var kvp in dictionary)
                    {
                        dictResult[kvp.Key] = ToSerializableValue(kvp.Value);
                    }
                    return dictResult;
                case Buffer or byte[]:
                    throw new Error("child_process IPC only supports JSON-serializable values in the current runtime. Buffer (byte[]) and other binary payloads are not yet supported.");
            }

            if (JavaScriptRuntime.Object.GetEnumerableKeys(value) is JavaScriptRuntime.Array keys && keys.length > 0)
            {
                var objectResult = new Dictionary<string, object?>(StringComparer.Ordinal);
                for (int i = 0; i < (int)keys.length; i++)
                {
                    var key = keys[i]?.ToString();
                    if (string.IsNullOrWhiteSpace(key))
                    {
                        continue;
                    }

                    objectResult[key] = ToSerializableValue(ObjectRuntime.GetProperty(value, key));
                }

                return objectResult;
            }

            throw new Error($"child_process IPC only supports JSON-serializable values in the current runtime (received '{value.GetType().Name}').");
        }
    }
}
