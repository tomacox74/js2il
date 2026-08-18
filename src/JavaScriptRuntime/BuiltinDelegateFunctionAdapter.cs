namespace JavaScriptRuntime;

/// <summary>
/// Explicit adapter for runtime-owned built-in CLR delegates.
/// Compiled JavaScript functions never use this representation.
/// </summary>
/// <remarks>
/// The adapter is the JavaScript-visible identity of a built-in function, so it is
/// realm-owned (issue #1824): <see cref="FromDelegate"/> resolves the stable adapter
/// from <see cref="RuntimeIntrinsics.BuiltinAdapters"/> of the ambient realm. Only the
/// immutable CLR metadata behind it (the static delegate, its method handle and its
/// <see cref="Closure.DelegateInvokeMetadata"/>) stays process-wide.
/// </remarks>
public sealed class BuiltinDelegateFunctionAdapter : JsFunctionObject
{
    private readonly object[] _scopes;
    private readonly Closure.DelegateInvokeMetadata _invokeMetadata;
    private readonly object _initializationLock = new();
    private bool _isConstructor;
    private bool _requiresInvocationContext;

    public BuiltinDelegateFunctionAdapter(
        Delegate target,
        object[]? scopes = null,
        bool isConstructor = false)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        _scopes = scopes ?? RuntimeServices.EmptyScopes;
        _invokeMetadata = Closure.GetDelegateInvokeMetadata(target);
        _isConstructor = isConstructor;
        _requiresInvocationContext =
            isConstructor
            || !BuiltinFunctionDelegates.IsReceiverAware(target);
        using (PropertyDescriptorStore.BeginIntrinsicInitialization())
        {
            Function.DefineMetadataProperty(
                this,
                "length",
                Function.GetLength(target));
            Function.DefineMetadataProperty(
                this,
                "name",
                Function.GetName(target));
        }
    }

    public Delegate Target { get; }

    internal object[] Scopes => _scopes;

    internal Closure.DelegateInvokeMetadata InvokeMetadata
        => _invokeMetadata;

    internal object InitializationLock => _initializationLock;

    public override bool IsConstructor => _isConstructor;

    public override bool RequiresInvocationContext => _requiresInvocationContext;

    public static BuiltinDelegateFunctionAdapter FromDelegate(Delegate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return GetOrCreateStableAdapter(target);
    }

    private static BuiltinDelegateFunctionAdapter GetOrCreateStableAdapter(
        Delegate target)
    {
        if (target.GetInvocationList().Length != 1)
        {
            return new BuiltinDelegateFunctionAdapter(target);
        }

        return RuntimeIntrinsics.Current.BuiltinAdapters.GetOrAdd(
            target,
            static resolved => new BuiltinDelegateFunctionAdapter(resolved));
    }

    internal static bool HasStableAdapterForTests(Delegate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return target.GetInvocationList().Length == 1
            && RuntimeIntrinsics.Current.BuiltinAdapters.Contains(target);
    }

    internal static object? WrapJavaScriptVisibleValue(object? value)
        => value is Delegate target
            ? FromDelegate(target)
            : value;

    internal static object NormalizeJavaScriptObject(object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return value is Delegate target
            ? FromDelegate(target)
            : value;
    }

    internal void Configure(
        bool requiresInvocationContext,
        bool? isConstructor = null)
    {
        var resolvedIsConstructor = isConstructor ?? _isConstructor;
        _requiresInvocationContext =
            requiresInvocationContext
            || (resolvedIsConstructor
                && BuiltinFunctionDelegates.IsReceiverAware(Target));
        if (isConstructor.HasValue)
        {
            _isConstructor = resolvedIsConstructor;
        }
    }

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => WrapJavaScriptVisibleValue(
            Closure.InvokeBuiltinDelegate(
                Target,
                _invokeMetadata,
                _scopes,
                thisArgument,
                arguments,
                newTarget: null));

    protected override object? ConstructCore(in JsCallArguments arguments, object? newTarget)
        => Function.Construct(this, arguments, newTarget);

    protected override object? ResolveThisArgumentCore(object? thisArgument)
        => Function.GetEffectiveThisArg(
            this,
            _invokeMetadata.IsJsFuncDelegate,
            thisArgument);
}
