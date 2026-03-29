using System;

namespace JavaScriptRuntime;

public static class AsyncIterator
{
    internal static readonly object Prototype = CreatePrototype();

    internal static void ConfigureIntrinsicSurface(object asyncIteratorConstructorValue)
    {
        DefineDataProperty(asyncIteratorConstructorValue, "prototype", Prototype);
        DefineDataProperty(Prototype, "constructor", asyncIteratorConstructorValue);
        DefineDataProperty(Prototype, "next", (Func<object[], object?[]?, object?>)PrototypeNext);
        DefineDataProperty(Prototype, "return", (Func<object[], object?[]?, object?>)PrototypeReturn);
        DefineDataProperty(Prototype, Symbol.asyncIterator.DebugId, (Func<object[], object?[]?, object?>)PrototypeSymbolAsyncIterator);
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "AsyncIterator");
    }

    internal static void InitializeAsyncIteratorSurface(object iterator)
    {
        if (PrototypeChain.GetPrototypeOrNull(iterator) == null)
        {
            PrototypeChain.SetPrototype(iterator, Prototype);
        }
    }

    private static object CreatePrototype()
    {
        return new JsObject();
    }

    private static void DefineDataProperty(object target, string key, object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(target, key, new JsPropertyDescriptor
        {
            Kind = JsPropertyDescriptorKind.Data,
            Enumerable = false,
            Configurable = true,
            Writable = true,
            Value = value
        });
    }

    private static object? PrototypeNext(object[] scopes, object?[]? args)
    {
        var receiver = RuntimeServices.GetCurrentThis();
        if (receiver is IJavaScriptAsyncIterator iterator)
        {
            return iterator.Next();
        }

        throw new TypeError("AsyncIterator.prototype.next called on incompatible receiver");
    }

    private static object? PrototypeReturn(object[] scopes, object?[]? args)
    {
        var receiver = RuntimeServices.GetCurrentThis();
        if (receiver is IJavaScriptAsyncIterator iterator)
        {
            return iterator.HasReturn
                ? iterator.Return()
                : Promise.resolve(IteratorResult.Create(null, done: true));
        }

        throw new TypeError("AsyncIterator.prototype.return called on incompatible receiver");
    }

    private static object? PrototypeSymbolAsyncIterator(object[] scopes, object?[]? args)
    {
        return RuntimeServices.GetCurrentThis();
    }
}
