namespace Jroc.Runtime;

/// <summary>
/// Callback used by an explicitly adapted CLR host function.
/// </summary>
public delegate object? JsHostFunctionCallback(
    object? receiver,
    object?[] arguments);

/// <summary>
/// Constructor callback used by an explicitly constructable CLR host function.
/// </summary>
public delegate object? JsHostConstructorCallback(
    object?[] arguments,
    object? newTarget);

/// <summary>
/// Describes a CLR callback that should enter JavaScript as a function object.
/// </summary>
public sealed class JsHostFunction
{
    private readonly JsHostFunctionCallback _callback;
    private readonly JsHostConstructorCallback? _constructor;

    public JsHostFunction(
        JsHostFunctionCallback callback,
        string? name = null,
        double length = 0,
        JsHostConstructorCallback? constructor = null)
    {
        ArgumentNullException.ThrowIfNull(callback);
        if (double.IsNaN(length) || double.IsInfinity(length) || length < 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(length),
                "Function length must be a finite, non-negative number.");
        }

        _callback = callback;
        _constructor = constructor;
        Name = name ?? callback.Method.Name;
        Length = Math.Truncate(length);
    }

    public string Name { get; }

    public double Length { get; }

    public bool IsConstructor => _constructor != null;

    internal object? Invoke(object? receiver, object?[] arguments)
        => _callback(receiver, arguments);

    internal object? Construct(object?[] arguments, object? newTarget)
        => (_constructor
            ?? throw new InvalidOperationException("The host function is not constructable."))(
                arguments,
                newTarget);
}
