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

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => Invoke(Target, _scopes, thisArgument, arguments.ToArray());

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => Function.Construct(Target, arguments.ToArray(), newTarget);

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
