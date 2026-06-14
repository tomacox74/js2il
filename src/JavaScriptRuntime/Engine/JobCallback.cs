namespace JavaScriptRuntime.EngineCore;

/// <summary>
/// ECMA-262 §9.5.1 JobCallback Records.
///
/// In the spec, a JobCallback record has fields:
/// - [[Callback]]
/// - [[HostDefined]]
///
/// JROC currently models Jobs as microtasks (queued Actions). This record provides a
/// stable place to carry host-defined context alongside callbacks.
/// </summary>
internal readonly record struct JobCallbackRecord(Action Callback, object? HostDefined);

/// <summary>
/// ECMA-262 §9.5.2–§9.5.3 host operations for JobCallback.
///
/// JROC is a non-browser host, so we use the default behavior.
/// </summary>
internal static class HostJobCallbacks
{
    internal static JobCallbackRecord HostMakeJobCallback(Action callback)
    {
        ArgumentNullException.ThrowIfNull(callback);
        return new JobCallbackRecord(callback, HostDefined: null);
    }

    internal static void HostCallJobCallback(in JobCallbackRecord jobCallback, object? v, object?[]? argumentsList)
    {
        // Default host behavior: call the callback. JROC does not currently model
        // realms/agent state, so HostDefined is unused today.
        jobCallback.Callback();
    }
}
