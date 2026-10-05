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
        DefineSymbolFunction(Symbol.asyncIterator, (BuiltinFunction0)PrototypeSymbolAsyncIterator);
        DefineSymbolFunction(Symbol.asyncDispose, (BuiltinFunction0)PrototypeSymbolAsyncDispose);
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

    private static void DefineSymbolFunction(Symbol symbol, BuiltinFunction0 function)
    {
        Function.InitializeFunctionInstance(function, 0d, $"[{symbol.Description}]", requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(function);
        DefineDataProperty(Prototype, symbol.DebugId, function);
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

    private static object? PrototypeSymbolAsyncDispose(object? thisArgument)
    {
        var capability = Promise.withResolvers();
        try
        {
            if (thisArgument is null or JsNull)
            {
                throw new TypeError("Async iterator disposal requires a non-null receiver");
            }

            var returnMethod = ObjectRuntime.GetProperty(thisArgument, "return");
            if (returnMethod is null or JsNull)
            {
                CallableOperations.Call1(capability.resolve, null, null);
            }
            else
            {
                if (!CallableOperations.IsCallable(returnMethod))
                {
                    throw new TypeError("Async iterator return method is not callable");
                }

                var result = CallableOperations.Call1(returnMethod, thisArgument, null);
                var promise = (Promise)Promise.resolve(result)!;
                BuiltinFunction1 fulfilled = (_, _) =>
                {
                    CallableOperations.Call1(capability.resolve, null, null);
                    return null;
                };
                promise.then(fulfilled, capability.reject);
            }
        }
        catch (Exception exception) when (exception is not ScriptProcessExitException)
        {
            var reason = exception is JsThrownValueException thrown ? thrown.Value : exception;
            CallableOperations.Call1(capability.reject, null, reason);
        }

        return capability.promise;
    }
}
