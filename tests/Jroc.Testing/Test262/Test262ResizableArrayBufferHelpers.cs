using JavaScriptRuntime;
using System.Numerics;

namespace Jroc.Tests;

internal static class Test262ResizableArrayBufferHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("floatCtors", CreateFloatConstructors)
            .AddGlobalFactory("ctors", CreateConstructors)
            .AddGlobalFactory(
                "CreateResizableArrayBuffer",
                () => Function(CreateResizableArrayBuffer, "CreateResizableArrayBuffer", 2))
            .AddGlobalFactory(
                "MayNeedBigInt",
                () => Function(MayNeedBigInt, "MayNeedBigInt", 2))
            .AddGlobalFactory(
                "Convert",
                () => Function(Convert, "Convert", 1))
            .AddGlobalFactory(
                "ToNumbers",
                () => Function(ToNumbers, "ToNumbers", 1))
            .AddGlobalFactory(
                "CreateRabForTest",
                () => Function(CreateRabForTest, "CreateRabForTest", 1))
            .AddGlobalFactory(
                "CollectValuesAndResize",
                () => Function(CollectValuesAndResize, "CollectValuesAndResize", 5))
            .AddGlobalFactory(
                "TestIterationAndResize",
                () => Function(TestIterationAndResize, "TestIterationAndResize", 5));
    }

    private static object CreateConstructors()
        => new JavaScriptRuntime.Array(
        new object?[]
        {
            Constructor(GlobalThis.Uint8Array),
            Constructor(GlobalThis.Int8Array),
            Constructor(GlobalThis.Uint16Array),
            Constructor(GlobalThis.Int16Array),
            Constructor(GlobalThis.Uint32Array),
            Constructor(GlobalThis.Int32Array),
            Constructor(GlobalThis.Float32Array),
            Constructor(GlobalThis.Float64Array),
            Constructor(GlobalThis.Uint8ClampedArray),
            Constructor(GlobalThis.BigUint64Array),
            Constructor(GlobalThis.BigInt64Array)
        });

    private static object CreateFloatConstructors()
        => new JavaScriptRuntime.Array(
        new object?[]
        {
            Constructor(GlobalThis.Float32Array),
            Constructor(GlobalThis.Float64Array)
        });

    private static object? CreateResizableArrayBuffer(object[] _, object?[]? args)
    {
        var options = new JsObject();
        ObjectRuntime.SetItem(options, "maxByteLength", Argument(args, 1));
        return new ArrayBuffer(Argument(args, 0), options);
    }

    private static object? MayNeedBigInt(object[] _, object?[]? args)
    {
        var typedArray = Argument(args, 0);
        var value = Argument(args, 1);
        return typedArray is BigInt64Array or BigUint64Array
            ? new BigInteger(TypeUtilities.ToNumber(value))
            : value;
    }

    private static object? Convert(object[] _, object?[]? args)
    {
        var value = Argument(args, 0);
        return value is BigInteger bigint ? (double)bigint : value;
    }

    private static object ToNumbers(object[] _, object?[]? args)
    {
        var source = Argument(args, 0);
        var result = new JavaScriptRuntime.Array();
        var length = TypeUtilities.ToInt32(ObjectRuntime.GetItem(source!, "length"));
        for (var index = 0; index < length; index++)
        {
            var value = ObjectRuntime.GetItem(source!, (double)index);
            result.push(value is BigInteger bigint ? (double)bigint : value);
        }

        return result;
    }

    private static object CreateRabForTest(object[] _, object?[]? args)
    {
        var constructor = Argument(args, 0)
            ?? throw Test262HostRuntimeIntrinsics.CreateTest262Error("constructor is required");
        var bytesPerElement = TypeUtilities.ToInt32(ObjectRuntime.GetItem(constructor, "BYTES_PER_ELEMENT"));
        var buffer = new ArrayBuffer(4d * bytesPerElement, CreateMaxByteLengthOptions(8 * bytesPerElement));
        var typedArray = ObjectRuntime.ConstructValue(constructor, new object[] { buffer });
        for (var index = 0; index < 4; index++)
        {
            ObjectRuntime.SetItem(
                typedArray!,
                (double)index,
                typedArray is BigInt64Array or BigUint64Array
                    ? new BigInteger(2 * index)
                    : 2d * index);
        }

        return buffer;
    }

    private static object? CollectValuesAndResize(object[] _, object?[]? args)
    {
        var value = Argument(args, 0);
        var values = Argument(args, 1)
            ?? throw Test262HostRuntimeIntrinsics.CreateTest262Error("values array is required");
        if (Argument(args, 2) is not ArrayBuffer buffer)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error("resizable ArrayBuffer is required");
        }

        var length = TypeUtilities.ToInt32(ObjectRuntime.GetItem(values, "length"));
        ObjectRuntime.SetItem(values, (double)length, value is BigInteger bigint ? (double)bigint : value);
        if (length + 1 == TypeUtilities.ToInt32(Argument(args, 3)))
        {
            buffer.resize(Argument(args, 4));
        }

        return true;
    }

    private static object? TestIterationAndResize(object[] _, object?[]? args)
    {
        var iterable = Argument(args, 0)
            ?? throw Test262HostRuntimeIntrinsics.CreateTest262Error("iterable is required");
        var expected = Argument(args, 1);
        if (Argument(args, 2) is not ArrayBuffer buffer)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error("resizable ArrayBuffer is required");
        }

        var resizeAfter = TypeUtilities.ToInt32(Argument(args, 3));
        var resizeTo = Argument(args, 4);
        var values = new List<object?>();
        var iterator = ObjectRuntime.GetIterator(iterable);
        var resized = false;

        while (true)
        {
            var next = iterator.Next();
            if (next.done)
            {
                break;
            }

            values.Add(next.value);
            if (!resized && values.Count == resizeAfter)
            {
                buffer.resize(resizeTo);
                resized = true;
            }
        }

        if (!resized)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                "TestIterationAndResize: resize condition should have been hit");
        }

        if (expected is null or JsNull)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                "TestIterationAndResize: expected an abrupt completion");
        }

        var expectedLength = TypeUtilities.ToInt32(ObjectRuntime.GetItem(expected, "length"));
        if (values.Count != expectedLength)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                "TestIterationAndResize: list of iterated values has an unexpected length");
        }

        for (var index = 0; index < values.Count; index++)
        {
            var expectedValue = ObjectRuntime.GetItem(expected, (double)index);
            if (!ValuesEqual(values[index], expectedValue))
            {
                throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                    "TestIterationAndResize: list of iterated values differs from expected values");
            }
        }

        return null;
    }

    private static bool ValuesEqual(object? actual, object? expected)
    {
        if (actual is IJavaScriptArray)
        {
            if (expected is not IJavaScriptArray)
            {
                return false;
            }

            var actualKey = ObjectRuntime.GetItem(actual, 0d);
            var actualValue = ObjectRuntime.GetItem(actual, 1d);
            return JavaScriptRuntime.Object.@is(actualKey, ObjectRuntime.GetItem(expected, 0d))
                && JavaScriptRuntime.Object.@is(
                    actualValue is BigInteger bigint ? (double)bigint : actualValue,
                    ObjectRuntime.GetItem(expected, 1d));
        }

        return JavaScriptRuntime.Object.@is(
            actual is BigInteger bigintValue ? (double)bigintValue : actual,
            expected);
    }

    private static JsObject CreateMaxByteLengthOptions(int maxByteLength)
    {
        var options = new JsObject();
        ObjectRuntime.SetItem(options, "maxByteLength", (double)maxByteLength);
        return options;
    }

    private static object Constructor(Delegate value)
    {
        var adapter = BuiltinDelegateFunctionAdapter.FromDelegate(value);
        JavaScriptRuntime.Function.MarkConstructible(adapter);
        return adapter;
    }

    private static object? Argument(object?[]? args, int index)
        => args != null && args.Length > index ? args[index] : null;

    private static BuiltinDelegateFunctionAdapter Function(
        Func<object[], object?[]?, object?> function,
        string name,
        double length)
        => Test262HostRuntimeIntrinsics.CreateFunction(function, name, length);
}
