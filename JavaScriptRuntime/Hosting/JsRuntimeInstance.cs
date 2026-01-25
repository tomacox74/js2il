using System.Collections.Concurrent;
using System.Reflection;
using JavaScriptRuntime;
using JavaScriptRuntime.CommonJS;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace Js2IL.Runtime;

/// <summary>
/// Hosts a single JS2IL "runtime instance" on a dedicated thread.
/// All JS execution (module load + subsequent invocations) is serialized onto that thread.
/// </summary>
internal sealed class JsRuntimeInstance : IDisposable
{
    private static readonly TimeSpan DisposeJoinTimeout = TimeSpan.FromSeconds(10);

    internal string ModuleId { get; }
    internal string? CompiledAssemblyName { get; }

    // Signals when the script thread has fully exited (ThreadMain finally block).
    // Using a TaskCompletionSource avoids allocating/disposal of an underlying WaitHandle.
    private readonly TaskCompletionSource _terminated = new(TaskCreationOptions.RunContinuationsAsynchronously);

    // Cross-thread work queue used to marshal calls onto the dedicated script thread.
    private readonly BlockingCollection<IWorkItem> _queue = new();

    // Dedicated thread that owns the engine, synchronization context, and event loop.
    private readonly Thread _thread;

    // Cancellation used to stop consuming the queue and unblock waiting operations during disposal.
    private readonly CancellationTokenSource _shutdown = new();

    // Completed once initial module load has either succeeded or failed (exception is propagated).
    // This is awaited synchronously in the ctor to surface module-load errors immediately.
    private readonly TaskCompletionSource _initialized = new(TaskCreationOptions.RunContinuationsAsynchronously);

    // Service provider/sync context are thread-affine and created inside ThreadMain.
    private ServiceContainer? _serviceProvider;
    private NodeEventLoopPump? _eventLoop;

    // Exports returned by CommonJS module evaluation (require(...) result).
    private object? _exports;

    // 0 -> not disposed, 1 -> dispose requested (used to guard multiple Dispose calls).
    private int _disposeSignaled;

    public JsRuntimeInstance(Assembly compiledAssembly, string moduleId)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        ModuleId = moduleId.Trim();
        CompiledAssemblyName = compiledAssembly.GetName().Name;

        // Treat bare specifiers as local modules by default ("foo" -> "./foo").
        var normalized = NormalizeLocalModuleSpecifier(ModuleId);

        _thread = new Thread(() => ThreadMain(compiledAssembly, normalized))
        {
            // Background thread so the host process can exit naturally if it forgets to Dispose.
            // This does not affect scheduling/priority; it only affects process shutdown semantics.
            // We may make this configurable in the future.
            IsBackground = true,
            Name = $"Js2IL.ScriptThread[{CompiledAssemblyName}:{normalized}]"
        };

        _thread.Start();

        // Block until initialization finishes so callers see module-load errors immediately.
        // This makes construction "fail fast" if the module throws during evaluation.
        _initialized.Task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// The CommonJS exports of the loaded module.
    /// The reference is produced during initialization, but the exports object itself may be mutable.
    /// Treat it as thread-affine: if you need to interact with it directly, marshal onto the script thread.
    /// </summary>
    public object? Exports
    {
        get
        {
            EnsureNotDisposed();
            return _exports;
        }
    }

    /// <summary>
    /// Invoke a function on the dedicated script thread and return its result.
    /// Exceptions thrown by the invocation are propagated to the caller.
    /// </summary>
    public TResult Invoke<TResult>(Func<TResult> func)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(func);

        // If already on the script thread, execute directly to avoid deadlock and extra scheduling.
        if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
        {
            return func();
        }

