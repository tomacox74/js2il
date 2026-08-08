namespace JavaScriptRuntime;

/// <summary>
/// Base class for JavaScript async function values implemented as runtime objects.
/// </summary>
public abstract class JsAsyncFunctionObject : JsFunctionObject
{
    protected sealed override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        try
        {
            return CallCoreAsync(thisArgument, arguments);
        }
        catch (Exception exception)
        {
            var reason = exception is JsThrownValueException thrown
                ? thrown.Value
                : exception;
            return (Promise)Promise.reject(reason)!;
        }
    }

    /// <summary>
    /// Invokes the compiled async body and returns its JavaScript Promise.
    /// </summary>
    protected abstract Promise CallCoreAsync(
        object? thisArgument,
        in JsCallArguments arguments);
}
