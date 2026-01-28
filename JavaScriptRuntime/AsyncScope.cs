namespace JavaScriptRuntime;

using System.Reflection;
using System;

public class AsyncScope : IAsyncScope
{
    public AsyncScope()
    {
        _deferred = Promise.withResolvers();
    }

    // State machine fields
    public int _asyncState;
    public PromiseWithResolvers? _deferred;
    public object? _moveNext;

    // Persistent storage for compiler-generated IL locals that must survive across awaits.
    // Indexed by MethodBodyIR variable slot.
    public object?[]? _locals;

    // Async try/finally completion tracking
    public object? _pendingException;
    public bool _hasPendingException;

    public object? _pendingReturnValue;
    public bool _hasPendingReturn;

    public int AsyncState
    {
        get => _asyncState;
        set => _asyncState = value;
    }

    public PromiseWithResolvers? Deferred
    {
        get => _deferred;
        set => _deferred = value;
    }

    public object? MoveNext
    {
        get => _moveNext;
        set => _moveNext = value;
    }

    /// <summary>
    /// Sets up async continuations for an await expression.
    /// Called by compiled async code at each await point to schedule MoveNext resumption.
    /// </summary>
    /// <param name="awaited">The value being awaited</param>
    /// <param name="scopesArray">The scopes array to pass to MoveNext</param>
    /// <param name="resultFieldName">Name of the field on the derived scope to store the awaited result (e.g., "_awaited1")</param>
    /// <param name="moveNext">The MoveNext method delegate. If null, uses <see cref="MoveNext"/>.</param>
    public void SetupAwaitContinuation(
        object? awaited,
        object[] scopesArray,
        string resultFieldName,
        object? moveNext)
    {
        if (string.Equals(Environment.GetEnvironmentVariable("JS2IL_DEBUG_ASYNC_REJECTIONS"), "1", StringComparison.Ordinal))
        {
            try
            {
                var promiseState = awaited is Promise debugPromise
                    ? debugPromise.GetType().GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(debugPromise)?.ToString()
                    : null;
                System.Console.Error.WriteLine($"[js2il] await setup: awaited={awaited?.GetType().FullName ?? "null"}, promiseState={promiseState ?? "n/a"}, resultField={resultFieldName}");
            }
            catch
            {
            }
        }

        // Wrap in Promise.resolve() per ECMA-262
        var promise = awaited is Promise p ? p : (Promise)Promise.resolve(awaited)!;

        // Get the scope type for reflection (awaited result storage)
        var scopeType = GetType();
        var resultField = scopeType.GetField(resultFieldName);

        var deferred = Deferred ?? throw new InvalidOperationException(
            $"Scope type {scopeType.Name} has null _deferred");

        // If moveNext is null, get it from the async scope
        moveNext ??= MoveNext;

        // Create onFulfilled callback: stores result, calls MoveNext
        var currentThis = RuntimeServices.GetCurrentThis();
        var onFulfilled = CreateFulfilledContinuation(this, scopesArray, resultField, moveNext, currentThis);

        // Create onRejected callback: rejects the outer promise
        var onRejected = CreateRejectedContinuation(this, deferred, currentThis);

        // Schedule continuations
        promise.@then(onFulfilled, onRejected);
    }

    /// <summary>
    /// Sets up async continuations for an await expression that should resume into a catch block on rejection.
    /// This stores the rejection reason into a pending-exception field and resumes MoveNext at a given state.
    /// </summary>
    public void SetupAwaitContinuationWithRejectResume(
        object? awaited,
        object[] scopesArray,
        string resultFieldName,
        object? moveNext,
        int rejectStateId,
        string pendingExceptionFieldName)
    {
        if (string.Equals(Environment.GetEnvironmentVariable("JS2IL_DEBUG_ASYNC_REJECTIONS"), "1", StringComparison.Ordinal))
        {
            try
            {
                var promiseState = awaited is Promise debugPromise
                    ? debugPromise.GetType().GetField("_state", BindingFlags.Instance | BindingFlags.NonPublic)?.GetValue(debugPromise)?.ToString()
                    : null;
                System.Console.Error.WriteLine($"[js2il] await setup(reject->{rejectStateId}): awaited={awaited?.GetType().FullName ?? "null"}, promiseState={promiseState ?? "n/a"}, resultField={resultFieldName}, pendingField={pendingExceptionFieldName}");
            }
            catch
            {
            }
        }

        var promise = awaited is Promise p ? p : (Promise)Promise.resolve(awaited)!;

        var scopeType = GetType();
        var resultField = scopeType.GetField(resultFieldName);
        var pendingField = scopeType.GetField(pendingExceptionFieldName);
        if (pendingField == null)
        {
            throw new InvalidOperationException($"Scope type {scopeType.Name} missing {pendingExceptionFieldName} field");
        }

        moveNext ??= MoveNext;

        var currentThis = RuntimeServices.GetCurrentThis();
        var onFulfilled = CreateFulfilledContinuation(this, scopesArray, resultField, moveNext, currentThis);

        var onRejected = CreateRejectedContinuationWithPendingException(
            this,
            scopesArray,
            pendingField,
            rejectStateId,
            moveNext,
            currentThis);

        promise.@then(onFulfilled, onRejected);
    }