        // Marshal onto the script thread; the worker completes the TCS when done.
        var tcs = new TaskCompletionSource<TResult>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            _queue.Add(new WorkItem<TResult>(func, tcs), _shutdown.Token);
        }
        catch (OperationCanceledException)
        {
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (InvalidOperationException)
        {
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        return tcs.Task.GetAwaiter().GetResult();
    }

    /// <summary>
    /// Invoke an action on the dedicated script thread.
    /// Exceptions thrown by the invocation are propagated to the caller.
    /// </summary>
    public void Invoke(Action action)
    {
        EnsureNotDisposed();
        ArgumentNullException.ThrowIfNull(action);

        // If already on the script thread, execute directly to avoid deadlock and extra scheduling.
        if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
        {
            action();
            return;
        }

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);
        try
        {
            _queue.Add(new WorkItem<object?>(() =>
            {
                action();
                return null;
            }, tcs), _shutdown.Token);
        }
        catch (OperationCanceledException)
        {
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (ObjectDisposedException)
        {
            // The queue can be disposed by the script thread during shutdown.
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (InvalidOperationException)
        {
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }

        // Sync wait to preserve a simple synchronous API surface for callers.
        _ = tcs.Task.GetAwaiter().GetResult();
    }

    public void Dispose()
    {
        // Ensure Dispose is idempotent and safe from multiple callers.
        if (Interlocked.Exchange(ref _disposeSignaled, 1) != 0)
        {
            return;
        }

        // Stop accepting work and wake the consuming enumerable.
        try
        {
            _shutdown.Cancel();
        }
        catch (ObjectDisposedException)
        {
            // If the runtime thread already terminated, it may have disposed the CTS.
        }

        try
        {
            _queue.CompleteAdding();
        }
        catch (ObjectDisposedException)
        {
            // If the runtime thread already terminated, it may have disposed the queue.
        }
        catch (InvalidOperationException)
        {
            // Already marked complete.
        }

        // Avoid self-join if Dispose is called from within the script thread.
        if (Thread.CurrentThread.ManagedThreadId != _thread.ManagedThreadId)
        {
            _ = _thread.Join(DisposeJoinTimeout);
        }

        // This type intentionally has no finalizer (it would be unsafe to block/join on the finalizer thread).
        // Ensure we don't ever pay finalization costs if one is added later.
        GC.SuppressFinalize(this);
    }

    internal bool IsShutdown => _terminated.Task.IsCompleted;

    internal bool WaitForShutdown(TimeSpan timeout)
    {
        // Never block waiting for ourselves.
        if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
        {
            return false;
        }

        return _terminated.Task.Wait(timeout);
    }

    private void ThreadMain(Assembly compiledAssembly, string moduleSpecifier)
    {
        try
        {
            // Extremely defensive: under normal usage, Dispose cannot be called until after the ctor returns
            // (which waits for initialization). This guard prevents configuring thread-affine runtime state
            // if cancellation/disposal is somehow signaled early.
            if (Volatile.Read(ref _disposeSignaled) != 0 || _shutdown.IsCancellationRequested)
            {
                _initialized.TrySetResult();
                return;
            }

            // Configure engine services *for this thread*; the sync context/event loop are thread-affine.
            var serviceProvider = Engine.ConfigureServiceProviderForCurrentThread(compiledAssembly);
            _serviceProvider = serviceProvider;
            _eventLoop = serviceProvider.Resolve<NodeEventLoopPump>();

            // Load/evaluate the entry module (CommonJS require) and capture its exports.
            var require = _serviceProvider.Resolve<Require>();
            _exports = require.RequireModule(moduleSpecifier);

            // Drain microtasks/queued work produced during module evaluation.
            // Timers are intentionally not awaited during initialization.
            Engine.RunEventLoopUntilIdle(_eventLoop, waitForTimers: false);

            // Signal successful initialization after module evaluation completes.
            _initialized.TrySetResult();

            // Process cross-thread invocations serially, while also pumping the JS event loop
            // (including timers) even when the host is idle. This avoids deadlocks where a
            // Promise resolves via setTimeout/setInterval but no new host invocations arrive.
            while (!_shutdown.IsCancellationRequested)
            {
                int waitMs = _eventLoop.GetWaitForWorkOrNextTimerMilliseconds(maxWaitMs: 50);

                if (_queue.TryTake(out var item, waitMs, _shutdown.Token))
                {
                    item.Execute();
                    Engine.RunEventLoopUntilIdle(_eventLoop, waitForTimers: false);
                    continue;
                }

                // Timeout: give the event loop a chance to run due timers/microtasks.
                Engine.RunEventLoopUntilIdle(_eventLoop, waitForTimers: false);
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown path: ensure ctor unblocks even if cancellation occurs during initialization.
            _initialized.TrySetResult();
        }
        catch (Exception ex)
        {
            // Propagate initialization or runtime failures to constructor/Invoke callers.
            _initialized.TrySetException(ex);
        }
        finally
        {
            // Clear ambient global provider to avoid leaking thread-local state after thread exits.
            GlobalThis.ServiceProvider = null;

            // Mark thread termination before disposing shared resources.
            _terminated.TrySetResult();

            // Release managed resources once the owning script thread is done using them.
            _queue.Dispose();
            _shutdown.Dispose();
        }
    }

    private void EnsureNotDisposed()
    {
        if (Volatile.Read(ref _disposeSignaled) != 0)
        {
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
    }

    private static string NormalizeLocalModuleSpecifier(string moduleId)
    {
        var trimmed = moduleId.Trim();

        // Preserve explicit relative/absolute specifiers; otherwise treat as local ("./").
        if (trimmed.StartsWith("./", StringComparison.Ordinal) ||
            trimmed.StartsWith("../", StringComparison.Ordinal) ||
            trimmed.StartsWith("/", StringComparison.Ordinal))
        {
            return trimmed;
        }

        return "./" + trimmed;
    }

    private interface IWorkItem
    {
        void Execute();
    }

    private sealed class WorkItem<TResult> : IWorkItem
    {
        private readonly Func<TResult> _func;
        private readonly TaskCompletionSource<TResult> _tcs;

        public WorkItem(Func<TResult> func, TaskCompletionSource<TResult> tcs)
        {
            _func = func;
            _tcs = tcs;
        }

        public void Execute()
        {
            try
            {
                var result = _func();
                _tcs.TrySetResult(result);
            }
            catch (TargetInvocationException tie) when (tie.InnerException != null)
            {
                // Unwrap reflection invocation exceptions so callers see the underlying JS/runtime error.
                _tcs.TrySetException(tie.InnerException);
            }
            catch (Exception ex)
            {
                _tcs.TrySetException(ex);
            }
        }
    }
}
