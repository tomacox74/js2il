using System.Numerics;
using JavaScriptRuntime;
using JavaScriptRuntime.EngineCore;

namespace Jroc.Tests;

public sealed class AtomicsWaitAsyncRuntimeTests
{
    [Fact]
    public async Task WaitWithoutTimeoutBlocksUntilNotification()
    {
        using var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(realm.Services);
        var context = RuntimeExecutionContext.GetOrCreate(realm.Services);
        SharedArrayBuffer buffer;
        using (context.EnterAsRoot())
        {
            buffer = new SharedArrayBuffer(4d);
        }

        var waiting = Task.Run(() =>
        {
            using var scope = context.EnterAsRoot();
            return Atomics.wait(new Int32Array(buffer), 0d, 0d);
        });

        try
        {
            Assert.True(SpinWait.SpinUntil(
                () => cluster.SharedServices.Atomics.WaiterCount == 1,
                TimeSpan.FromSeconds(5)));
            using (context.EnterAsRoot())
            {
                Assert.Equal(1d, Atomics.notify(new Int32Array(buffer), 0d, 1d));
            }
            Assert.Equal("ok", await waiting.WaitAsync(TimeSpan.FromSeconds(5)));
        }
        finally
        {
            agent.Dispose();
        }
    }

    [Fact]
    public void WaitAsyncNotifiesOnlyMatchingByteOffsetAndResolvesOnMicrotask()
    {
        using var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(realm.Services);
        var scheduler = (NodeSchedulerState)realm.Services.Resolve<IMicrotaskScheduler>();
        using var scope = RuntimeExecutionContext.GetOrCreate(realm.Services).EnterAsRoot();

        var buffer = new SharedArrayBuffer(16d);
        var first = new Int32Array(buffer);
        var second = new Int32Array(buffer, 4d);
        var result = Atomics.waitAsync(second, 0d, 0d, double.PositiveInfinity);
        Assert.True((bool)ObjectRuntime.GetProperty(result, "async")!);
        Assert.Equal(1, scheduler.PendingIoCount);
        var promise = Assert.IsType<JavaScriptRuntime.Promise>(ObjectRuntime.GetProperty(result, "value"));
        Assert.Equal(0d, Atomics.notify(first, 0d, null));
        Assert.Equal(1d, Atomics.notify(first, 1d, null));
        Assert.Equal(0d, Atomics.notify(second, 0d, null));
        while (scheduler.TryDequeueMicrotask(out var job))
        {
            job!();
        }
        Assert.Equal("ok", JavaScriptRuntime.Promise.AwaitValue(promise));
        Assert.Equal(0, scheduler.PendingIoCount);
    }

    [Fact]
    public void BigInt64WaitAndImmediateOutcomesUseAtomicValue()
    {
        using var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(realm.Services);
        using var scope = RuntimeExecutionContext.GetOrCreate(realm.Services).EnterAsRoot();

        var view = new BigInt64Array(new SharedArrayBuffer(8d));
        var mismatch = Atomics.waitAsync(view, 0d, BigInteger.One, null);
        Assert.False((bool)ObjectRuntime.GetProperty(mismatch, "async")!);
        Assert.Equal("not-equal", ObjectRuntime.GetProperty(mismatch, "value"));

        var timedOut = Atomics.waitAsync(view, 0d, BigInteger.Zero, 0d);
        Assert.False((bool)ObjectRuntime.GetProperty(timedOut, "async")!);
        Assert.Equal("timed-out", ObjectRuntime.GetProperty(timedOut, "value"));
        Assert.Equal(0, cluster.SharedServices.Atomics.WaiterCount);
    }

    [Fact]
    public void TimeoutRemovesWaiterAndQueuesPromiseResolution()
    {
        using var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(realm.Services);
        var scheduler = (NodeSchedulerState)realm.Services.Resolve<IMicrotaskScheduler>();
        using var scope = RuntimeExecutionContext.GetOrCreate(realm.Services).EnterAsRoot();

        var view = new Int32Array(new SharedArrayBuffer(4d));
        var result = Atomics.waitAsync(view, 0d, 0d, 10d);
        Assert.True((bool)ObjectRuntime.GetProperty(result, "async")!);
        Assert.Equal(1, scheduler.PendingIoCount);
        Assert.Equal(1, cluster.SharedServices.Atomics.WaiterCount);
        Assert.True(SpinWait.SpinUntil(
            () => cluster.SharedServices.Atomics.WaiterCount == 0,
            TimeSpan.FromSeconds(5)));
        Assert.Equal(0d, Atomics.notify(view, 0d, null));
        Action? job = null;
        Assert.True(SpinWait.SpinUntil(
            () => scheduler.TryDequeueMicrotask(out job),
            TimeSpan.FromSeconds(5)));
        job!();
        Assert.Equal("timed-out", JavaScriptRuntime.Promise.AwaitValue(
            ObjectRuntime.GetProperty(result, "value")));
        Assert.Equal(0, scheduler.PendingIoCount);
    }

    [Fact]
    public void AnotherAgentNotifiesWaitingAgentsPromise()
    {
        using var cluster = new RuntimeAgentCluster();
        var waitingAgent = cluster.CreateAgent();
        var notifyingAgent = cluster.CreateAgent();
        var waitingRealm = waitingAgent.CreateRealm();
        var notifyingRealm = notifyingAgent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(waitingRealm.Services);
        RuntimeServices.ConfigureServiceProvider(notifyingRealm.Services);
        var waitingContext = RuntimeExecutionContext.GetOrCreate(waitingRealm.Services);
        var notifyingContext = RuntimeExecutionContext.GetOrCreate(notifyingRealm.Services);
        var scheduler = (NodeSchedulerState)waitingRealm.Services.Resolve<IMicrotaskScheduler>();

        SharedArrayBuffer buffer;
        object result;
        using (waitingContext.EnterAsRoot())
        {
            buffer = new SharedArrayBuffer(8d);
            result = Atomics.waitAsync(new BigInt64Array(buffer), 0d, BigInteger.Zero, null);
        }

        using (notifyingContext.EnterAsRoot())
        {
            var wrapper = buffer.CreateWrapperForCurrentRealm();
            Assert.Equal(1d, Atomics.notify(new BigInt64Array(wrapper), 0d, 1d));
        }

        using (waitingContext.EnterAsRoot())
        {
            Assert.True(scheduler.TryDequeueMicrotask(out var job));
            job!();
            Assert.Equal("ok", JavaScriptRuntime.Promise.AwaitValue(
                ObjectRuntime.GetProperty(result, "value")));
        }
    }

    [Fact]
    public void DisposingAgentCancelsInfiniteWaitAndReleasesPendingIo()
    {
        using var cluster = new RuntimeAgentCluster();
        var agent = cluster.CreateAgent();
        var realm = agent.CreateRealm();
        RuntimeServices.ConfigureServiceProvider(realm.Services);
        var scheduler = (NodeSchedulerState)realm.Services.Resolve<IMicrotaskScheduler>();
        using (RuntimeExecutionContext.GetOrCreate(realm.Services).EnterAsRoot())
        {
            var view = new Int32Array(new SharedArrayBuffer(4d));
            Atomics.waitAsync(view, 0d, 0d, null);
            Assert.Equal(1, cluster.SharedServices.Atomics.WaiterCount);
            Assert.Equal(1, scheduler.PendingIoCount);
        }

        agent.Dispose();
        Assert.Equal(0, cluster.SharedServices.Atomics.WaiterCount);
        Assert.Equal(0, scheduler.PendingIoCount);
    }
}
