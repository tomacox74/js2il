using System.Reflection;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime;

internal sealed class RuntimeLifecycle : IDisposable
{
    private readonly IDisposable? _ambientSuppression;
    private readonly bool _ownsAgent;
    private readonly bool _ownsCluster;
    private int _disposed;

    private RuntimeLifecycle(
        RuntimeAgentCluster cluster,
        RuntimeAgent agent,
        RuntimeRealm realm,
        ServiceContainer services,
        RuntimeExecutionContext executionContext,
        NodeEventLoopPump eventLoop,
        bool ownsAgent,
        bool ownsCluster,
        IDisposable? ambientSuppression)
    {
        Cluster = cluster;
        Agent = agent;
        Realm = realm;
        Services = services;
        ExecutionContext = executionContext;
        EventLoop = eventLoop;
        _ownsAgent = ownsAgent;
        _ownsCluster = ownsCluster;
        _ambientSuppression = ambientSuppression;
    }

    internal RuntimeAgentCluster Cluster { get; }

    internal RuntimeAgent Agent { get; }

    internal RuntimeRealm Realm { get; }

    internal ServiceContainer Services { get; }

    internal RuntimeExecutionContext ExecutionContext { get; }

    internal NodeEventLoopPump EventLoop { get; }

    internal static RuntimeLifecycle Create(
        Assembly modulesAssembly,
        bool isHostedExecution,
        string? compiledAssemblyPath = null,
        RuntimeAgentCluster? cluster = null,
        ServiceContainer? existingServices = null,
        Action<ServiceContainer>? configureServices = null,
        bool suppressInheritedExecutionContext = false)
    {
        ArgumentNullException.ThrowIfNull(modulesAssembly);
        if (cluster != null && existingServices != null)
        {
            throw new ArgumentException(
                "An existing service container already determines its runtime agent cluster.");
        }

        IDisposable? ambientSuppression = null;
        RuntimeAgentCluster? selectedCluster = null;
        RuntimeAgent? agent = null;
        var ownsAgent = false;
        var ownsCluster = false;

        try
        {
            if (suppressInheritedExecutionContext)
            {
                ambientSuppression = RuntimeExecutionContext.SuppressInheritedState();
            }

            if (RuntimeExecutionContext.Current != null)
            {
                throw new InvalidOperationException(
                    "A JROC runtime execution frame is already active. " +
                    "Exit the current frame before starting another engine.");
            }

            RuntimeRealm realm;
            ServiceContainer services;
            if (existingServices != null)
            {
                realm = existingServices.OwningRealm
                    ?? throw new InvalidOperationException(
                        "The supplied runtime service container has no owning realm.");
                if (realm.IsDisposed)
                {
                    throw new ObjectDisposedException(nameof(RuntimeRealm));
                }

                selectedCluster = realm.Agent.Cluster;
                agent = realm.Agent;
                services = existingServices;
            }
            else
            {
                selectedCluster = cluster ?? new RuntimeAgentCluster();
                ownsCluster = cluster == null;
                agent = selectedCluster.CreateAgent();
                ownsAgent = true;
                realm = agent.CreateRealm();
                services = realm.Services;
                RuntimeServices.ConfigureServiceProvider(services);
            }

            configureServices?.Invoke(services);

            var executionContext = RuntimeExecutionContext.GetOrCreate(
                services,
                isHostedExecution,
                CompiledAssemblyPathResolver.Resolve(
                    modulesAssembly,
                    compiledAssemblyPath,
                    allowAssemblyLocationFallback: !isHostedExecution));

            realm.ModuleState.ModulesAssembly = modulesAssembly;
            _ = services.Resolve<NodeSchedulerState>();
            var eventLoop = services.Resolve<NodeEventLoopPump>();

            return new RuntimeLifecycle(
                selectedCluster,
                agent,
                realm,
                services,
                executionContext,
                eventLoop,
                ownsAgent,
                ownsCluster,
                ambientSuppression);
        }
        catch
        {
            try
            {
                if (ownsAgent)
                {
                    agent?.Dispose();
                }
            }
            finally
            {
                try
                {
                    if (ownsCluster)
                    {
                        selectedCluster?.Dispose();
                    }
                }
                finally
                {
                    ambientSuppression?.Dispose();
                }
            }

            throw;
        }
    }

    internal IDisposable EnterAsRoot()
    {
        ThrowIfDisposed();
        return ExecutionContext.EnterAsRoot();
    }

    internal void Execute(
        Action<ServiceContainer> entryPoint,
        bool waitForTimers)
    {
        ArgumentNullException.ThrowIfNull(entryPoint);
        ThrowIfDisposed();

        using var scope = EnterAsRoot();
        entryPoint(Services);
        Engine.RunEventLoopUntilIdle(EventLoop, waitForTimers);
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) != 0)
        {
            return;
        }

        try
        {
            if (_ownsAgent)
            {
                Agent.Dispose();
            }
            else
            {
                Agent.Scheduling.AsyncContext.Reset();
            }
        }
        finally
        {
            try
            {
                if (_ownsCluster)
                {
                    Cluster.Dispose();
                }
            }
            finally
            {
                _ambientSuppression?.Dispose();
            }
        }
    }

    private void ThrowIfDisposed()
    {
        if (Volatile.Read(ref _disposed) != 0)
        {
            throw new ObjectDisposedException(nameof(RuntimeLifecycle));
        }
    }
}
