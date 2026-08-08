using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Runtime.CompilerServices;
using JavaScriptRuntime;
using JavaScriptRuntime.CommonJS;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Node;

namespace Jroc.Runtime;

/// <summary>
/// Hosts a single JROC "runtime instance" on a dedicated thread.
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
    private readonly ConcurrentDictionary<IWorkItem, byte> _pendingWorkItems = new();
    private readonly ConcurrentDictionary<IRuntimeDependentOperation, byte> _runtimeDependentOperations = new();

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
    private readonly JsModuleLoadOptions? _options;
    private readonly ConditionalWeakTable<object, JsCallable> _callableWrappers = new();
    private readonly ConditionalWeakTable<Delegate, HostedDelegateFunctionObject> _hostDelegateAdapters = new();
    private readonly ConditionalWeakTable<JsHostFunction, HostedCallbackFunctionObject> _hostFunctionAdapters = new();
    private readonly ConditionalWeakTable<object, ConcurrentDictionary<MethodInfo, HostedMethodFunctionObject>> _hostMethodAdapters = new();

    public JsRuntimeInstance(Assembly compiledAssembly, string moduleId, JsModuleLoadOptions? options = null)
    {
        ArgumentNullException.ThrowIfNull(compiledAssembly);
        ArgumentException.ThrowIfNullOrWhiteSpace(moduleId);

        ModuleId = moduleId.Trim();
        CompiledAssemblyName = compiledAssembly.GetName().Name;
        _options = options;

        // Treat bare specifiers as local modules by default ("foo" -> "./foo").
        var normalized = NormalizeLocalModuleSpecifier(ModuleId);

        _thread = new Thread(() => ThreadMain(compiledAssembly, normalized))
        {
            // Background thread so the host process can exit naturally if it forgets to Dispose.
            // This does not affect scheduling/priority; it only affects process shutdown semantics.
            // We may make this configurable in the future.
            IsBackground = true,
            Name = $"Jroc.ScriptThread[{CompiledAssemblyName}:{normalized}]"
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
        var item = new WorkItem<TResult>(func, tcs);
        RegisterWorkItem(item);
        try
        {
            _queue.Add(item, _shutdown.Token);
        }
        catch (OperationCanceledException)
        {
            FailWorkItem(item);
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (ObjectDisposedException)
        {
            FailWorkItem(item);
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (InvalidOperationException)
        {
            FailWorkItem(item);
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
        var item = new WorkItem<object?>(() =>
        {
            action();
            return null;
        }, tcs);
        RegisterWorkItem(item);
        try
        {
            _queue.Add(item, _shutdown.Token);
        }
        catch (OperationCanceledException)
        {
            FailWorkItem(item);
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (ObjectDisposedException)
        {
            FailWorkItem(item);
            throw new ObjectDisposedException(nameof(JsRuntimeInstance));
        }
        catch (InvalidOperationException)
        {
            FailWorkItem(item);
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

        FailPendingOperations();

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

    internal int PendingWorkItemCount => _pendingWorkItems.Count;

    internal bool WaitForShutdown(TimeSpan timeout)
    {
        // Never block waiting for ourselves.
        if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
        {
            return false;
        }

        return _terminated.Task.Wait(timeout);
    }

    internal JsCallable GetOrCreateCallableWrapper(object callable)
    {
        ArgumentNullException.ThrowIfNull(callable);
        return _callableWrappers.GetValue(
            callable,
            target => new JsCallable(this, target));
    }

    internal HostedMethodFunctionObject GetOrCreateHostMethodAdapter(
        object target,
        MethodInfo method)
    {
        var methods = _hostMethodAdapters.GetOrCreateValue(target);
        return methods.GetOrAdd(
            method,
            candidate => new HostedMethodFunctionObject(this, target, candidate));
    }

    internal bool TryPost(Action action)
    {
        ArgumentNullException.ThrowIfNull(action);

        if (Volatile.Read(ref _disposeSignaled) != 0)
        {
            return false;
        }

        if (Thread.CurrentThread.ManagedThreadId == _thread.ManagedThreadId)
        {
            action();
            return true;
        }

        var item = new PostedWorkItem(action);
        if (!TryRegisterWorkItem(item))
        {
            return false;
        }

        try
        {
            _queue.Add(item, _shutdown.Token);
            return true;
        }
        catch (Exception exception)
            when (exception is OperationCanceledException
                or ObjectDisposedException
                or InvalidOperationException)
        {
            FailWorkItem(item);
            return false;
        }
    }

    internal bool TryRegisterRuntimeDependentOperation(
        IRuntimeDependentOperation operation)
    {
        ArgumentNullException.ThrowIfNull(operation);

        if (Volatile.Read(ref _disposeSignaled) != 0)
        {
            operation.OnRuntimeDisposed(CreateDisposedException());
            return false;
        }

        if (!_runtimeDependentOperations.TryAdd(operation, 0))
        {
            throw new InvalidOperationException("The runtime-dependent operation is already registered.");
        }

        if (Volatile.Read(ref _disposeSignaled) == 0)
        {
            return true;
        }

        if (_runtimeDependentOperations.TryRemove(operation, out _))
        {
            operation.OnRuntimeDisposed(CreateDisposedException());
        }

        return false;
    }

    internal void UnregisterRuntimeDependentOperation(
        IRuntimeDependentOperation operation)
        => _runtimeDependentOperations.TryRemove(operation, out _);

    internal object? NormalizeHostValue(object? value)
    {
        if (value is null)
        {
            return null;
        }

        value = value switch
        {
            JsCallable callable => callable.Unwrap(this),
            JsDynamicValueProxy proxy => proxy.Unwrap(this),
            JsDynamicExports exports => exports.UnwrapExports(this),
            JsHandleProxy handleProxy => handleProxy.UnwrapTarget(this),
            JsConstructorProxy ctorProxy => ctorProxy.UnwrapConstructor(this),
            Task task => JsTaskPromiseInterop.ToPromise(this, task),
            JsHostFunction hostFunction => _hostFunctionAdapters.GetValue(
                hostFunction,
                descriptor => new HostedCallbackFunctionObject(this, descriptor)),
            Delegate callback => _hostDelegateAdapters.GetValue(
                callback,
                target => new HostedDelegateFunctionObject(this, target)),
            _ => value,
        };

        return value switch
        {
            double => value,
            float single => (double)single,
            decimal number => (double)number,
            sbyte or byte or short or ushort or int or uint or long or ulong
                => Convert.ToDouble(value, CultureInfo.InvariantCulture),
            char character => character.ToString(),
            _ => value,
        };
    }

    internal object[] NormalizeHostArguments(object?[]? arguments)
    {
        if (arguments == null || arguments.Length == 0)
        {
            return System.Array.Empty<object>();
        }

        var normalized = new object[arguments.Length];
        for (var index = 0; index < arguments.Length; index++)
        {
            normalized[index] = NormalizeHostValue(arguments[index])!;
        }

        return normalized;
    }

    internal object? ProjectHostValue(object? value)
        => JsDynamicValueProxy.Wrap(this, value);

    internal object?[] ProjectHostArguments(object?[] arguments)
    {
        if (arguments.Length == 0)
        {
            return System.Array.Empty<object?>();
        }

        var projected = new object?[arguments.Length];
        for (var index = 0; index < arguments.Length; index++)
        {
            projected[index] = ProjectHostValue(arguments[index]);
        }

        return projected;
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
            var serviceProvider = Engine.ConfigureServiceProviderForCurrentThread(
                compiledAssembly,
                isHostedExecution: true,
                compiledAssemblyPath: _options?.CompiledAssemblyPath);
            _serviceProvider = serviceProvider;

            if (_options?.HostRuntimeIntrinsics != null)
            {
                serviceProvider.Replace(_options.HostRuntimeIntrinsics);
            }

            if (_options?.ChildProcessLauncher != null)
            {
                serviceProvider.RegisterInstance<IChildProcessLauncher>(_options.ChildProcessLauncher);
            }

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
                    try
                    {
                        item.Execute();
                    }
                    finally
                    {
                        _pendingWorkItems.TryRemove(item, out _);
                    }
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
            FailPendingOperations();

            if (_serviceProvider?.TryResolve<RuntimeExecutionContext>(out var runtimeContext) == true && runtimeContext != null)
            {
                RuntimeServices.UnregisterModuleRequires(runtimeContext.RegisteredModuleRequires);
            }

            // Clear ambient global provider to avoid leaking thread-local state after thread exits.
            GlobalThis.ServiceProvider = null;
            _exports = null;
            _eventLoop = null;
            _serviceProvider = null;

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

    private void RegisterWorkItem(IWorkItem item)
    {
        if (!TryRegisterWorkItem(item))
        {
            throw CreateDisposedException();
        }
    }

    private bool TryRegisterWorkItem(IWorkItem item)
    {
        if (Volatile.Read(ref _disposeSignaled) != 0)
        {
            item.FailIfNotStarted(CreateDisposedException());
            return false;
        }

        if (!_pendingWorkItems.TryAdd(item, 0))
        {
            throw new InvalidOperationException("The work item is already registered.");
        }

        if (Volatile.Read(ref _disposeSignaled) == 0)
        {
            return true;
        }

        FailWorkItem(item);
        return false;
    }

    private void FailWorkItem(IWorkItem item)
    {
        if (_pendingWorkItems.TryRemove(item, out _))
        {
            item.FailIfNotStarted(CreateDisposedException());
        }
    }

    private void FailPendingOperations()
    {
        foreach (var item in _pendingWorkItems.Keys)
        {
            FailWorkItem(item);
        }

        foreach (var operation in _runtimeDependentOperations.Keys)
        {
            if (_runtimeDependentOperations.TryRemove(operation, out _))
            {
                operation.OnRuntimeDisposed(CreateDisposedException());
            }
        }
    }

    private static ObjectDisposedException CreateDisposedException()
        => new(nameof(JsRuntimeInstance));

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
        void FailIfNotStarted(Exception exception);
    }

    private sealed class WorkItem<TResult> : IWorkItem
    {
        private readonly Func<TResult> _func;
        private readonly TaskCompletionSource<TResult> _tcs;
        private int _state;

        public WorkItem(Func<TResult> func, TaskCompletionSource<TResult> tcs)
        {
            _func = func;
            _tcs = tcs;
        }

        public void Execute()
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            {
                return;
            }

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
            finally
            {
                Volatile.Write(ref _state, 2);
            }
        }

        public void FailIfNotStarted(Exception exception)
        {
            if (Interlocked.CompareExchange(ref _state, 2, 0) == 0)
            {
                _tcs.TrySetException(exception);
            }
        }
    }

    private sealed class PostedWorkItem : IWorkItem
    {
        private readonly Action _action;
        private int _state;

        public PostedWorkItem(Action action)
        {
            _action = action;
        }

        public void Execute()
        {
            if (Interlocked.CompareExchange(ref _state, 1, 0) != 0)
            {
                return;
            }

            try
            {
                _action();
            }
            finally
            {
                Volatile.Write(ref _state, 2);
            }
        }

        public void FailIfNotStarted(Exception exception)
            => Interlocked.CompareExchange(ref _state, 2, 0);
    }
}

internal interface IRuntimeDependentOperation
{
    void OnRuntimeDisposed(ObjectDisposedException exception);
}
