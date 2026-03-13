using System.Collections.Generic;

namespace JavaScriptRuntime.EngineCore;

internal sealed class FinalizationRegistryHost : IFinalizationRegistryHost
{
    private readonly object _sync = new();
    private readonly ICleanupJobScheduler _cleanupScheduler;
    private readonly List<WeakReference<JavaScriptRuntime.FinalizationRegistry>> _registries = new();
    private readonly List<object> _keptObjects = new();

    public FinalizationRegistryHost(ICleanupJobScheduler cleanupScheduler)
    {
        ArgumentNullException.ThrowIfNull(cleanupScheduler);
        _cleanupScheduler = cleanupScheduler;
    }

    public void TrackRegistry(JavaScriptRuntime.FinalizationRegistry registry)
    {
        ArgumentNullException.ThrowIfNull(registry);

        lock (_sync)
        {
            for (int i = _registries.Count - 1; i >= 0; i--)
            {
                if (!_registries[i].TryGetTarget(out var existing))
                {
                    _registries.RemoveAt(i);
                    continue;
                }

                if (ReferenceEquals(existing, registry))
                {
                    return;
                }
            }

            _registries.Add(new WeakReference<JavaScriptRuntime.FinalizationRegistry>(registry));
        }
    }

    public void AddToKeptObjects(object target)
    {
        ArgumentNullException.ThrowIfNull(target);

        lock (_sync)
        {
            _keptObjects.Add(target);
        }
    }

    public void ClearKeptObjects()
    {
        lock (_sync)
        {
            _keptObjects.Clear();
        }
    }

    public void CollectAndQueueCleanupJobs(bool forceCollection)
    {
        if (forceCollection)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
        }

        var jobs = new List<(JavaScriptRuntime.FinalizationRegistry Registry, object? HeldValue)>();

        lock (_sync)
        {
            for (int i = _registries.Count - 1; i >= 0; i--)
            {
                if (!_registries[i].TryGetTarget(out var registry))
                {
                    _registries.RemoveAt(i);
                    continue;
                }

                registry.CollectCleanupJobs(jobs);
            }
        }

        foreach (var job in jobs)
        {
            var registry = job.Registry;
            var heldValue = job.HeldValue;
            var jobCallback = HostJobCallbacks.HostMakeJobCallback(() => registry.InvokeCleanupCallback(heldValue));

            _cleanupScheduler.QueueCleanupJob(() =>
            {
                HostJobCallbacks.HostCallJobCallback(jobCallback, v: null, argumentsList: System.Array.Empty<object?>());
            });
        }
    }
}
