using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using JavaScriptRuntime;
using Jroc;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// A fixture owns one agent cluster and its workers; never share a report queue across fixtures.
internal sealed class Test262AgentHelpers : IDisposable
{
    private const int DeadlineMs = 15000;
    private readonly ConcurrentQueue<string> _reports = new();
    private readonly List<Thread> _workers = [];
    private readonly object _gate = new();
    private RuntimeAgentCluster? _cluster;
    private SharedArrayBuffer? _broadcast;
    private Exception? _failure;
    private bool _disposed;

    internal object CreateAgentObject(bool atomicsHelper)
    {
        var agent = new JsObject();
        ObjectRuntime.SetItem(agent, "start", CreateFunction((Action<object?>)Start, "start", 1));
        ObjectRuntime.SetItem(agent, "broadcast", CreateFunction((Action<object?>)Broadcast, "broadcast", 1));
        ObjectRuntime.SetItem(agent, "receiveBroadcast", CreateFunction(
            (Action<object?>)ReceiveBroadcast, "receiveBroadcast", 1));
        ObjectRuntime.SetItem(agent, "report", CreateFunction((Action<object?>)Report, "report", 1));
        ObjectRuntime.SetItem(agent, "getReport", CreateFunction(
            (Func<object?>)(atomicsHelper ? GetReport : GetReportNow), "getReport", 0));
        ObjectRuntime.SetItem(agent, "getReportAsync", CreateFunction(
            (Func<object>)GetReportAsync, "getReportAsync", 0));
        ObjectRuntime.SetItem(agent, "safeBroadcast", CreateFunction(
            (Action<object?>)SafeBroadcast, "safeBroadcast", 1));
        ObjectRuntime.SetItem(agent, "safeBroadcastAsync", CreateFunction(
            (Func<object?, object?, object?, object>)SafeBroadcastAsync, "safeBroadcastAsync", 3));
        ObjectRuntime.SetItem(agent, "waitUntil", CreateFunction(
            (Action<object?, object?, object?>)WaitUntil, "waitUntil", 3));
        ObjectRuntime.SetItem(agent, "sleep", CreateFunction((Action<object?>)Sleep, "sleep", 1));
        ObjectRuntime.SetItem(agent, "trySleep", CreateFunction((Action<object?>)Sleep, "trySleep", 1));
        ObjectRuntime.SetItem(agent, "tryYield", CreateFunction((Action)TryYield, "tryYield", 0));
        ObjectRuntime.SetItem(agent, "leaving", CreateFunction((Action)(() => { }), "leaving", 0));
        ObjectRuntime.SetItem(agent, "monotonicNow", CreateFunction(
            (Func<double>)(() => Stopwatch.GetTimestamp() * 1000d / Stopwatch.Frequency), "monotonicNow", 0));
        ObjectRuntime.SetItem(agent, "setTimeout", CreateFunction(
            (Func<object?, object?, object>)((callback, delay) =>
                GlobalThis.setTimeout(callback!, delay!)), "setTimeout", 2));
        var timeouts = new JsObject();
        ObjectRuntime.SetItem(timeouts, "yield", 100d);
        ObjectRuntime.SetItem(timeouts, "small", 200d);
        ObjectRuntime.SetItem(timeouts, "long", 1000d);
        ObjectRuntime.SetItem(timeouts, "huge", 10000d);
        ObjectRuntime.SetItem(agent, "timeouts", timeouts);
        return agent;
    }

    private void Start(object? source)
    {
        var context = RuntimeExecutionContext.CurrentOrOverride
            ?? throw new InvalidOperationException("$262.agent.start requires an active runtime.");
        var cluster = context.Agent.Cluster;
        lock (_gate)
        {
            if (_cluster != null && !ReferenceEquals(_cluster, cluster))
                throw new InvalidOperationException("Test262 workers cannot cross agent clusters.");
            _cluster = cluster;
        }

        var text = DotNet2JSConversions.ToString(source);
        var path = Path.Combine(Directory.GetCurrentDirectory(), $"__test262_agent_{Guid.NewGuid():N}.js");
        var files = new MockFileSystem();
        files.AddFile(path, text);
        var descriptors = Test262HostRuntimeIntrinsics.Create(null, new Test262AsyncCompletion(), this);
        var artifact = JrocInMemoryCompiler.Compile(new JrocInMemoryCompileRequest(path)
        {
            SourceText = text,
            FileSystem = files,
            HostRuntimeIntrinsics = descriptors,
            AssumeUnmodifiedHostGlobals = true
        });
        var loaded = JrocInMemoryAssemblyLoader.Load(artifact);
        var entry = (Action<string[]>)Delegate.CreateDelegate(
            typeof(Action<string[]>), loaded.Assembly.EntryPoint
                ?? throw new InvalidOperationException("Worker has no entry point."));
        var worker = new Thread(() =>
        {
            try
            {
                using (loaded)
                using (var lifecycle = RuntimeLifecycle.Create(
                    loaded.Assembly, isHostedExecution: true, cluster: cluster,
                    configureServices: services => services.Replace(descriptors),
                    suppressInheritedExecutionContext: true))
                {
                    Engine._serviceProviderOverride.Value = lifecycle.Services;
                    try
                    {
                        entry(System.Array.Empty<string>());
                    }
                    finally
                    {
                        Engine._serviceProviderOverride.Value = null;
                    }
                }
            }
            catch (Exception error)
            {
                lock (_gate)
                    _failure ??= error;
            }
        }) { IsBackground = true, Name = "Test262 agent" };
        lock (_gate)
            _workers.Add(worker);
        worker.Start();
    }