    private static Func<object[]?, object?, object?> CreateFulfilledContinuation(
        object scope,
        object[] scopesArray,
        FieldInfo? resultField,
        object? moveNext,
        object? capturedThis)
    {
        return new Func<object[]?, object?, object?>((_, value) =>
        {
            var previousThis = RuntimeServices.SetCurrentThis(capturedThis);
            try
            {
                if (string.Equals(Environment.GetEnvironmentVariable("JS2IL_DEBUG_ASYNC_REJECTIONS"), "1", StringComparison.Ordinal))
                {
                    try
                    {
                        System.Console.Error.WriteLine($"[js2il] await fulfilled: value={value} ({value?.GetType().FullName ?? "null"}), resultField={(resultField?.Name ?? "<none>")}, moveNextType={(moveNext?.GetType().FullName ?? "null")}");
                    }
                    catch
                    {
                    }
                }

                // Store the awaited value to the result field if specified
                resultField?.SetValue(scope, value);

                // Call MoveNext to resume the state machine
                InvokeMoveNext(moveNext, scopesArray);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }

            return null;
        });
    }

    private static Func<object[]?, object?, object?> CreateRejectedContinuationWithPendingException(
        object scope,
        object[] scopesArray,
        FieldInfo pendingField,
        int rejectStateId,
        object? moveNext,
        object? capturedThis)
    {
        return new Func<object[]?, object?, object?>((_, reason) =>
        {
            var previousThis = RuntimeServices.SetCurrentThis(capturedThis);
            try
            {
                if (string.Equals(Environment.GetEnvironmentVariable("JS2IL_DEBUG_ASYNC_REJECTIONS"), "1", StringComparison.Ordinal))
                {
                    try
                    {
                        System.Console.Error.WriteLine($"[js2il] await rejected -> state {rejectStateId}: {reason} ({reason?.GetType().FullName ?? "null"})");
                    }
                    catch
                    {
                        // Best-effort debugging only.
                    }
                }

                pendingField.SetValue(scope, reason);
                ((IAsyncScope)scope).AsyncState = rejectStateId;
                InvokeMoveNext(moveNext, scopesArray);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }

            return null;
        });
    }

    private static void InvokeMoveNext(object? moveNext, object[] scopesArray)
    {
        if (moveNext == null)
        {
            throw new InvalidOperationException(
                "Cannot resume async function: _moveNext is null. The async function closure was not properly initialized.");
        }

        if (string.Equals(Environment.GetEnvironmentVariable("JS2IL_DEBUG_ASYNC_REJECTIONS"), "1", StringComparison.Ordinal))
        {
            try
            {
                System.Console.Error.WriteLine($"[js2il] invoking MoveNext: {moveNext.GetType().FullName}");
            }
            catch
            {
            }
        }

        if (moveNext is Func<object[], object> fn)
        {
            fn(scopesArray);
        }
        else if (moveNext is Action<object[]> action)
        {
            action(scopesArray);
        }
        else
        {
            Closure.InvokeWithArgs(moveNext, scopesArray);
        }
    }

    private static Func<object[]?, object?, object?> CreateRejectedContinuation(
        IAsyncScope scope,
        PromiseWithResolvers deferred,
        object? capturedThis)
    {
        return new Func<object[]?, object?, object?>((_, reason) =>
        {
            var previousThis = RuntimeServices.SetCurrentThis(capturedThis);
            try
            {
                // Mark state as completed
                scope.AsyncState = -1;

                // Reject the outer promise - use empty scopes array since reject doesn't need scopes
                Closure.InvokeWithArgs(deferred.reject, System.Array.Empty<object>(), reason);
            }
            finally
            {
                RuntimeServices.SetCurrentThis(previousThis);
            }

            return null;
        });
    }
}
