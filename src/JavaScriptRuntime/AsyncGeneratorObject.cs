using System;

namespace JavaScriptRuntime;

/// <summary>
/// Runtime representation of an async generator object.
///
/// This is the async counterpart to <see cref="GeneratorObject"/>. Each call to next/throw/return
/// creates a fresh promise (via <see cref="Promise.withResolvers"/>) and invokes the compiled step
/// method. Yield/return sites resolve the promise with an <see cref="IteratorResult"/>.
/// </summary>
public sealed class AsyncGeneratorObject : IJavaScriptAsyncIterator
{
    internal static readonly object PrototypeObject = CreatePrototype();
    private readonly object[] _scopes;

    public AsyncGeneratorObject(object[] scopes)
    {
        _scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        PrototypeChain.SetPrototype(this, PrototypeObject);
        GetLeafScope().ThisValue = RuntimeServices.GetCurrentThis();
    }

    private static object CreatePrototype()
    {
        var prototype = new JsObject();
        PrototypeChain.SetPrototype(prototype, AsyncIterator.Prototype);
        return prototype;
    }

    internal static void ConfigurePrototype(
        object asyncGeneratorFunctionPrototype)
    {
        using var _ = PropertyDescriptorStore.BeginIntrinsicInitialization();

        DefineDataProperty(
            PrototypeObject,
            "constructor",
            asyncGeneratorFunctionPrototype);
        DefineDataProperty(
            PrototypeObject,
            "next",
            (Func<object[], object?[]?, object?>)PrototypeNext);
        DefineDataProperty(
            PrototypeObject,
            "return",
            (Func<object[], object?[]?, object?>)PrototypeReturn);
        DefineDataProperty(
            PrototypeObject,
            "throw",
            (Func<object[], object?[]?, object?>)PrototypeThrow);
        DefineDataProperty(
            PrototypeObject,
            Symbol.toStringTag.DebugId,
            "AsyncGenerator");
    }

    private static void DefineDataProperty(
        object target,
        string key,
        object? value)
    {
        PropertyDescriptorStore.DefineOrUpdate(
            target,
            key,
            new JsPropertyDescriptor
            {
                Kind = JsPropertyDescriptorKind.Data,
                Enumerable = false,
                Configurable = true,
                Writable = true,
                Value = value
            });
    }

    private static AsyncGeneratorObject GetReceiver(string methodName)
    {
        if (RuntimeServices.GetCurrentThis()
            is AsyncGeneratorObject generator)
        {
            return generator;
        }

        throw new TypeError(
            $"AsyncGenerator.prototype.{methodName} called on incompatible receiver");
    }

    private static object? PrototypeNext(
        object[] scopes,
        object?[]? args)
        => GetReceiver("next").next(
            args is { Length: > 0 } ? args[0] : null);

    private static object? PrototypeReturn(
        object[] scopes,
        object?[]? args)
        => GetReceiver("return").@return(
            args is { Length: > 0 } ? args[0] : null);

    private static object? PrototypeThrow(
        object[] scopes,
        object?[]? args)
        => GetReceiver("throw").@throw(
            args is { Length: > 0 } ? args[0] : null);

    private AsyncGeneratorScope GetLeafScope()
    {
        if (_scopes.Length == 0)
        {
            throw new InvalidOperationException("Async generator scopes array is empty.");
        }

        if (_scopes[0] is not AsyncGeneratorScope ags)
        {
            throw new InvalidOperationException($"Async generator scopes[0] is not an AsyncGeneratorScope (actual={_scopes[0]?.GetType().FullName ?? "<null>"}).");
        }

        return ags;
    }

    private PromiseWithResolvers PrepareDeferred(AsyncGeneratorScope scope)
    {
        var deferred = Promise.withResolvers();
        scope.Deferred = deferred;
        scope.AsyncState = 0;
        return deferred;
    }

    private static void RejectDeferred(PromiseWithResolvers deferred, object? reason)
    {
        CallableOperations.Call1(deferred.reject, null, reason);
    }

    private void InvokeMoveNext(
        AsyncGeneratorScope scope,
        CompiledContinuation moveNext)
    {
        var previousThis = RuntimeServices.SetCurrentThis(
            RuntimeServices.ResolveLexicalThis(scope.ThisValue));
        try
        {
            moveNext.Resume();
        }
        finally
        {
            RuntimeServices.SetCurrentThis(previousThis);
        }
    }

    public object next(object? value = null)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            return Promise.resolve(IteratorResult.Create(null, done: true))!;
        }

        // Clear prior resume protocol.
        scope.HasResumeException = false;
        scope.ResumeException = null;
        scope.HasReturn = false;
        scope.ReturnValue = null;

        // On first next(arg), arg is ignored per JS semantics.
        scope.ResumeValue = scope.Started ? value : null;
        scope.Started = true;

        var deferred = PrepareDeferred(scope);

        var moveNext = scope.MoveNext;
        if (moveNext == null)
        {
            throw new InvalidOperationException("Async generator MoveNext is null. The closure was not properly initialized.");
        }

        try
        {
            InvokeMoveNext(scope, moveNext);
        }
        catch (Exception ex)
        {
            scope.Done = true;
            scope.AsyncState = -1;

            var reason = ex is JsThrownValueException jsv ? jsv.Value : ex;
            RejectDeferred(deferred, reason);
        }

        return deferred.promise!;
    }

    public object @throw(object? error)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            return Promise.reject(error)!;
        }

        scope.ResumeValue = null;
        scope.HasReturn = false;
        scope.ReturnValue = null;

        scope.HasResumeException = true;
        scope.ResumeException = error;

        var deferred = PrepareDeferred(scope);

        var moveNext = scope.MoveNext;
        if (moveNext == null)
        {
            throw new InvalidOperationException("Async generator MoveNext is null. The closure was not properly initialized.");
        }

        try
        {
            InvokeMoveNext(scope, moveNext);
        }
        catch (Exception ex)
        {
            scope.Done = true;
            scope.AsyncState = -1;

            var reason = ex is JsThrownValueException jsv ? jsv.Value : ex;
            RejectDeferred(deferred, reason);
        }

        return deferred.promise!;
    }

    public object @return(object? value)
    {
        var scope = GetLeafScope();

        if (scope.Done)
        {
            return Promise.resolve(IteratorResult.Create(value, done: true))!;
        }

        scope.HasResumeException = false;
        scope.ResumeException = null;
        scope.ResumeValue = null;

        scope.HasReturn = true;
        scope.ReturnValue = value;

        var deferred = PrepareDeferred(scope);

        var moveNext = scope.MoveNext;
        if (moveNext == null)
        {
            throw new InvalidOperationException("Async generator MoveNext is null. The closure was not properly initialized.");
        }

        try
        {
            InvokeMoveNext(scope, moveNext);
        }
        catch (Exception ex)
        {
            scope.Done = true;
            scope.AsyncState = -1;

            var reason = ex is JsThrownValueException jsv ? jsv.Value : ex;
            RejectDeferred(deferred, reason);
        }

        return deferred.promise!;
    }

    // IJavaScriptAsyncIterator (for for await..of lowering)
    public object? Next() => next();

    public bool HasReturn => true;

    public object? Return() => @return(null);
}
