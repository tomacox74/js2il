using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262ResizableArrayBufferHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("ctors", CreateConstructors)
            .AddGlobalFactory(
                "CreateResizableArrayBuffer",
                () => Function(CreateResizableArrayBuffer, "CreateResizableArrayBuffer", 2))
            .AddGlobalFactory(
                "MayNeedBigInt",
                () => Function(MayNeedBigInt, "MayNeedBigInt", 2))
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
            Constructor(GlobalThis.Uint8ClampedArray)
        });

    private static object? CreateResizableArrayBuffer(object[] _, object?[]? args)
    {
        var options = new JsObject();
        ObjectRuntime.SetItem(options, "maxByteLength", Argument(args, 1));
        return new ArrayBuffer(Argument(args, 0), options);
    }

    private static object? MayNeedBigInt(object[] _, object?[]? args)
        => Argument(args, 1);

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

        if (expected is null || expected is JsNull)
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
            var actual = TypeUtilities.ToNumber(values[index]);
            var expectedValue = ObjectRuntime.GetItem(expected, (double)index);
            if (!JavaScriptRuntime.Object.@is(actual, expectedValue))
            {
                throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                    "TestIterationAndResize: list of iterated values differs from expected values");
            }
        }

        return null;
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
