using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Text;
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
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.SyncDefault);

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
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.SyncDefault);

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
            var stdio = ParseStdioConfiguration(TryGetOption(options, "stdio"), StdioConfiguration.AsyncDefault);
            var child = new ChildProcessHandle(stdio);
            var completion = CreateCompletionPromise(child, commandText, callback, suppressUnhandledError);

            try
            {
                var psi = BuildStartInfo(commandText, argList, cwd, shell, stdio);
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
            ((IScheduler)NodeScheduler).ScheduleImmediate(action);
        }

        private static DiagnosticsProcessStartInfo BuildStartInfo(
            string command,
            IReadOnlyList<string> args,
            string? cwd,
            bool shell,
            StdioConfiguration stdio)
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

        private static StdioConfiguration ParseStdioConfiguration(object? stdio, StdioConfiguration defaults)
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

                if (text.Equals("ignore", StringComparison.OrdinalIgnoreCase))
                {
                    return StdioConfiguration.IgnoreAll;
                }
            }

            if (TryCoerceArray(stdio, out var slots))
            {
                return new StdioConfiguration(
                    ParseSlotMode(slots, 0, defaults.StdinMode),
                    ParseSlotMode(slots, 1, defaults.StdoutMode),
                    ParseSlotMode(slots, 2, defaults.StderrMode));
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

        private static StdioMode ParseSlotMode(object?[] slots, int index, StdioMode defaultValue)
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

            return defaultValue;
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
            public static readonly StdioConfiguration SyncDefault = new(StdioMode.Inherit, StdioMode.Pipe, StdioMode.Pipe);
            public static readonly StdioConfiguration AsyncDefault = new(StdioMode.Pipe, StdioMode.Pipe, StdioMode.Pipe);
            public static readonly StdioConfiguration InheritAll = new(StdioMode.Inherit, StdioMode.Inherit, StdioMode.Inherit);
            public static readonly StdioConfiguration IgnoreAll = new(StdioMode.Ignore, StdioMode.Ignore, StdioMode.Ignore);

            public StdioConfiguration(StdioMode stdinMode, StdioMode stdoutMode, StdioMode stderrMode)
            {
                StdinMode = stdinMode;
                StdoutMode = stdoutMode;
                StderrMode = stderrMode;
            }

            public StdioMode StdinMode { get; }

            public StdioMode StdoutMode { get; }

            public StdioMode StderrMode { get; }
        }

        public sealed class ChildProcessHandle : EventEmitter
        {
            private readonly Readable? _stdoutReadable;
            private readonly Readable? _stderrReadable;
            private readonly ChildProcessWritable? _stdinWritable;
            private DiagnosticsProcess? _process;
            private bool _completed;

            internal ChildProcessHandle(StdioConfiguration stdio)
            {
                _stdinWritable = stdio.StdinMode == StdioMode.Pipe ? new ChildProcessWritable() : null;
                _stdoutReadable = stdio.StdoutMode == StdioMode.Pipe ? new Readable() : null;
                _stderrReadable = stdio.StderrMode == StdioMode.Pipe ? new Readable() : null;

                stdin = _stdinWritable != null ? _stdinWritable : JsNull.Null;
                stdout = _stdoutReadable != null ? _stdoutReadable : JsNull.Null;
                stderr = _stderrReadable != null ? _stderrReadable : JsNull.Null;
                pid = JsNull.Null;
                exitCode = JsNull.Null;
            }

            public object stdin { get; }

            public object stdout { get; }

            public object stderr { get; }

            public object pid { get; private set; }

            public object exitCode { get; private set; }

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

            internal void CompleteSuccess(ProcessCompletionResult completion)
            {
                if (_completed)
                {
                    return;
                }

                _completed = true;
                exitCode = (double)completion.ExitCode;

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

                emit("exit", (double)completion.ExitCode, JsNull.Null);
                emit("close", (double)completion.ExitCode, JsNull.Null);
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
                _ = signal;

                try
                {
                    if (_process == null || _process.HasExited)
                    {
                        return false;
                    }

                    _process.Kill();
                    return true;
                }
                catch
                {
                    return false;
                }
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
}
