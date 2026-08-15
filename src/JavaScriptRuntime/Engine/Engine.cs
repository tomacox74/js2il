using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using JavaScriptRuntime.Modules.CommonJS;
using JavaScriptRuntime.Modules.Shared;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Node;

namespace JavaScriptRuntime;

/// <summary>
/// Entry point for executing JavaScript code that has been compiled to a dotnet assembly.
/// </summary>
public class Engine
{
    internal static readonly RuntimeServiceProviderOverride _serviceProviderOverride = new();

    public void Execute([NotNull] ModuleMainDelegate scriptEntryPoint)
    {
        ArgumentNullException.ThrowIfNull(scriptEntryPoint);

        try
        {
            var serviceProvider = ConfigureRuntime(
                modulesAssembly: scriptEntryPoint.Method.Module.Assembly,
                isHostedExecution: false);
            var runtimeContext = serviceProvider.Resolve<RuntimeExecutionContext>();

            using (runtimeContext.EnterAsRoot())
            {
                try
                {
                    RuntimeServices.SetCurrentThis(null);
                    ConfigureChildProcessIpc(serviceProvider);
                    var moduleExecutor = new ModuleExecutor(serviceProvider);

                    var forkEntryModule = System.Environment.GetEnvironmentVariable(
                        ChildProcessRuntimeOptions.ForkEntryModuleEnvVar);
                    if (!string.IsNullOrWhiteSpace(forkEntryModule))
                    {
                        moduleExecutor.Execute(scriptEntryPoint, forkEntryModule);
                    }
                    else
                    {
                        moduleExecutor.Execute(scriptEntryPoint);
                    }

                    RunEventLoopUntilIdle(
                        serviceProvider.Resolve<NodeEventLoopPump>(),
                        waitForTimers: true);
                }
                finally
                {
                    RuntimeServices.UnregisterModuleRequires(
                        runtimeContext.RegisteredModuleRequires);
                    if (serviceProvider.TryResolve<AsyncContextRuntime>(
                            out var asyncContext)
                        && asyncContext != null)
                    {
                        asyncContext.Reset();
                    }

                    RuntimeServices.SetCurrentThis(null);
                }
            }
        }
        finally
        {
            _serviceProviderOverride.Value = null;
        }
    }

    internal static ServiceContainer ConfigureRuntime(
        Assembly modulesAssembly,
        bool isHostedExecution = false,
        string? compiledAssemblyPath = null)
    {
        ArgumentNullException.ThrowIfNull(modulesAssembly);

        // Prevent accidentally hosting multiple runtimes on the same thread.
        // This catches common integration bugs where a host thread is reused and global state leaks.
        if (RuntimeExecutionContext.Current != null)
        {
            throw new InvalidOperationException(
                "A JROC runtime execution frame is already active. " +
                "Exit the current frame before starting another engine.");
        }

        // Use the test override if present; otherwise construct the default runtime container.
        var serviceProvider = _serviceProviderOverride.Value ?? RuntimeServices.BuildServiceProvider();

        _ = RuntimeExecutionContext.GetOrCreate(
            serviceProvider,
            isHostedExecution,
            CompiledAssemblyPathResolver.Resolve(
                modulesAssembly,
                compiledAssemblyPath,
                allowAssemblyLocationFallback: !isHostedExecution));

        // Resolve scheduler/event-loop singletons via DI so other services can depend on them.
        // Note: ServiceContainer manages singleton instances per-container.
        _ = serviceProvider.Resolve<NodeSchedulerState>();
        _ = serviceProvider.Resolve<NodeEventLoopPump>();

        // Provide the compiled modules assembly for runtime dependency/module resolution.
        serviceProvider.Resolve<LocalModulesAssembly>().ModulesAssembly = modulesAssembly;

        return serviceProvider;
    }

    internal sealed class RuntimeServiceProviderOverride
    {
        internal ServiceContainer? Value
        {
            get => RuntimeExecutionContext.ServiceProviderOverride;
            set => RuntimeExecutionContext.ServiceProviderOverride = value;
        }
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

    private static void ConfigureChildProcessIpc(ServiceContainer serviceProvider)
    {
        var portText = System.Environment.GetEnvironmentVariable(ChildProcessRuntimeOptions.ForkIpcPortEnvVar);
        var ipcToken = System.Environment.GetEnvironmentVariable(ChildProcessRuntimeOptions.ForkIpcTokenEnvVar);
        if (string.IsNullOrWhiteSpace(portText) || !int.TryParse(portText, out var port))
        {
            return;
        }

        if (string.IsNullOrWhiteSpace(ipcToken))
        {
            throw new Error("child_process IPC bootstrap is missing the required authentication token.");
        }

        var scheduler = serviceProvider.Resolve<NodeSchedulerState>();
        var ioScheduler = serviceProvider.Resolve<IIOScheduler>();
        var channel = ChildProcessIpcChannel.CreateClient(port, ipcToken, action => NodeNetworkingCommon.ScheduleImmediateOnEventLoop(scheduler, action), ioScheduler);
        channel.Start();
        serviceProvider.RegisterInstance(channel);
    }
}
