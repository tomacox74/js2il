using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Js2IL.Runtime;

internal static class JsPromiseTaskInterop
{
    internal static Task ToTask(JsRuntimeInstance runtime, Promise promise)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

        // IMPORTANT: Promise.then() queues microtasks via GlobalThis.ServiceProvider.
        // That provider is only configured on the runtime's dedicated script thread.
        // Subscribe to the promise on that thread so async/await works for hosts.
        runtime.Invoke(() =>
        {
            promise.then(
                onFulfilled: new Func<object[]?, object?, object?>((_, _) =>
                {
                    tcs.TrySetResult(null);
                    return null;
                }),
                onRejected: new Func<object[]?, object?, object?>((_, reason) =>
                {
                    tcs.TrySetException(ToException(reason));
                    return null;
                }));

            return (object?)null;
        });

        return tcs.Task;
    }

    internal static Task<T> ToTask<T>(JsRuntimeInstance runtime, Promise promise)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

        // See note above: ensure promise wiring happens on the script thread.
        runtime.Invoke(() =>
        {
            promise.then(
                onFulfilled: new Func<object[]?, object?, object?>((_, value) =>
                {
                    try
                    {
                        var converted = JsReturnConverter.ConvertReturn(runtime, value, typeof(T));
                        tcs.TrySetResult((T)converted!);
                    }
                    catch (Exception ex)
                    {
                        tcs.TrySetException(ex);
                    }

                    return null;
                }),
                onRejected: new Func<object[]?, object?, object?>((_, reason) =>
                {
                    tcs.TrySetException(ToException(reason));
                    return null;
                }));

            return (object?)null;
        });

        return tcs.Task;
    }

    private static Exception ToException(object? reason)
    {
        if (reason is Exception ex)
        {
            return ex;
        }

        return new JsThrownValueException(reason);
    }
}
