using System;

namespace JavaScriptRuntime;

public static class AsyncIterator
{
    /// <summary>Realm-owned <c>%AsyncIteratorPrototype%</c> (issue #1824).</summary>
    internal static object Prototype
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.AsyncIteratorPrototype,
            static () => new JsObject());

    internal static void ConfigureIntrinsicSurface(object asyncIteratorConstructorValue)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        DefineDataProperty(asyncIteratorConstructorValue, "prototype", Prototype);
        DefineDataProperty(Prototype, "constructor", asyncIteratorConstructorValue);
        DefineDataProperty(Prototype, "next", (BuiltinFunction0)PrototypeNext);
        DefineDataProperty(Prototype, "return", (BuiltinFunction1)PrototypeReturn);
        DefineDataProperty(Prototype, Symbol.asyncIterator.DebugId, (BuiltinFunction0)PrototypeSymbolAsyncIterator);
        DefineDataProperty(Prototype, Symbol.toStringTag.DebugId, "AsyncIterator");
    }

    internal static void InitializeAsyncIteratorSurface(object iterator)
    {
        if (PrototypeChain.GetPrototypeOrNull(iterator) == null)
        {
            PrototypeChain.SetPrototype(iterator, Prototype);
        }
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

    private static object? PrototypeNext(object? thisArgument)
    {
        if (thisArgument is IJavaScriptAsyncIterator iterator)
        {
            return iterator.Next();
        }

        throw new TypeError("AsyncIterator.prototype.next called on incompatible receiver");
    }

    private static object? PrototypeReturn(object? thisArgument, object? returnValue)
    {
        if (thisArgument is AsyncGeneratorObject asyncGenerator)
        {
            return asyncGenerator.@return(returnValue);
        }

        if (thisArgument is IJavaScriptAsyncIterator iterator)
        {
            return iterator.HasReturn
                ? iterator.Return()
                : Promise.resolve(IteratorResult.Create(null, done: true));
        }

        throw new TypeError("AsyncIterator.prototype.return called on incompatible receiver");
    }

    private static object? PrototypeSymbolAsyncIterator(object? thisArgument)
    {
        return thisArgument;
    }
}
