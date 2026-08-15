using System;

namespace JavaScriptRuntime;

public static class AsyncGeneratorFunction
{
    private static readonly Func<object[], object?[]?, object?> _constructor = AsyncGeneratorFunctionConstructor;
    /// <summary>Realm-owned <c>%AsyncGeneratorFunction.prototype%</c> (issue #1824).</summary>
    internal static JsObject Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.AsyncGeneratorFunctionPrototype,
            static () => new JsObject(),
            static prototype => InitializePrototype(prototype));

    public static object InitializeFunctionObject(object functionObject)
    {
        ArgumentNullException.ThrowIfNull(functionObject);
        PrototypeChain.SetPrototype(functionObject, Prototype);
        GeneratorObject.EnsureGeneratorFunctionPrototypeProperty(
            functionObject,
            AsyncGeneratorObject.PrototypeObject);
        return functionObject;
    }

    /// <summary>
    /// Wires this realm's <c>%AsyncGeneratorFunction%</c> surface. Runs once per realm
    /// from the intrinsic slot initializer (issue #1824) rather than once per process
    /// from a static constructor.
    /// </summary>
    private static void InitializePrototype(JsObject prototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        PrototypeChain.SetPrototype(prototype, JavaScriptRuntime.Function.Prototype);
        PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = _constructor
        });

        JavaScriptRuntime.Function.InitializeFunctionInstance(_constructor, 1d, "AsyncGeneratorFunction", requiresInvocationContext: false);
        JavaScriptRuntime.Function.MarkConstructible(_constructor);
        PrototypeChain.SetPrototype(_constructor, GlobalThis.Function);
        PropertyDescriptorStore.DefineOrUpdate(_constructor, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = prototype
        });
        AsyncGeneratorObject.ConfigurePrototype(prototype);
    }

    private static object? AsyncGeneratorFunctionConstructor(object[] scopes, object?[]? args)
    {
        var callArgs = args ?? System.Array.Empty<object?>();
        var length = JavaScriptRuntime.Function.ParseDynamicFunctionParameterNames(callArgs).Length;

        Func<object[], object?[]?, object?> functionValue = static (_, __) =>
            throw new NotSupportedException("Dynamically constructed async generator functions are not invokable in jroc. Use statically declared async generator functions instead.");
        JavaScriptRuntime.AsyncFunction.InitializeFunctionInstance(functionValue, length, "anonymous", requiresInvocationContext: false);
        InitializeFunctionObject(functionValue);
        return BuiltinDelegateFunctionAdapter.FromDelegate(functionValue);
    }
}
