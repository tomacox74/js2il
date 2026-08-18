namespace JavaScriptRuntime;

public readonly struct GeneratedInvocationContext
{
    private readonly object? _thisArgument;
    private readonly JsCallArguments _arguments;
    private readonly object? _callee;
    private readonly object? _newTarget;

    private GeneratedInvocationContext(
        InvocationContextRequirements requirements,
        object? thisArgument,
        in JsCallArguments arguments,
        object? callee,
        object? newTarget)
    {
        _thisArgument = (requirements & InvocationContextRequirements.This) != 0
            ? thisArgument
            : null;
        _arguments = (requirements & InvocationContextRequirements.Arguments) != 0
            ? arguments
            : default;
        _callee = (requirements & InvocationContextRequirements.Callee) != 0
            ? callee
            : null;
        _newTarget = (requirements & InvocationContextRequirements.NewTarget) != 0
            ? newTarget
            : null;
    }

    public static GeneratedInvocationContext Create(
        int requirements,
        object? thisArgument,
        JsCallArguments arguments,
        object? callee,
        object? newTarget)
        => new(
            (InvocationContextRequirements)requirements,
            thisArgument,
            arguments,
            callee,
            newTarget);

    public static GeneratedInvocationContext CreateFromArray(
        int requirements,
        object? thisArgument,
        object?[]? arguments,
        object? callee,
        object? newTarget)
    {
        var callArguments = JsCallArguments.FromArray(arguments);
        return new GeneratedInvocationContext(
            (InvocationContextRequirements)requirements,
            thisArgument,
            callArguments,
            callee,
            newTarget);
    }

    public static object? GetThis(GeneratedInvocationContext context)
        => context._thisArgument;

    public static object? GetCallee(GeneratedInvocationContext context)
        => context._callee;

    public static object? GetNewTarget(GeneratedInvocationContext context)
        => context._newTarget;

    public static object?[] GetArguments(GeneratedInvocationContext context)
        => context._arguments.ToArray();

    public static int GetArgumentCount(GeneratedInvocationContext context)
        => context._arguments.Count;

    public static object? GetArgument(
        GeneratedInvocationContext context,
        int index)
        => context._arguments.GetArgument(index);
}
