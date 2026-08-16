using JavaScriptRuntime;
using JavaScriptRuntime.DependencyInjection;
using JavaScriptRuntime.EngineCore;
using JavaScriptRuntime.Node;

namespace Jroc.Tests;

public sealed class RuntimeAgentSchedulingTests
{
    [Fact]
    public void RealmsInOneAgentShareSchedulingAndAsyncContextServices()
    {
        var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var first = CreateConfiguredRealm(agent);
        var second = CreateConfiguredRealm(agent);

        var firstScheduler = first.Services.Resolve<NodeSchedulerState>();
        var firstEventLoop = first.Services.Resolve<NodeEventLoopPump>();

        Assert.Same(firstScheduler, second.Services.Resolve<NodeSchedulerState>());
        Assert.Same(firstScheduler, second.Services.Resolve<IScheduler>());
        Assert.Same(firstScheduler, second.Services.Resolve<IMicrotaskScheduler>());
        Assert.Same(firstEventLoop, second.Services.Resolve<NodeEventLoopPump>());
        Assert.Same(
            first.Services.Resolve<AsyncContextRuntime>(),
            second.Services.Resolve<AsyncContextRuntime>());
        Assert.Same(
            first.Services.Resolve<IFinalizationRegistryHost>(),
            second.Services.Resolve<IFinalizationRegistryHost>());
        Assert.Same(
            agent.Scheduling,
            first.Services.Resolve<RuntimeAgentSchedulingState>());

        cluster.Dispose();
    }

    [Fact]
    public void AgentsDrainTimersMicrotasksAndCallbacksIndependently()
    {
        var first = CreateAgentRuntime();
        var second = CreateAgentRuntime();
        var firstCallbacks = new List<string>();
        var secondCallbacks = new List<string>();

        QueueAllPhases(first.Services, firstCallbacks);
        QueueAllPhases(second.Services, secondCallbacks);

        DrainRunnableWork(first.EventLoop);

        Assert.Equal(["nextTick", "microtask", "immediate", "timer"], firstCallbacks);
        Assert.Empty(secondCallbacks);
        Assert.True(second.EventLoop.HasPendingWork());

        DrainRunnableWork(second.EventLoop);

        Assert.Equal(["nextTick", "microtask", "immediate", "timer"], secondCallbacks);
        first.Cluster.Dispose();
        second.Cluster.Dispose();
    }

    [Fact]
    public void AgentsRunSchedulingAndAsyncContextConcurrentlyWithoutCrossDelivery()
    {
        using var start = new Barrier(3);
        var firstCallbacks = new List<string>();
        var secondCallbacks = new List<string>();
        Exception? firstFailure = null;
        Exception? secondFailure = null;
        var firstThread = new Thread(
            () => RunConcurrentAgent(
                "first",
                start,
                firstCallbacks,
                exception => firstFailure = exception));
        var secondThread = new Thread(
            () => RunConcurrentAgent(
                "second",
                start,
                secondCallbacks,
                exception => secondFailure = exception));

        firstThread.Start();
        secondThread.Start();
        Assert.True(start.SignalAndWait(TimeSpan.FromSeconds(5)));
        firstThread.Join();
        secondThread.Join();

        Assert.Null(firstFailure);
        Assert.Null(secondFailure);
        Assert.Equal(
            ["first-nextTick", "first-microtask", "first-store", "first-immediate", "first-timer"],
            firstCallbacks);
        Assert.Equal(
            ["second-nextTick", "second-microtask", "second-store", "second-immediate", "second-timer"],
            secondCallbacks);
    }

    [Fact]
    public void ExternalWakeRunsOnlyOnTheOwningAgentExecutor()
    {
        var runtime = CreateAgentRuntime();
        var ownerThreadId = Environment.CurrentManagedThreadId;
        var callbackThreadId = 0;
        Exception? producerFailure = null;

        var producer = new Thread(() =>
        {
            try
            {
                runtime.Agent.EnqueueFromExternalThread(
                    () => callbackThreadId = Environment.CurrentManagedThreadId);
            }
            catch (Exception exception)
            {
                producerFailure = exception;
            }
        });
        producer.Start();
        producer.Join();

        Assert.Null(producerFailure);
        Assert.Equal(0, callbackThreadId);
        runtime.EventLoop.RunOneIteration();
        Assert.Equal(ownerThreadId, callbackThreadId);

        Exception? wrongThreadFailure = null;
        var wrongExecutor = new Thread(() =>
        {
            wrongThreadFailure = Record.Exception(
                runtime.EventLoop.RunOneIteration);
        });
        wrongExecutor.Start();
        wrongExecutor.Join();

        Assert.IsType<InvalidOperationException>(wrongThreadFailure);
        runtime.Cluster.Dispose();
    }

    [Fact]
    public void DisposingOneAgentCancelsAndClearsOnlyItsOwnWork()
    {
        var first = CreateAgentRuntime();
        var second = CreateAgentRuntime();
        var firstRan = false;
        var secondRan = false;
        var firstScheduler = first.Services.Resolve<NodeSchedulerState>();

        first.Agent.EnqueueFromExternalThread(() => firstRan = true);
        second.Agent.EnqueueFromExternalThread(() => secondRan = true);
        firstScheduler.BeginIo();
        _ = first.Services.Resolve<IScheduler>().Schedule(
            () => firstRan = true,
            TimeSpan.FromHours(1));

        first.Agent.Dispose();

        Assert.True(first.Agent.ShutdownToken.IsCancellationRequested);
        Assert.False(firstScheduler.HasPendingWork());
        Assert.Equal(0, firstScheduler.PendingIoCount);
        Assert.Throws<ObjectDisposedException>(
            () => first.Agent.EnqueueFromExternalThread(() => { }));
        Assert.False(firstRan);
        Assert.True(second.EventLoop.HasPendingWork());

        second.EventLoop.RunOneIteration();
        Assert.True(secondRan);
        Assert.False(second.Agent.ShutdownToken.IsCancellationRequested);

        first.Cluster.Dispose();
        second.Cluster.Dispose();
    }

