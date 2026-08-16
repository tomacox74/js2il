using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Node;

namespace JavaScriptRuntime;

/// <summary>
/// Agent-owned scheduling, asynchronous context, and cooperative shutdown state.
/// </summary>
internal sealed class RuntimeAgentSchedulingState : IDisposable
{
    private readonly object _gate = new();
    private readonly CancellationTokenSource _shutdown = new();
    private NodeSchedulerState? _scheduler;
    private NodeEventLoopPump? _eventLoop;
    private FinalizationRegistryHost? _finalizationHost;
    private ITickSource? _tickSource;
    private IWaitHandle? _wakeup;
    private RuntimeOwnershipState _state;

    internal AsyncContextRuntime AsyncContext { get; } = new();

    internal CancellationToken ShutdownToken => _shutdown.Token;

    internal NodeSchedulerState GetOrCreateScheduler(
        ITickSource tickSource,
        IWaitHandle wakeup)
    {
        ArgumentNullException.ThrowIfNull(tickSource);
        ArgumentNullException.ThrowIfNull(wakeup);

        lock (_gate)
        {
            ThrowIfDisposed();
            if (_scheduler != null)
            {
                return _scheduler;
            }

            _tickSource = tickSource;
            _wakeup = wakeup;
            _scheduler = new NodeSchedulerState(
                tickSource,
                wakeup,
                AsyncContext);
            return _scheduler;
        }
    }

    internal IFinalizationRegistryHost GetOrCreateFinalizationHost(
        Func<NodeSchedulerState> schedulerFactory)
    {
        ArgumentNullException.ThrowIfNull(schedulerFactory);

        lock (_gate)
        {
            ThrowIfDisposed();
            return _finalizationHost ??= new FinalizationRegistryHost(
                schedulerFactory());
        }
    }

    internal NodeEventLoopPump GetOrCreateEventLoop(
        Func<NodeSchedulerState> schedulerFactory)
    {
        ArgumentNullException.ThrowIfNull(schedulerFactory);

        lock (_gate)
        {
            ThrowIfDisposed();
            if (_eventLoop != null)
            {
                return _eventLoop;
            }

            var scheduler = schedulerFactory();
            var tickSource = _tickSource
                ?? throw new InvalidOperationException(
                    "The runtime agent tick source has not been initialized.");
            var wakeup = _wakeup
                ?? throw new InvalidOperationException(
                    "The runtime agent wake handle has not been initialized.");
            var finalizationHost = _finalizationHost
                ??= new FinalizationRegistryHost(scheduler);
            _eventLoop = new NodeEventLoopPump(
                scheduler,
                tickSource,
                wakeup,
                finalizationHost);
            return _eventLoop;
        }
    }

    internal void EnqueueFromExternalThread(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);

        NodeSchedulerState scheduler;
        lock (_gate)
        {
            ThrowIfDisposed();
            scheduler = _scheduler
                ?? throw new InvalidOperationException(
                    "The runtime agent scheduler has not been initialized.");
        }

        if (!scheduler.TryQueueExternalImmediate(callback))
        {
            throw new ObjectDisposedException(nameof(RuntimeAgent));
        }
    }

    internal void RequestShutdown()
    {
        NodeSchedulerState? scheduler;
        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            scheduler = _scheduler;
        }

        if (!_shutdown.IsCancellationRequested)
        {
            _shutdown.Cancel();
        }

        scheduler?.SignalWakeup();
    }

    public void Dispose()
    {
        NodeEventLoopPump? eventLoop;
        NodeSchedulerState? scheduler;
        FinalizationRegistryHost? finalizationHost;

        lock (_gate)
        {
            if (_state == RuntimeOwnershipState.Disposed)
            {
                return;
            }

            _state = RuntimeOwnershipState.Disposing;
            eventLoop = _eventLoop;
            scheduler = _scheduler;
            finalizationHost = _finalizationHost;
        }

        RequestShutdown();
        eventLoop?.Dispose();
        finalizationHost?.Dispose();
        scheduler?.Dispose();
        AsyncContext.Reset();

        lock (_gate)
        {
            _state = RuntimeOwnershipState.Disposed;
        }
    }

    private void ThrowIfDisposed()
    {
        if (_state != RuntimeOwnershipState.Active)
        {
            throw new ObjectDisposedException(nameof(RuntimeAgent));
        }
    }
}