    private void Broadcast(object? value)
    {
        if (value is not SharedArrayBuffer buffer)
            throw new TypeError("$262.agent.broadcast requires a SharedArrayBuffer");
        lock (_gate)
            _broadcast = buffer;
    }

    private void ReceiveBroadcast(object? callback)
    {
        if (!CallableOperations.IsCallable(callback))
            throw new TypeError("$262.agent.receiveBroadcast requires a callback");
        var deadline = Stopwatch.StartNew();
        SharedArrayBuffer buffer;
        while (true)
        {
            lock (_gate)
            {
                if (_broadcast is not null)
                {
                    buffer = _broadcast;
                    break;
                }
            }
            CheckFailureAndDeadline(deadline, "receiveBroadcast");
            Thread.Sleep(5);
        }
        Invoke(callback, buffer.CreateWrapperForCurrentRealm());
    }

    private void Report(object? value)
    {
        _reports.Enqueue(DotNet2JSConversions.ToString(value));
    }

    private object? GetReport()
    {
        var deadline = Stopwatch.StartNew();
        while (true)
        {
            if (_reports.TryDequeue(out var report))
                return report;
            CheckFailureAndDeadline(deadline, "getReport");
            Thread.Sleep(5);
        }
    }

    private object? GetReportNow()
    {
        CheckFailureAndDeadline(Stopwatch.StartNew(), "getReport");
        return _reports.TryDequeue(out var report) ? report : JsNull.Null;
    }

    private object GetReportAsync()
        => new Promise(CreateFunction((Action<object?, object?>)((resolve, reject) =>
        {
            var deadline = Stopwatch.StartNew();
            void Poll()
            {
                if (_reports.TryDequeue(out var report))
                {
                    Invoke(resolve, report);
                    return;
                }
                try
                {
                    CheckFailureAndDeadline(deadline, "getReportAsync");
                }
                catch (Exception error)
                {
                    Invoke(reject, error);
                    return;
                }
                GlobalThis.setTimeout(CreateFunction((Action)Poll, "", 0), 10d);
            }
            Poll();
        }), "", 2));

    private void SafeBroadcast(object? typedArray)
    {
        if (typedArray is not Int32Array and not BigInt64Array)
            throw CreateTest262Error("Only Int32Array and BigInt64Array can be used as shared waitable typed arrays.");
        var buffer = ObjectRuntime.GetItem(typedArray!, "buffer");
        Broadcast(buffer);
    }

    private object SafeBroadcastAsync(object? typedArray, object? index, object? expected)
    {
        SafeBroadcast(typedArray);
        WaitUntil(typedArray, index, expected);
        TryYield();
        return Promise.resolve(Atomics.load(typedArray!, index!))!;
    }

    private void WaitUntil(object? typedArray, object? index, object? expected)
    {
        var deadline = Stopwatch.StartNew();
        while (!Operators.StrictEqual(Atomics.load(typedArray, index), expected))
        {
            CheckFailureAndDeadline(deadline, "waitUntil");
            Thread.Sleep(1);
        }
    }

    private static void Sleep(object? milliseconds)
    {
        var number = TypeUtilities.ToNumber(milliseconds);
        if (double.IsNaN(number) || number <= 0)
            return;
        Thread.Sleep((int)System.Math.Min(number, DeadlineMs));
    }

    private static void TryYield() => Thread.Sleep(100);

    private void CheckFailureAndDeadline(Stopwatch elapsed, string operation)
    {
        lock (_gate)
        {
            if (_failure is not null)
                throw new InvalidOperationException($"Test262 agent failed during {operation}.", _failure);
            if (_disposed)
                throw new InvalidOperationException($"Test262 agent stopped during {operation}.");
        }
        if (elapsed.ElapsedMilliseconds > DeadlineMs)
            throw new TimeoutException($"Test262 agent {operation} timed out after {DeadlineMs}ms.");
    }

    public void Dispose()
    {
        lock (_gate)
            _disposed = true;
        Thread[] workers;
        lock (_gate)
            workers = _workers.ToArray();
        var deadline = Stopwatch.StartNew();
        foreach (var worker in workers)
            worker.Join(System.Math.Max(0, 2000 - (int)deadline.ElapsedMilliseconds));
        if (_failure is not null)
            throw new InvalidOperationException("Test262 worker failed.", _failure);
        if (workers.Any(worker => worker.IsAlive))
            throw new TimeoutException("Test262 worker did not leave before harness cleanup.");
    }
}
