using System;
using System.Collections.Generic;

namespace JavaScriptRuntime;

/// <summary>
/// Runtime representation of an async generator object.
///
/// This is the async counterpart to <see cref="GeneratorObject"/>. Each call to next/throw/return
/// creates a fresh promise (via <see cref="Promise.withResolvers"/>) and invokes the compiled step
/// method. Yield/return sites resolve the promise with an <see cref="IteratorResult"/>.
/// </summary>
public sealed class AsyncGeneratorObject : JsObject, IJavaScriptAsyncIterator
{
    /// <summary>Realm-owned <c>%AsyncGeneratorPrototype%</c> (issue #1824).</summary>
    internal static object PrototypeObject
        => RuntimeIntrinsics.Current.GetOrCreate(
            RuntimeIntrinsicSlot.AsyncGeneratorPrototype,
            static () => new JsObject(),
            static prototype =>
            {
                PrototypeChain.SetPrototype(prototype, AsyncIterator.Prototype);

                // Touching the realm's %AsyncGeneratorFunction.prototype% installs this
                // object's own methods through ConfigurePrototype below.
                _ = AsyncGeneratorFunction.Prototype;
            });
    private readonly object[] _scopes;
    private enum RequestKind { Next, Throw, Return }
    private sealed record Request(RequestKind Kind, object? Value, PromiseWithResolvers Capability);
    private readonly Queue<Request> _requests = new();
    private bool _active;
    private bool _draining;

    public AsyncGeneratorObject(object[] scopes)
    {
        _scopes = scopes ?? throw new ArgumentNullException(nameof(scopes));
        PrototypeChain.InitializePrototype(this, PrototypeObject);
        GetLeafScope().ThisValue = RuntimeServices.GetCurrentThis();
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
            (BuiltinFunction1)PrototypeNext);
        DefineDataProperty(
            PrototypeObject,
            "return",
            (BuiltinFunction1)PrototypeReturn);
        DefineDataProperty(
            PrototypeObject,
            "throw",
            (BuiltinFunction1)PrototypeThrow);
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

    private static AsyncGeneratorObject GetReceiver(object? thisValue, string methodName)
    {
        if (thisValue is AsyncGeneratorObject generator)
        {
            return generator;
        }

        throw new TypeError(
            $"AsyncGenerator.prototype.{methodName} called on incompatible receiver");
    }

    private static object? PrototypeNext(
        object? thisArgument,
        object? valueArgument)
        => GetReceiver(thisArgument, "next").next(valueArgument);

    private static object? PrototypeReturn(
        object? thisArgument,
        object? valueArgument)
        => GetReceiver(thisArgument, "return").@return(valueArgument);

    private static object? PrototypeThrow(
        object? thisArgument,
        object? valueArgument)
        => GetReceiver(thisArgument, "throw").@throw(valueArgument);

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

    public object next(object? value = null) => Enqueue(RequestKind.Next, value);

    public object @throw(object? error) => Enqueue(RequestKind.Throw, error);

    public object @return(object? value) => Enqueue(RequestKind.Return, value);

    private object Enqueue(RequestKind kind, object? value)
    {
        var capability = Promise.withResolvers();
        _requests.Enqueue(new Request(kind, value, capability));
        Drain();
        return capability.promise;
    }

    private void Drain()
    {
        if (_draining)
        {
            return;
        }

        _draining = true;
        try
        {
            while (!_active && _requests.Count > 0)
            {
                _active = true;
                StartRequest(_requests.Peek());
            }
        }
        finally
        {
            _draining = false;
        }
    }

    private void CompleteRequest(Request request, object? value, bool rejected)
    {
        if (rejected)
        {
            var scope = GetLeafScope();
            scope.Done = true;
            scope.AsyncState = -1;
        }

        CallableOperations.Call1(
            rejected ? request.Capability.reject : request.Capability.resolve, null, value);
        _requests.Dequeue();
        _active = false;
        Drain();
    }

    private void StartRequest(Request request)
    {
        var scope = GetLeafScope();
        try
        {
            if (!scope.Started && request.Kind != RequestKind.Next)
            {
                scope.Done = true;
            }

            if (scope.Done)
            {
                if (request.Kind == RequestKind.Return)
                {
                    ((Promise)Promise.resolve(request.Value)!).then(
                        (BuiltinFunction1)((_, value) =>
                        {
                            CompleteRequest(request, IteratorResult.Create(value, done: true), rejected: false);
                            return null;
                        }),
                        (BuiltinFunction1)((_, reason) =>
                        {
                            CompleteRequest(request, reason, rejected: true);
                            return null;
                        }));
                }
                else
                {
                    CompleteRequest(request,
                        request.Kind == RequestKind.Throw ? request.Value : IteratorResult.Create(null, done: true),
                        rejected: request.Kind == RequestKind.Throw);
                }
                return;
            }

            scope.HasResumeException = request.Kind == RequestKind.Throw;
            scope.ResumeException = scope.HasResumeException ? request.Value : null;
            scope.HasReturn = request.Kind == RequestKind.Return;
            scope.ReturnValue = scope.HasReturn ? request.Value : null;
            scope.ResumeValue = request.Kind == RequestKind.Next && scope.Started ? request.Value : null;
            scope.Started = true;
            scope.AsyncState = 0;

            // Completion hooks release the queue without observing the user's promise,
            // which would incorrectly mark an unhandled rejection as handled.
            scope.Deferred = new PromiseWithResolvers(
                request.Capability.promise,
                (BuiltinFunction1)((_, value) =>
                {
                    CompleteRequest(request, value, rejected: false);
                    return null;
                }),
                (BuiltinFunction1)((_, reason) =>
                {
                    CompleteRequest(request, reason, rejected: true);
                    return null;
                }));
            InvokeMoveNext(scope, scope.MoveNext
                ?? throw new InvalidOperationException("Async generator MoveNext is null."));
        }
        catch (Exception exception)
        {
            CompleteRequest(request,
                exception is JsThrownValueException thrown ? thrown.Value : exception,
                rejected: true);
        }
    }

    // IJavaScriptAsyncIterator (for for await..of lowering)
    public object? Next() => next();

    public bool HasReturn => true;

    public object? Return() => @return(null);
}
