namespace JavaScriptRuntime;

/// <summary>
/// Base class for JavaScript function values implemented as runtime objects.
/// </summary>
public abstract class JsFunctionObject : JsObject
{
    protected JsFunctionObject()
    {
        PrototypeChain.InitializePrototype(this, Function.Prototype);
    }

    /// <summary>
    /// Gets whether this function implements ECMAScript [[Construct]].
    /// </summary>
    public virtual bool IsConstructor => false;

    /// <summary>
    /// Gets whether invocation must populate ambient runtime call state.
    /// </summary>
    public virtual bool RequiresInvocationContext => true;

    internal object? InvokeCall(object? thisArgument, in JsCallArguments arguments)
        => CallCore(thisArgument, arguments);

    internal object? InvokeConstruct(in JsCallArguments arguments, object? newTarget)
        => ConstructCore(arguments, newTarget);

    internal object? ResolveThisArgument(object? thisArgument)
        => ResolveThisArgumentCore(thisArgument);

    internal object? ResolveCallNewTarget()
        => ResolveCallNewTargetCore();

    internal object? GetLexicalSuperReceiver()
        => GetLexicalSuperReceiverCore();

    internal object[]? GetLexicalSuperScopes()
        => GetLexicalSuperScopesCore();

    /// <summary>
    /// Implements ECMAScript [[Call]] for this function object.
    /// </summary>
    protected abstract object? CallCore(object? thisArgument, in JsCallArguments arguments);

    protected virtual object? ResolveThisArgumentCore(object? thisArgument)
        => thisArgument;

    protected virtual object? ResolveCallNewTargetCore()
        => null;

    protected virtual object? GetLexicalSuperReceiverCore()
        => null;

    protected virtual object[]? GetLexicalSuperScopesCore()
        => null;

    /// <summary>
    /// Implements ECMAScript [[Construct]] for constructable function objects.
    /// </summary>
    protected virtual object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => throw new TypeError("Value is not a constructor");
}
