namespace JavaScriptRuntime;

public partial class RuntimeServices
{
    internal sealed record PreparedDynamicFunction(object Factory, object[] Sources);

    internal static bool TryCreatePreparedDynamicFunction(object?[] arguments, out object? function)
    {
        function = null;
        if (_currentInvocation.Value?.PreparedFunction is not { } prepared
            || prepared.Sources.Length != arguments.Length)
            return false;
        for (var i = 0; i < arguments.Length; i++)
        {
            if (!string.Equals(DotNet2JSConversions.ToString(arguments[i]), prepared.Sources[i] as string, StringComparison.Ordinal))
                return false;
        }
        function = CallableOperations.Call0(prepared.Factory, null);
        Function.DefineMetadataProperty(function!, "name", "anonymous");
        return true;
    }

    public static object? GetPreparedDynamicFunctionConstructor(object receiver)
        => ObjectRuntime.GetItem(receiver, "Function");

    public static object? ConstructPreparedDynamicFunction(
        object? constructor,
        object? receiver,
        object?[] arguments,
        object? factory,
        object[] sources,
        object? syntaxError)
        => CreatePreparedDynamicFunction(constructor, receiver, arguments, factory, sources, syntaxError, construct: true);

    public static object? CallPreparedDynamicFunction(
        object? constructor,
        object? receiver,
        object?[] arguments,
        object? factory,
        object[] sources,
        object? syntaxError)
        => CreatePreparedDynamicFunction(constructor, receiver, arguments, factory, sources, syntaxError, construct: false);

    private static object? CreatePreparedDynamicFunction(
        object? constructor,
        object? receiver,
        object?[] arguments,
        object? factory,
        object[] sources,
        object? syntaxError,
        bool construct)
    {
        if (constructor is JsClassConstructorObject && factory is not null)
        {
            var previous = _currentInvocation.Value;
            PublishInvocationFrame((previous ?? new InvocationFrame()) with
            {
                PreparedFunction = new PreparedDynamicFunction(factory, sources)
            });
            try
            {
                return CallableOperations.Construct(constructor, arguments, constructor);
            }
            finally
            {
                PublishInvocationFrame(previous);
            }
        }
        if (constructor is not BuiltinDelegateFunctionAdapter adapter
            || !adapter.Target.Equals(GlobalThis.Function))
        {
            return construct
                ? CallableOperations.Construct(constructor, arguments, constructor)
                : CallableOperations.Call(constructor, receiver, arguments);
        }

        using var realmScope = EnterFunctionRealm(adapter);
        var convertedArguments = new object?[arguments.Length];
        var matches = arguments.Length == sources.Length;
        for (var index = 0; index < arguments.Length; index++)
        {
            var source = DotNet2JSConversions.ToString(arguments[index]);
            convertedArguments[index] = source;
            matches &= index < sources.Length && string.Equals(source, (string)sources[index], StringComparison.Ordinal);
        }
        if (!matches)
        {
            // Only the intrinsic consumes coerced strings; replacement constructors receive the originals.
            return construct
                ? CallableOperations.Construct(constructor, convertedArguments, constructor)
                : CallableOperations.Call(constructor, receiver, convertedArguments);
        }

        if (syntaxError is string errorMessage)
        {
            throw new SyntaxError(errorMessage);
        }
        // This compiler-generated thunk only allocates the prepared callable. It has no user body.
        var function = ((JsFunctionObject)factory!).InvokeCall(null, JsCallArguments.Empty)!;
        Function.DefineMetadataProperty(function, "name", "anonymous");
        return function;
    }
}
