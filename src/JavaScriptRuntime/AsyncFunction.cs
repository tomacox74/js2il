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

    public static object InitializeFunctionInstance(object functionValue)
    {
        ArgumentNullException.ThrowIfNull(functionValue);

        if (!ReferenceEquals(PrototypeChain.GetPrototypeOrNull(functionValue), Prototype))
        {
            PrototypeChain.SetPrototype(functionValue, Prototype);
        }

        return functionValue;
    }

    public static object InitializeFunctionInstance(object functionValue, double length, string? name)
    {
        return InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext: true);
    }

    public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext)
        => InitializeFunctionInstance(functionValue, length, name, requiresInvocationContext, hasRestrictedProperties: false);

    public static object InitializeFunctionInstance(object functionValue, double length, string? name, bool requiresInvocationContext, bool hasRestrictedProperties)
    {
        InitializeFunctionInstance(functionValue);
        if (hasRestrictedProperties)
        {
            Function.DefineRestrictedFunctionProperties(functionValue);
        }

        if (functionValue is Delegate del)
        {
            Function.SetRequiresInvocationContext(del, requiresInvocationContext);
            Function.DefineMetadataProperty(del, "length", length);
            Function.DefineMetadataProperty(del, "name", name ?? string.Empty);
            Function.MarkUndefinedPrototype(del);
        }

        return functionValue;
    }

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
        return functionValue;
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
