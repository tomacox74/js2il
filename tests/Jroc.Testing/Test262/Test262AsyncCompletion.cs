using JavaScriptRuntime;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// Owned by a single harness execution, not by the process or a promise reaction.
internal sealed class Test262AsyncCompletion
{
    private readonly bool _requiresCompletion;
    private int _completionCount;
    private Exception? _failure;

    public Test262AsyncCompletion(bool requiresCompletion = true)
        => _requiresCompletion = requiresCompletion;

    public void Done(object? error)
        // Pinned doneprintHandle.js uses `if (error)`. Consequently asyncHelpers.js
        // also treats a forwarded falsy rejection/throw as successful completion.
        => Complete(TypeUtilities.ToBoolean(error), error);

    private void Complete(bool failed, object? error)
    {
        _completionCount++;
        if (failed)
        {
            _failure ??= error as Exception ?? new JsThrownValueException(error);
        }
        if (_completionCount > 1)
        {
            _failure ??= CreateTest262Error("$DONE called more than once");
        }
    }

    public void AsyncTest(object? testFunc)
    {
        if (!_requiresCompletion)
        {
            Complete(true, CreateTest262Error("asyncTest called without async flag"));
            return;
        }
        if (!CallableOperations.IsCallable(testFunc))
        {
            Complete(true, CreateTest262Error("asyncTest called with non-function argument"));
            return;
        }

        try
        {
            var result = Invoke(testFunc);
            var fulfilled = CreateFunction((Action)(() => Done(null)), "", 0);
            var rejected = CreateFunction((Action<object?>)Done, "", 1);
            ObjectRuntime.CallMember(result!, "then", new object[] { fulfilled, rejected });
        }
        catch (Exception error)
        {
            Done(error is JsThrownValueException thrown ? thrown.Value : error);
        }
    }

    public Exception? GetFailure(string testName, bool hasUnhandledException)
    {
        if (_failure is not null)
        {
            return _failure;
        }
        if (_requiresCompletion && _completionCount == 0 && !hasUnhandledException)
        {
            throw new InvalidOperationException(
                $"Test262 async test '{testName}' completed without calling $DONE.");
        }
        return null;
    }
}
