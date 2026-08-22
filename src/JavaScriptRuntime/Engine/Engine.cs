using System.Diagnostics.CodeAnalysis;
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

        using var lifecycle = RuntimeLifecycle.Create(
            scriptEntryPoint.Method.Module.Assembly,
            isHostedExecution: false,
            existingServices: _serviceProviderOverride.Value);
        lifecycle.Execute(
            serviceProvider =>
            {
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
            },
            waitForTimers: true);
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

    internal static void ConfigureChildProcessIpc(ServiceContainer serviceProvider)
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
