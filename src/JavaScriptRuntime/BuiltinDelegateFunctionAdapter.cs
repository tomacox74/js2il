namespace JavaScriptRuntime;

using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.CompilerServices;

/// <summary>
/// Explicit adapter for runtime-owned built-in CLR delegates.
/// Compiled JavaScript functions never use this representation.
/// </summary>
public sealed class BuiltinDelegateFunctionAdapter : JsFunctionObject
{
    private sealed class AdapterTypeCache
    {
        public ConcurrentDictionary<Type, BuiltinDelegateFunctionAdapter> Adapters { get; } = new();
    }

    private sealed class AdapterMethodCache
    {
        public ConcurrentDictionary<(MethodInfo Method, Type DelegateType), BuiltinDelegateFunctionAdapter> Adapters { get; } = new();
    }

    private static readonly ConditionalWeakTable<MethodInfo, AdapterTypeCache> StaticAdapters = new();
    private static readonly ConditionalWeakTable<object, AdapterMethodCache> InstanceAdapters = new();

    private readonly object[] _scopes;
    private readonly Closure.DelegateInvokeMetadata _invokeMetadata;
    private readonly object _initializationLock = new();
    private bool _isConstructor;
    private bool _requiresInvocationContext = true;

    public BuiltinDelegateFunctionAdapter(
        Delegate target,
        object[]? scopes = null,
        bool isConstructor = false)
    {
        Target = target ?? throw new ArgumentNullException(nameof(target));
        _scopes = scopes ?? RuntimeServices.EmptyScopes;
        _invokeMetadata = Closure.GetDelegateInvokeMetadata(target);
        _isConstructor = isConstructor;
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

        if (target.Target == null)
        {
            var cache = StaticAdapters.GetOrCreateValue(target.Method);
            return cache.Adapters.GetOrAdd(
                target.GetType(),
                _ => new BuiltinDelegateFunctionAdapter(target));
        }

        var instanceCache = InstanceAdapters.GetOrCreateValue(target.Target);
        return instanceCache.Adapters.GetOrAdd(
            (target.Method, target.GetType()),
            _ => new BuiltinDelegateFunctionAdapter(target));
    }

    internal static bool HasStableAdapterForTests(Delegate target)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (target.GetInvocationList().Length != 1)
        {
            return false;
        }

        if (target.Target == null)
        {
            return StaticAdapters.TryGetValue(target.Method, out var staticCache)
                && staticCache.Adapters.ContainsKey(target.GetType());
        }

        return InstanceAdapters.TryGetValue(target.Target, out var instanceCache)
            && instanceCache.Adapters.ContainsKey((target.Method, target.GetType()));
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
        _requiresInvocationContext = requiresInvocationContext;
        if (isConstructor.HasValue)
        {
            _isConstructor = isConstructor.Value;
        }
    }

    protected override object? CallCore(object? thisArgument, in JsCallArguments arguments)
        => WrapJavaScriptVisibleValue(
            Closure.InvokeBuiltinDelegate(
                Target,
                _invokeMetadata,
                _scopes,
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
