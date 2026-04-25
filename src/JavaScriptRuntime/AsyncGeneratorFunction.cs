using System;

namespace JavaScriptRuntime;

public static class AsyncGeneratorFunction
{
    private static readonly Func<object[], object?, object?> _constructor = AsyncGeneratorFunctionConstructor;
    private static readonly JsObject Prototype = CreatePrototype();

    static AsyncGeneratorFunction()
    {
        PrototypeChain.SetPrototype(_constructor, JavaScriptRuntime.Function.Prototype);
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

    private static object? AsyncGeneratorFunctionConstructor(object[] scopes, object? bodyArg)
    {
        throw new NotSupportedException("The AsyncGeneratorFunction constructor is not supported yet.");
    }
}
