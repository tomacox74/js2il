using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262AtomicsHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("testWithAtomicsOutOfBoundsIndices", () => Function(
                TestWithOutOfBoundsIndices,
                "testWithAtomicsOutOfBoundsIndices"))
            .AddGlobalFactory("testWithAtomicsInBoundsIndices", () => Function(
                TestWithInBoundsIndices,
                "testWithAtomicsInBoundsIndices"))
            .AddGlobalFactory("testWithAtomicsNonViewValues", () => Function(
                TestWithNonViewValues,
                "testWithAtomicsNonViewValues"));
    }

    private static object? TestWithOutOfBoundsIndices(object[] _, object? callback)
    {
        object?[] generators =
        [
            IndexGenerator("negativeOne", _ => -1d),
            IndexGenerator("length", view => ObjectRuntime.GetItem(view!, "length")),
            IndexGenerator("twiceLength", view => TypeUtilities.ToNumber(ObjectRuntime.GetItem(view!, "length")) * 2),
            IndexGenerator("positiveInfinity", _ => double.PositiveInfinity),
            IndexGenerator("negativeInfinity", _ => double.NegativeInfinity),
            IndexGenerator("valueOf125", _ => CoercibleIndex("valueOf", 125d)),
            IndexGenerator("toString125", _ => CoercibleIndex("toString", "125"))
        ];

        InvokeForEach(callback, generators);
        return null;
    }

    private static object? TestWithInBoundsIndices(object[] _, object? callback)
    {
        object?[] generators =
        [
            IndexGenerator("negativeZero", _ => -0d),
            IndexGenerator("stringNegativeZero", _ => "-0"),
            IndexGenerator("undefined", _ => null),
            IndexGenerator("nan", _ => double.NaN),
            IndexGenerator("fraction", _ => 0.5d),
            IndexGenerator("stringFraction", _ => "0.5"),
            IndexGenerator("negativeFraction", _ => -0.9d),
            IndexGenerator("ordinaryObject", _ => new JsObject()),
            IndexGenerator("lastIndex", view => TypeUtilities.ToNumber(ObjectRuntime.GetItem(view!, "length")) - 1),
            IndexGenerator("valueOfZero", _ => CoercibleIndex("valueOf", 0d)),
            IndexGenerator("toStringZero", _ => CoercibleIndex("toString", "0"))
        ];

        InvokeForEach(callback, generators);
        return null;
    }

    private static object? TestWithNonViewValues(object[] _, object? callback)
    {
        object?[] values =
        [
            JsNull.Null,
            null,
            true,
            false,
            new JsObject(),
            10d,
            3.14d,
            new JsObject(),
            "Hi there",
            new JavaScriptRuntime.Date(),
            new JsObject(),
            new JsObject(),
            new DataView(new ArrayBuffer(10d)),
            new ArrayBuffer(128d),
            new SharedArrayBuffer(128d),
            new Error("Ouch"),
            new JavaScriptRuntime.Array(new object?[] { 1d, 1d, 2d, 3d, 5d, 8d }),
            Test262HostRuntimeIntrinsics.CreateFunction((Func<object?, double>)(value => -TypeUtilities.ToNumber(value)), string.Empty, 1),
            new Symbol("halleluja"),
            GlobalThis.Object,
            GlobalThis.Int32Array,
            GlobalThis.Date,
            GlobalThis.Math,
            GlobalThis.Atomics
        ];

        InvokeForEach(callback, values);
        return null;
    }

    private static void InvokeForEach(object? callback, IEnumerable<object?> values)
    {
        foreach (var value in values)
        {
            Test262HostRuntimeIntrinsics.Invoke(callback, value);
        }
    }

    private static object CoercibleIndex(string methodName, object? result)
    {
        var value = new JsObject();
        ObjectRuntime.SetItem(
            value,
            methodName,
            Test262HostRuntimeIntrinsics.CreateFunction((Func<object?>)(() => result), methodName, 0));
        if (string.Equals(methodName, "toString", StringComparison.Ordinal))
        {
            ObjectRuntime.SetItem(value, "valueOf", false);
        }

        return value;
    }

    private static object IndexGenerator(string name, Func<object?, object?> generator)
        => Test262HostRuntimeIntrinsics.CreateFunction(generator, name, 1);

    private static BuiltinDelegateFunctionAdapter Function(Func<object[], object?, object?> function, string name)
        => Test262HostRuntimeIntrinsics.CreateFunction(function, name, 1);
}
