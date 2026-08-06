namespace JavaScriptRuntime;

/// <summary>
/// Transitional bound-function representation used while callable families migrate.
/// </summary>
internal sealed class BoundFunctionObject : JsFunctionObject
{
    private readonly JsFunctionObject _target;
    private readonly object? _boundThis;
    private readonly object?[] _boundArguments;

    public BoundFunctionObject(
        JsFunctionObject target,
        object? boundThis,
        object?[] boundArguments)
    {
        _target = target;
        _boundThis = boundThis;
        _boundArguments = boundArguments;
    }

    public override bool IsConstructor => _target.IsConstructor;

    public override bool RequiresInvocationContext => false;

    protected override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
    {
        return CallableOperations.Call(
            _target,
            _boundThis,
            Combine(arguments));
    }

    protected override object? ConstructCore(
        in JsCallArguments arguments,
        object? newTarget)
    {
        var effectiveNewTarget = ReferenceEquals(newTarget, this)
            ? _target
            : newTarget;
        return CallableOperations.Construct(
            _target,
            Combine(arguments),
            effectiveNewTarget);
    }

    private object?[] Combine(in JsCallArguments arguments)
    {
        if (_boundArguments.Length == 0)
        {
            return arguments.ToArray();
        }

        var result = new object?[_boundArguments.Length + arguments.Count];
        System.Array.Copy(_boundArguments, result, _boundArguments.Length);
        for (var index = 0; index < arguments.Count; index++)
        {
            result[_boundArguments.Length + index] = arguments.GetArgument(index);
        }
        return result;
    }
}
