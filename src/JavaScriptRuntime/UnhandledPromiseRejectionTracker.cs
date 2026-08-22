using System.Runtime.CompilerServices;

namespace JavaScriptRuntime;

internal interface IUnhandledPromiseRejectionEnvironment
{
}

internal sealed class UnhandledPromiseRejectionTracker
{
    private readonly bool _enabled;
    private readonly Dictionary<Promise, object?> _rejections =
        new(ReferenceEqualityComparer.Instance);

    public UnhandledPromiseRejectionTracker(IEnvironment environment)
    {
        _enabled = environment is IUnhandledPromiseRejectionEnvironment;
    }

    internal void Track(Promise promise, object? reason)
    {
        if (_enabled)
        {
            _rejections[promise] = reason;
        }
    }

    internal void MarkHandled(Promise promise)
    {
        if (_enabled)
        {
            _rejections.Remove(promise);
        }
    }

    internal void ThrowIfUnhandled()
    {
        if (_rejections.Count == 0)
        {
            return;
        }

        var reason = _rejections.Values.First();
        throw reason switch
        {
            Exception exception => exception,
            _ => new JsThrownValueException(reason)
        };
    }
}
