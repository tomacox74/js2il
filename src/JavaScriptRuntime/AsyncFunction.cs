using System;

namespace JavaScriptRuntime;

public static class AsyncFunction
{
    internal static readonly object Prototype = CreatePrototype();
    private static readonly Func<object[], object?[], object?> ConstructorValue = static (_, args) =>
        CreateDynamicAsyncFunction(args);

    static AsyncFunction()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        Function.InitializeFunctionInstance(ConstructorValue);
        Function.MarkConstructible(ConstructorValue);
        PrototypeChain.SetPrototype(ConstructorValue, GlobalThis.Function);
        PrototypeChain.SetPrototype(Prototype, Function.Prototype);

        DefineDataProperty(
            ConstructorValue,
            "length",
            1d,
            writable: false,
            configurable: true);
        DefineDataProperty(
            ConstructorValue,
            "name",
            "AsyncFunction",
            writable: false,
            configurable: true);
        DefineDataProperty(
            ConstructorValue,
            "prototype",
            Prototype,
            writable: false,
            configurable: false);

        DefineDataProperty(Prototype, "constructor", ConstructorValue);
        DefineDataProperty(
            Prototype,
            Symbol.toStringTag.DebugId,
            "AsyncFunction",
            writable: false,
            configurable: true);
    }

    public static T InitializeFunctionInstance<T>(T functionValue)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(functionValue);
        var functionObject =
            BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(
                functionValue);

        if (functionObject is BuiltinDelegateFunctionAdapter adapter)
        {
            lock (adapter.InitializationLock)
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                InitializeFunctionPrototype(functionObject);
            }
        }
        else
        {
            InitializeFunctionPrototype(functionObject);
        }

        return functionValue;
    }

    private static void InitializeFunctionPrototype(object functionObject)
    {
        if (!ReferenceEquals(
                PrototypeChain.GetPrototypeOrNull(functionObject),
                Prototype))
        {
            PrototypeChain.SetPrototype(functionObject, Prototype);
        }
    }

    public static object InitializeFunctionInstance(object functionValue)
        => InitializeFunctionInstance<object>(functionValue);

    public static T InitializeFunctionInstance<T>(T functionValue, double length, string? name)
        where T : class
    {
        return InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext: true);
    }

    public static object InitializeFunctionInstance(object functionValue, double length, string? name)
        => InitializeFunctionInstance<object>(functionValue, length, name);

    public static T InitializeFunctionInstance<T>(T functionValue, double length, string? name, bool requiresInvocationContext)
        where T : class
        => InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext, hasRestrictedProperties: false);

    public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext)
        => InitializeFunctionInstance<object>(functionValue, length, name, requiresInvocationContext);

    public static T InitializeFunctionInstance<T>(T functionValue, double length, string? name, bool requiresInvocationContext, bool hasRestrictedProperties)
        where T : class
    {
        var functionObject =
            BuiltinDelegateFunctionAdapter.NormalizeJavaScriptObject(
                functionValue);

        if (functionObject is BuiltinDelegateFunctionAdapter adapter)
        {
            lock (adapter.InitializationLock)
            {
                using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();
                InitializeFunctionInstanceCore(
                    functionObject,
                    length,
                    name,
                    requiresInvocationContext,
                    hasRestrictedProperties);
            }
        }
        else
        {
            InitializeFunctionInstanceCore(
                functionObject,
                length,
                name,
                requiresInvocationContext,
                hasRestrictedProperties);
        }

        return functionValue;
    }

    private static void InitializeFunctionInstanceCore(
        object functionObject,
        double length,
        string? name,
        bool requiresInvocationContext,
        bool hasRestrictedProperties)
    {
        InitializeFunctionPrototype(functionObject);
        if (hasRestrictedProperties)
        {
            Function.DefineRestrictedFunctionProperties(functionObject);
        }

        if (functionObject is BuiltinDelegateFunctionAdapter adapter)
        {
            adapter.Configure(requiresInvocationContext);
            Function.MarkUndefinedPrototype(adapter);
        }
        Function.DefineMetadataProperty(functionObject, "length", length);
        Function.DefineMetadataProperty(functionObject, "name", name ?? string.Empty);
    }

    public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext, bool hasRestrictedProperties)
        => InitializeFunctionInstance<object>(functionValue, length, name, requiresInvocationContext, hasRestrictedProperties);

    private static object CreatePrototype()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        return new JsObject();
    }

    private static object CreateDynamicAsyncFunction(object?[]? args)
    {
        var callArgs = args ?? System.Array.Empty<object?>();
        var length = Function.ParseDynamicFunctionParameterNames(callArgs).Length;

        Func<object[], object?[]?, object?> functionValue = static (_, __) => Promise.resolve(null);
        InitializeFunctionInstance(functionValue, length, "anonymous", requiresInvocationContext: false);
        return BuiltinDelegateFunctionAdapter.FromDelegate(functionValue);
    }

    private static void DefineDataProperty(object target, string key, object? value, bool writable = true, bool configurable = true)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = configurable,
            Writable = writable,
            Value = value
        });
    }
}
