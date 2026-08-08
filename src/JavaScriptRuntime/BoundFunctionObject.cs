namespace JavaScriptRuntime;

/// <summary>
/// ECMAScript bound-function exotic object.
/// </summary>
internal sealed class BoundFunctionObject : JsFunctionObject
{
    private readonly object _target;
    private readonly object? _boundThis;
    private readonly object?[] _boundArguments;

    public BoundFunctionObject(
        object target,
        object? boundThis,
        object?[] boundArguments)
    {
        _target = target;
        _boundThis = boundThis;
        _boundArguments = boundArguments;
    }

    public override bool IsConstructor => CallableOperations.IsConstructor(_target);

    public override bool RequiresInvocationContext => false;

    internal object Target => _target;

    protected override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        var combined = JsCallArguments.Prepend(_boundArguments, arguments);
        return CallableOperations.Call(
            _target,
            _boundThis,
            combined);
    }

    protected override object? ConstructCore(
        in JsCallArguments arguments,
        object? newTarget)
    {
        var effectiveNewTarget = ReferenceEquals(newTarget, this)
            ? _target
            : newTarget;
        var combined = JsCallArguments.Prepend(_boundArguments, arguments);
        return CallableOperations.Construct(
            _target,
            combined,
            effectiveNewTarget);
    }
}
