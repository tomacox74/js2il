using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JavaScriptRuntime.CommonJS;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;

namespace JavaScriptRuntime;

/// <summary>
/// Entry point for executing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    /// <summary>
    /// Per-thread override for the runtime service provider (primarily used by tests).
    /// If set, <see cref="ConfigureServiceProviderForCurrentThread"/> will use it instead of building a new container.
    /// </summary>
    internal readonly static ThreadLocal<ServiceContainer?> _serviceProviderOverride = new(() => null);

    public void Execute([NotNull] ModuleMainDelegate scriptEntryPoint)
    {
        try
        {
            // Validate caller provided a valid entry point delegate.
            // Note: the delegate's Method/Module/Assembly is used to discover the compiled module assembly.
            ArgumentNullException.ThrowIfNull(scriptEntryPoint);

            // Configure per-thread services and install the Node-like synchronization context.
            // This enables timers/microtasks and other async behavior to run deterministically on this thread.
            var serviceProvider = ConfigureServiceProviderForCurrentThread(
                modulesAssembly: scriptEntryPoint.Method.Module.Assembly);

            // Execute the script using the CommonJS module system.
            // Future: Add ESM support with a different executor.
            var moduleExecutor = new ModuleExecutor(serviceProvider);
            moduleExecutor.Execute(scriptEntryPoint);

            // Drain microtasks and timers until no more work remains.
            RunEventLoopUntilIdle(serviceProvider.Resolve<NodeEventLoopPump>(), waitForTimers: true);
        }
        finally
        {
            // Cleanup global/thread-local state so repeated Engine.Execute calls (and tests) do not leak state.
            // TODO: change globalthis to be a instance
            GlobalThis.ServiceProvider = null;
            _serviceProviderOverride.Value = null;
        }
    }

    internal static ServiceContainer ConfigureServiceProviderForCurrentThread(Assembly modulesAssembly)
    {
        ArgumentNullException.ThrowIfNull(modulesAssembly);

        // Prevent accidentally hosting multiple runtimes on the same thread.
        // This catches common integration bugs where a host thread is reused and global state leaks.
        if (GlobalThis.ServiceProvider != null)
        {
            throw new InvalidOperationException(
                "A JS2IL runtime is already configured for the current thread. " +
                "Create a dedicated thread per loaded module/runtime, or ensure the previous engine cleaned up correctly.");
        }

        // Use the test override if present; otherwise construct the default runtime container.
        var serviceProvider = _serviceProviderOverride.Value ?? RuntimeServices.BuildServiceProvider();

        // Resolve scheduler/event-loop singletons via DI so other services can depend on them.
        // Note: ServiceContainer manages singleton instances per-container.
        _ = serviceProvider.Resolve<NodeSchedulerState>();
        _ = serviceProvider.Resolve<NodeEventLoopPump>();

        // Expose the service provider via GlobalThis (current design uses global state).
        GlobalThis.ServiceProvider = serviceProvider;

        // Provide the compiled modules assembly for runtime dependency/module resolution.
        serviceProvider.Resolve<LocalModulesAssembly>().ModulesAssembly = modulesAssembly;

        return serviceProvider;
    }

    internal static void RunEventLoopUntilIdle(NodeEventLoopPump ctx, bool waitForTimers)
    {
        ArgumentNullException.ThrowIfNull(ctx);

        if (waitForTimers)
        {
            // Drain everything, including future timers (blocking between ticks).
            while (ctx.HasPendingWork())
            {
                ctx.RunOneIteration();
                ctx.WaitForWorkOrNextTimer();
            }
            return;
        }

        // Drain only work that is runnable *now* (microtasks, immediates, macrotasks,
        // and timers that are already due). Do not busy-loop waiting for future timers.
        while (ctx.HasPendingWorkNow())
        {
            ctx.RunOneIteration();
        }
    }
}