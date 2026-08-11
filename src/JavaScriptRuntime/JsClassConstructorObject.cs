namespace JavaScriptRuntime;

/// <summary>
/// Shared runtime behavior for generated ECMAScript class constructor objects.
/// </summary>
public abstract class JsClassConstructorObject : JsFunctionObject
{
    private Type? _type;
    private object[]? _scopes;

    public override bool IsConstructor => true;

    public Type Type => _type
        ?? throw new InvalidOperationException(
            "Class constructor object has not been initialized.");

    public object[] Scopes => _scopes
        ?? throw new InvalidOperationException(
            "Class constructor object has not been initialized.");

    public int FormalParameterCount { get; private set; }

    internal void Initialize(
        Type type,
        object[] scopes,
        int formalParameterCount)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(scopes);

        if (_type is not null)
        {
            if (!ReferenceEquals(_type, type)
                || !ReferenceEquals(_scopes, scopes)
                || FormalParameterCount != formalParameterCount)
            {
                throw new InvalidOperationException(
                    "Class constructor object was initialized more than once.");
            }

            return;
        }

        _type = type;
        _scopes = scopes;
        FormalParameterCount = formalParameterCount;
    }

    protected sealed override object? CallCore(
        object? thisArgument,
        in JsCallArguments arguments)
        => throw new TypeError(
            "Class constructor cannot be invoked without 'new'");

    protected sealed override object? ConstructCore(
        in JsCallArguments arguments,
        object? newTarget)
        => ObjectRuntime.ConstructValue(
            this,
            System.Array.ConvertAll(
                arguments.ToArray(),
                static argument => argument!),
            newTarget);
}
