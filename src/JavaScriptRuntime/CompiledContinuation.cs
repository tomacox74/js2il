namespace JavaScriptRuntime;

/// <summary>
/// Internal runtime representation of a generated async or generator step.
/// This object is resumable infrastructure only and is never a JavaScript callable value.
/// </summary>
public sealed class CompiledContinuation
{
    private readonly Delegate _target;
    private readonly Closure.DelegateInvokeMetadata _invokeMetadata;
    private readonly object[] _scopes;
    private readonly object?[] _arguments;

    private CompiledContinuation(
        Delegate target,
        object[] scopes,
        object?[] arguments)
    {
        _target = target;
        _invokeMetadata = Closure.GetDelegateInvokeMetadata(target);
        _scopes = scopes;
        _arguments = arguments;
    }

    internal object[] Scopes => _scopes;

    public static CompiledContinuation Create(
        object target,
        object[] scopes,
        object?[] arguments)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentNullException.ThrowIfNull(scopes);
        ArgumentNullException.ThrowIfNull(arguments);
        if (target is not Delegate continuationTarget)
        {
            throw new ArgumentException(
                "Expected a generated continuation delegate.",
                nameof(target));
        }

        return new CompiledContinuation(
            continuationTarget,
            scopes,
            arguments);
    }

    public object? Resume()
        => Closure.InvokeContinuationTarget(
            _target,
            _invokeMetadata,
            _scopes,
            _arguments);
}
