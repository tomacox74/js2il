namespace JavaScriptRuntime;

/// <summary>
/// Transitional adapter for delegate-backed JavaScript functions.
/// </summary>
public sealed class LegacyDelegateFunctionAdapter : JsFunctionObject
{
    private readonly object[] _scopes;

    public LegacyDelegateFunctionAdapter(Delegate target, object[]? scopes = null)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        _scopes = scopes ?? RuntimeServices.EmptyScopes;
    }

    public Delegate Target { get; }

    public override bool IsConstructor => ObjectRuntime.IsConstructibleValue(Target);

    protected override object? CallCore(object? thisArgument, object?[] arguments)
        => Invoke(Target, _scopes, thisArgument, arguments);

    protected override object? ConstructCore(object?[] arguments, object? newTarget)
        => Function.Construct(Target, arguments, newTarget);

    internal static object? Invoke(
        Delegate target,
        object[] scopes,
        object? thisArgument,
        object?[] arguments)
    {
        var effectiveThis = Function.GetEffectiveThisArg(target, thisArgument);
        var previousThis = RuntimeServices.SetCurrentThis(effectiveThis);
        try
        {
            return Closure.InvokeWithArgs(target, scopes, arguments);
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }
}
