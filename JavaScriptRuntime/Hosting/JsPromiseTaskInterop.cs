using System.Threading.Tasks;
using JavaScriptRuntime;

namespace Js2IL.Runtime;

internal static class JsPromiseTaskInterop
{
    internal static Task ToTask(Promise promise)
    {
        ArgumentNullException.ThrowIfNull(promise);

        var tcs = new TaskCompletionSource<object?>(TaskCreationOptions.RunContinuationsAsynchronously);

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

        return tcs.Task;
    }

    internal static Task<T> ToTask<T>(JsRuntimeInstance runtime, Promise promise)
    {
        ArgumentNullException.ThrowIfNull(runtime);
        ArgumentNullException.ThrowIfNull(promise);

        var tcs = new TaskCompletionSource<T>(TaskCreationOptions.RunContinuationsAsynchronously);

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
