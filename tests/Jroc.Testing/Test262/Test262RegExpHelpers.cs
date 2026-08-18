using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262RegExpHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder.AddGlobalFactory("matchValidator", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Func<object?, object?, object?, object?>)MatchValidator,
            "matchValidator",
            3));
    }

    public static void CompareIterator(object? iterator, object? validators, object? message)
    {
        var length = TypeUtilities.ToNumber(ObjectRuntime.GetItem(validators!, "length"));
        for (var index = 0; index < length; index++)
        {
            var result = ObjectRuntime.CallMember(iterator!, "next", global::System.Array.Empty<object>());
            if (TypeUtilities.ToBoolean(ObjectRuntime.GetItem(result!, "done")))
            {
                Test262HostRuntimeIntrinsics.ThrowAssertion(message, $"Expected iterator value at index {index}");
            }

            var validator = ObjectRuntime.GetItem(validators!, (double)index);
            CallableOperations.Call1(validator, null, ObjectRuntime.GetItem(result!, "value"));
        }

        var completion = ObjectRuntime.CallMember(iterator!, "next", global::System.Array.Empty<object>());
        if (!TypeUtilities.ToBoolean(ObjectRuntime.GetItem(completion!, "done"))
            || ObjectRuntime.GetItem(completion!, "value") is not null)
        {
            Test262HostRuntimeIntrinsics.ThrowAssertion(message, "Expected iterator to complete");
        }
    }

    private static object? MatchValidator(object? expectedEntries, object? expectedIndex, object? expectedInput)
    {
        return Test262HostRuntimeIntrinsics.CreateFunction(
            (Action<object?>)(match =>
            {
                if (!CompareArray(match, expectedEntries)
                    || !JavaScriptRuntime.Object.@is(ObjectRuntime.GetItem(match!, "index"), expectedIndex)
                    || !JavaScriptRuntime.Object.@is(ObjectRuntime.GetItem(match!, "input"), expectedInput))
                {
                    Test262HostRuntimeIntrinsics.ThrowAssertion(null, "RegExp match did not match the expected result");
                }
            }),
            string.Empty,
            1);
    }

    private static bool CompareArray(object? actual, object? expected)
    {
        if (actual is null || expected is null)
        {
            return false;
        }

        var actualLength = TypeUtilities.ToNumber(ObjectRuntime.GetItem(actual, "length"));
        var expectedLength = TypeUtilities.ToNumber(ObjectRuntime.GetItem(expected, "length"));
        if (actualLength != expectedLength)
        {
            return false;
        }

        for (var index = 0; index < actualLength; index++)
        {
            if (!JavaScriptRuntime.Object.@is(
                    ObjectRuntime.GetItem(actual, (double)index),
                    ObjectRuntime.GetItem(expected, (double)index)))
            {
                return false;
            }
        }

        return true;
    }
}
