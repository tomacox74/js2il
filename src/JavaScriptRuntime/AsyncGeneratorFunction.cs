using System;

namespace JavaScriptRuntime;

public static class AsyncGeneratorFunction
{
    private static readonly Func<object[], object?[]?, object?> _constructor = AsyncGeneratorFunctionConstructor;
    private static readonly JsObject Prototype = CreatePrototype();

    static AsyncGeneratorFunction()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        JavaScriptRuntime.Function.InitializeFunctionInstance(_constructor, 1d, "AsyncGeneratorFunction", requiresInvocationContext: false);
        PrototypeChain.SetPrototype(_constructor, GlobalThis.Function);
        PropertyDescriptorStore.DefineOrUpdate(_constructor, "prototype", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = false,
            Writable = false,
            Value = Prototype
        });
    }

    public static object InitializeFunctionObject(object functionObject)
    {
        ArgumentNullException.ThrowIfNull(functionObject);
        PrototypeChain.SetPrototype(functionObject, Prototype);
        return functionObject;
    }

    private static JsObject CreatePrototype()
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        var prototype = new JsObject();
        PrototypeChain.SetPrototype(prototype, JavaScriptRuntime.Function.Prototype);
        PropertyDescriptorStore.DefineOrUpdate(prototype, "constructor", new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = _constructor
        });
        return prototype;
    }

    private static object? AsyncGeneratorFunctionConstructor(object[] scopes, object?[]? args)
    {
        var callArgs = args ?? System.Array.Empty<object?>();
        var length = JavaScriptRuntime.Function.ParseDynamicFunctionParameterNames(callArgs).Length;

        Func<object[], object?[]?, object?> functionValue = static (_, __) =>
            throw new NotSupportedException("Dynamically constructed async generator functions are not invokable in jroc. Use statically declared async generator functions instead.");
        JavaScriptRuntime.AsyncFunction.InitializeFunctionInstance(functionValue, length, "anonymous", requiresInvocationContext: false);
        InitializeFunctionObject(functionValue);
        return functionValue;
    }
}
