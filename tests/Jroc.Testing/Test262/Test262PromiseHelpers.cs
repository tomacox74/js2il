using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262PromiseHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("checkSequence", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object?, object?, bool>)CheckSequence,
                "checkSequence",
                2))
            .AddGlobalFactory("checkSettledPromises", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Action<object?, object?, object?>)CheckSettledPromises,
                "checkSettledPromises",
                3));
    }

    private static bool CheckSequence(object? values, object? message)
    {
        var entries = EnumerateArrayLike(values!).ToArray();
        for (var i = 0; i < entries.Length; i++)
        {
            if (!JavaScriptRuntime.Object.@is(entries[i], (double)(i + 1)))
            {
                var prefix = string.IsNullOrEmpty(Test262HostRuntimeIntrinsics.ToMessage(message))
                    ? "Steps in unexpected sequence:"
                    : Test262HostRuntimeIntrinsics.ToMessage(message);
                throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                    $"{prefix} '{string.Join(",", entries.Select(Test262HostRuntimeIntrinsics.ToMessage))}'");
            }
        }

        return true;
    }

    private static void CheckSettledPromises(object? settledValues, object? expectedValues, object? message)
    {
        var settled = EnumerateArrayLike(settledValues!).ToArray();
        var expected = EnumerateArrayLike(expectedValues!).ToArray();
        var prefix = string.IsNullOrEmpty(Test262HostRuntimeIntrinsics.ToMessage(message))
            ? string.Empty
            : $"{Test262HostRuntimeIntrinsics.ToMessage(message)}: ";
        Assert(JavaScriptRuntime.Array.isArray(settledValues), $"{prefix}Settled values is an array");
        Assert(settled.Length == expected.Length, $"{prefix}The settled values has a different length than expected");

        for (var i = 0; i < settled.Length; i++)
        {
            var actualItem = settled[i]!;
            var expectedItem = expected[i]!;
            Assert(Test262HostRuntimeIntrinsics.HasOwn(actualItem, "status"), $"{prefix}The settled value has a property status");
            var status = ObjectRuntime.GetItem(actualItem, "status");
            AssertSame(status, ObjectRuntime.GetItem(expectedItem, "status"), $"{prefix}status for item {i}");
            if (string.Equals(Test262HostRuntimeIntrinsics.ToMessage(status), "fulfilled", StringComparison.Ordinal))
            {
                Assert(Test262HostRuntimeIntrinsics.HasOwn(actualItem, "value"), $"{prefix}The fulfilled promise has a property named value");
                Assert(!Test262HostRuntimeIntrinsics.HasOwn(actualItem, "reason"), $"{prefix}The fulfilled promise has no property named reason");
                AssertSame(ObjectRuntime.GetItem(actualItem, "value"), ObjectRuntime.GetItem(expectedItem, "value"), $"{prefix}value for item {i}");
            }
            else
            {
                AssertSame(status, "rejected", $"{prefix}Valid statuses are only fulfilled or rejected");
                Assert(!Test262HostRuntimeIntrinsics.HasOwn(actualItem, "value"), $"{prefix}The rejected promise has no property named value");
                Assert(Test262HostRuntimeIntrinsics.HasOwn(actualItem, "reason"), $"{prefix}The rejected promise has a property named reason");
                AssertSame(ObjectRuntime.GetItem(actualItem, "reason"), ObjectRuntime.GetItem(expectedItem, "reason"), $"{prefix}Reason value for item {i}");
            }
        }
    }

    private static void Assert(bool condition, string message)
    {
        if (!condition)
        {
            Test262HostRuntimeIntrinsics.ThrowAssertion(message);
        }
    }

    private static void AssertSame(object? actual, object? expected, string message)
    {
        if (!JavaScriptRuntime.Object.@is(actual, expected))
        {
            Test262HostRuntimeIntrinsics.ThrowAssertion(message);
        }
    }

    private static IEnumerable<object?> EnumerateArrayLike(object value)
    {
        var length = global::System.Math.Max(0, TypeUtilities.ToInt32(ObjectRuntime.GetItem(value, "length")));
        for (var i = 0; i < length; i++)
        {
            yield return ObjectRuntime.GetItem(value, (double)i);
        }
    }
}