    [Fact]
    public void ConcurrentAgentDisposalStopsBeforeTheNextQueuedCallback()
    {
        var runtime = CreateAgentRuntime();
        var firstRan = false;
        var secondRan = false;
        Thread? disposer = null;

        runtime.Agent.EnqueueFromExternalThread(() =>
        {
            firstRan = true;
            disposer = new Thread(runtime.Agent.Dispose);
            disposer.Start();
            Assert.True(
                SpinWait.SpinUntil(
                    () => runtime.Agent.ShutdownToken.IsCancellationRequested,
                    TimeSpan.FromSeconds(5)));
        });
        runtime.Agent.EnqueueFromExternalThread(() => secondRan = true);

        runtime.EventLoop.RunOneIteration();
        disposer!.Join();

        Assert.True(firstRan);
        Assert.False(secondRan);
        Assert.True(runtime.Agent.IsDisposed);
        runtime.Cluster.Dispose();
    }

    [Fact]
    public void ShutdownPreventsNewRealmsWithoutAffectingAnotherAgent()
    {
        var first = CreateAgentRuntime();
        var second = CreateAgentRuntime();

        first.Agent.RequestShutdown();

        Assert.True(first.Agent.ShutdownToken.IsCancellationRequested);
        Assert.Throws<ObjectDisposedException>(first.Agent.CreateRealm);
        Assert.Throws<ObjectDisposedException>(
            () => first.Agent.EnqueueFromExternalThread(() => { }));
        Assert.False(second.Agent.ShutdownToken.IsCancellationRequested);

        var additionalRealm = second.Agent.CreateRealm();
        Assert.Same(second.Agent, additionalRealm.Agent);

        first.Agent.Dispose();
        first.Cluster.Dispose();
        second.Cluster.Dispose();
    }

    [Fact]
    public void AgentSchedulingServicesCannotBeReplacedOrRemoved()
    {
        var runtime = CreateAgentRuntime();

        Assert.Throws<InvalidOperationException>(
            () => runtime.Services.Replace(new AsyncContextRuntime()));
        Assert.Throws<InvalidOperationException>(
            () => runtime.Services.Remove<NodeSchedulerState>());
        Assert.Throws<InvalidOperationException>(
            () => runtime.Services.Remove<NodeEventLoopPump>());
        Assert.Throws<InvalidOperationException>(
            () => runtime.Services.Remove<IScheduler>());

        runtime.Services.Clear();

        Assert.Same(
            runtime.Agent.Scheduling.AsyncContext,
            runtime.Services.Resolve<AsyncContextRuntime>());
        Assert.Same(
            runtime.Agent.Scheduling,
            runtime.Services.Resolve<RuntimeAgentSchedulingState>());

        runtime.Cluster.Dispose();
    }

    private static AgentRuntime CreateAgentRuntime()
    {
        var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = CreateConfiguredRealm(agent);
        return new AgentRuntime(
            cluster,
            agent,
            realm.Services,
            realm.Services.Resolve<NodeEventLoopPump>());
    }

    private static RuntimeRealm CreateConfiguredRealm(RuntimeAgent agent)
        => agent.CreateRealm();

    private static void QueueAllPhases(
        ServiceContainer services,
        List<string> callbacks,
        string prefix = "")
    {
        var scheduler = services.Resolve<NodeSchedulerState>();
        scheduler.QueueNextTick(() => callbacks.Add($"{prefix}nextTick"));
        services.Resolve<IMicrotaskScheduler>()
            .QueueMicrotask(() => callbacks.Add($"{prefix}microtask"));
        _ = services.Resolve<IScheduler>()
            .ScheduleImmediate(() => callbacks.Add($"{prefix}immediate"));
        _ = services.Resolve<IScheduler>()
            .Schedule(() => callbacks.Add($"{prefix}timer"), TimeSpan.Zero);
    }

    private static void RunConcurrentAgent(
        string name,
        Barrier start,
        List<string> callbacks,
        Action<Exception> recordFailure)
    {
        RuntimeAgentCluster? cluster = null;
        try
        {
            var runtime = CreateAgentRuntime();
            cluster = runtime.Cluster;
            var asyncContext = runtime.Services.Resolve<AsyncContextRuntime>();
            var storage = new AsyncLocalStorageObject(
                asyncContext,
                options: null,
                prototype: new JsObject());
            storage.enterWith($"{name}-store");

            QueueAllPhases(runtime.Services, callbacks, $"{name}-");
            runtime.Services.Resolve<IMicrotaskScheduler>()
                .QueueMicrotask(
                    () => callbacks.Add(
                        storage.getStore()?.ToString()
                            ?? throw new InvalidOperationException(
                                "The agent async context store was not restored.")));

            if (!start.SignalAndWait(TimeSpan.FromSeconds(5)))
            {
                throw new TimeoutException(
                    "Timed out waiting for the concurrent agent start barrier.");
            }

            DrainRunnableWork(runtime.EventLoop);
        }
        catch (Exception exception)
        {
            recordFailure(exception);
        }
        finally
        {
            cluster?.Dispose();
        }
    }

    private static void DrainRunnableWork(NodeEventLoopPump eventLoop)
    {
        while (eventLoop.HasPendingWorkNow())
        {
            eventLoop.RunOneIteration();
        }
    }

    private sealed record AgentRuntime(
        RuntimeAgentCluster Cluster,
        RuntimeAgent Agent,
        ServiceContainer Services,
        NodeEventLoopPump EventLoop);
}
