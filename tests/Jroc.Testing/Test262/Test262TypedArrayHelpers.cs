using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262TypedArrayHelpers
{
    private sealed record ArgumentFactory(string Name, string[] Features, Func<object?, object?> Convert);

    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("floatArrayConstructors", CreateFloatConstructors)
            .AddGlobalFactory("nonClampedIntArrayConstructors", CreateNonClampedIntConstructors)
            .AddGlobalFactory("intArrayConstructors", CreateIntConstructors)
            .AddGlobalFactory("bigIntArrayConstructors", CreateBigIntConstructors)
            .AddGlobalFactory("typedArrayConstructors", CreateTypedArrayConstructors)
            .AddGlobalFactory("allTypedArrayConstructors", CreateAllTypedArrayConstructors)
            .AddGlobalFactory("TypedArray", () => JavaScriptRuntime.Object.getPrototypeOf(Constructor(GlobalThis.Int8Array)))
            .AddGlobalFactory("typedArrayCtorArgFactories", CreateArgumentFactoryFunctions)
            .AddGlobalFactory("testWithAllTypedArrayConstructors", () => Function(TestWithAllConstructors, "testWithAllTypedArrayConstructors", 4))
            .AddGlobalFactory("testWithTypedArrayConstructors", () => Function(TestWithConstructors, "testWithTypedArrayConstructors", 4))
            .AddGlobalFactory("testWithBigIntTypedArrayConstructors", () => Function(TestWithBigIntConstructors, "testWithBigIntTypedArrayConstructors", 4))
            .AddGlobalFactory("nonAtomicsFriendlyTypedArrayConstructors", CreateNonAtomicsFriendlyConstructors)
            .AddGlobalFactory("testWithNonAtomicsFriendlyTypedArrayConstructors", () => Function(TestWithNonAtomicsFriendlyConstructors, "testWithNonAtomicsFriendlyTypedArrayConstructors", 1))
            .AddGlobalFactory("testWithAtomicsFriendlyTypedArrayConstructors", () => Function(TestWithAtomicsFriendlyConstructors, "testWithAtomicsFriendlyTypedArrayConstructors", 1))
            .AddGlobalFactory("testTypedArrayConversions", () => Function(TestTypedArrayConversions, "testTypedArrayConversions", 2))
            .AddGlobalFactory("isFloatTypedArrayConstructor", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object[], object?, bool>)IsFloatTypedArrayConstructor,
                "isFloatTypedArrayConstructor",
                1))
            .AddGlobalFactory("floatTypedArrayConstructorPrecision", () => Function(FloatTypedArrayConstructorPrecision, "floatTypedArrayConstructorPrecision", 1));
    }

    private static object? TestWithAllConstructors(object[] _, object?[]? args)
    {
        args ??= [];
        var callback = Argument(args, 0);
        var selected = Argument(args, 1);
        var constructors = TypeUtilities.ToBoolean(selected)
            ? EnumerateArrayLike(selected!).ToArray()
            : AllConstructors();
        var factories = SelectFactories(Argument(args, 2), Argument(args, 3));
        if (factories.Count == 0)
        {
            throw Test262HostRuntimeIntrinsics.CreateTest262Error("no arg factories match the requested filters");
        }

        foreach (var factory in factories)
        {
            foreach (var constructor in constructors)
            {
                var capturedConstructor = constructor;
                var boundFactory = Test262HostRuntimeIntrinsics.CreateFunction(
                    (Func<object?, object?>)(value => factory.ConvertFor(capturedConstructor, value)),
                    $"bound {factory.Name}",
                    1);
                Test262HostRuntimeIntrinsics.Invoke(callback, constructor, boundFactory);
            }
        }

        return null;
    }

    private static object? TestWithConstructors(object[] scopes, object?[]? args)
    {
        args ??= [];
        var forwarded = (object?[])args.Clone();
        if (forwarded.Length < 2)
        {
            global::System.Array.Resize(ref forwarded, 2);
        }

        if (!TypeUtilities.ToBoolean(forwarded[1]))
        {
            forwarded[1] = CreateTypedArrayConstructors();
        }
        return TestWithAllConstructors(scopes, forwarded);
    }

    private static object? TestWithBigIntConstructors(object[] scopes, object?[]? args)
    {
        args ??= [];
        var forwarded = (object?[])args.Clone();
        if (forwarded.Length < 2)
        {
            global::System.Array.Resize(ref forwarded, 2);
        }

        if (!TypeUtilities.ToBoolean(forwarded[1]))
        {
            forwarded[1] = CreateBigIntConstructors();
        }
        return TestWithAllConstructors(scopes, forwarded);
    }

    private static object? TestWithNonAtomicsFriendlyConstructors(object[] scopes, object?[]? args)
        => TestWithAllConstructors(
            scopes,
            [Argument(args ?? [], 0), CreateNonAtomicsFriendlyConstructors()]);

    private static object? TestWithAtomicsFriendlyConstructors(object[] scopes, object?[]? args)
        => TestWithAllConstructors(
            scopes,
            [Argument(args ?? [], 0), ArrayOf(AtomicsFriendlyConstructors())]);

    private static object? TestTypedArrayConversions(object[] _, object?[]? args)
    {
        args ??= [];
        var conversionValues = Argument(args, 0)!;
        var callback = Argument(args, 1);
        var values = ObjectRuntime.GetItem(conversionValues, "values");
        var expected = ObjectRuntime.GetItem(conversionValues, "expected");
        foreach (var constructor in TypedArrayConstructors())
        {
            var name = Test262HostRuntimeIntrinsics.ToMessage(ObjectRuntime.GetItem(constructor!, "name"));
            var expectedForType = ObjectRuntime.GetItem(expected, name[..^5]);
            var index = 0;
            foreach (var value in EnumerateArrayLike(values))
            {
                var expectedValue = ObjectRuntime.GetItem(expectedForType, (double)index++);
                var initial = JavaScriptRuntime.Object.@is(expectedValue, 0d) ? 1d : 0d;
                Test262HostRuntimeIntrinsics.Invoke(callback, constructor, value, expectedValue, initial);
            }
        }

        return null;
    }

    private static bool IsFloatTypedArrayConstructor(object[] _, object? value)
        => ReferenceEquals(value, Constructor(GlobalThis.Float64Array))
            || ReferenceEquals(value, Constructor(GlobalThis.Float32Array));

    private static string FloatTypedArrayConstructorPrecision(object[] _, object? value)
    {
        if (ReferenceEquals(value, Constructor(GlobalThis.Float32Array)))
        {
            return "single";
        }

        if (ReferenceEquals(value, Constructor(GlobalThis.Float64Array)))
        {
            return "double";
        }

        throw new Error("Malformed test - floatTypedArrayConstructorPrecision called with non-float TypedArray");
    }

    private static IReadOnlyList<ArgumentFactory> SelectFactories(object? include, object? exclude)
    {
        var factories = CreateArgumentFactories();
        if (TypeUtilities.ToBoolean(include))
        {
            var features = EnumerateArrayLike(include!)
                .Select(Test262HostRuntimeIntrinsics.ToMessage)
                .ToHashSet(StringComparer.Ordinal);
            factories = factories.Where(factory => factory.Features.Any(features.Contains)).ToList();
        }

        if (TypeUtilities.ToBoolean(exclude))
        {
            var features = EnumerateArrayLike(exclude!)
                .Select(Test262HostRuntimeIntrinsics.ToMessage)
                .ToHashSet(StringComparer.Ordinal);
            factories = factories.Where(factory => !factory.Features.Any(features.Contains)).ToList();
        }

        return factories;
    }

    private static List<ArgumentFactory> CreateArgumentFactories()
        =>
        [
            new("makePassthrough", ["passthrough"], value => value),
            new("makeArray", ["arraylike"], MakeArray),
            new("makeArrayLike", ["arraylike"], MakeArrayLike),
            new("makeIterable", ["iterable"], MakeArray),
            new("makeArrayBuffer", ["arraybuffer"], value => value)
        ];

    private static object? MakeArray(object? value)
    {
        if (TypeUtilities.IsPrimitive(value))
        {
            var number = TypeUtilities.ToNumber(value);
            if (!(number >= 0 && number < 9007199254740992d))
            {
                return value;
            }

            var length = checked((int)number);
            return new JavaScriptRuntime.Array(Enumerable.Repeat<object?>("0", length).ToArray());
        }

        return JavaScriptRuntime.Array.from(value, null, null);
    }

    private static object? MakeArrayLike(object? value)
    {
        var array = MakeArray(value);
        if (TypeUtilities.IsPrimitive(array))
        {
            return array;
        }

        var result = new JsObject();
        var values = EnumerateArrayLike(array!).ToArray();
        ObjectRuntime.SetItem(result, "length", (double)values.Length);
        for (var i = 0; i < values.Length; i++)
        {
            ObjectRuntime.SetItem(result, (double)i, values[i]);
        }

        return result;
    }

    private static object? ConvertFor(this ArgumentFactory factory, object? constructor, object? value)
    {
        var converted = factory.Convert(value);
        if (!string.Equals(factory.Name, "makeArrayBuffer", StringComparison.Ordinal)
            || TypeUtilities.IsPrimitive(converted))
        {
            return converted;
        }

        var typedArray = ObjectRuntime.ConstructValue(constructor!, new object[] { converted! });
        return ObjectRuntime.GetItem(typedArray!, "buffer");
    }

    private static object CreateArgumentFactoryFunctions()
        => ArrayOf(CreateArgumentFactories().Select(factory =>
            (object?)Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object?, object?, object?>)((constructor, value) => factory.ConvertFor(constructor, value)),
                factory.Name,
                2)));

    private static object CreateFloatConstructors() => ArrayOf(FloatConstructors());
    private static object CreateNonClampedIntConstructors() => ArrayOf(NonClampedIntConstructors());
    private static object CreateIntConstructors() => ArrayOf(IntConstructors());
    private static object CreateBigIntConstructors() => ArrayOf(BigIntConstructors());
    private static object CreateTypedArrayConstructors() => ArrayOf(TypedArrayConstructors());
    private static object CreateAllTypedArrayConstructors() => ArrayOf(AllConstructors());
    private static object CreateNonAtomicsFriendlyConstructors() => ArrayOf(NonAtomicsFriendlyConstructors());

    private static object?[] FloatConstructors() => [Constructor(GlobalThis.Float64Array), Constructor(GlobalThis.Float32Array)];
    private static object?[] NonClampedIntConstructors()
        => [Constructor(GlobalThis.Int32Array), Constructor(GlobalThis.Int16Array), Constructor(GlobalThis.Int8Array), Constructor(GlobalThis.Uint32Array), Constructor(GlobalThis.Uint16Array), Constructor(GlobalThis.Uint8Array)];
    private static object?[] IntConstructors() => [.. NonClampedIntConstructors(), Constructor(GlobalThis.Uint8ClampedArray)];
    private static object?[] BigIntConstructors() => [Constructor(GlobalThis.BigInt64Array), Constructor(GlobalThis.BigUint64Array)];
    private static object?[] TypedArrayConstructors() => [.. FloatConstructors(), .. IntConstructors()];
    private static object?[] AllConstructors() => [.. TypedArrayConstructors(), .. BigIntConstructors()];
    private static object?[] NonAtomicsFriendlyConstructors() => [.. FloatConstructors(), Constructor(GlobalThis.Uint8ClampedArray)];
    private static object?[] AtomicsFriendlyConstructors()
        => [Constructor(GlobalThis.Int32Array), Constructor(GlobalThis.Int16Array), Constructor(GlobalThis.Int8Array), Constructor(GlobalThis.Uint32Array), Constructor(GlobalThis.Uint16Array), Constructor(GlobalThis.Uint8Array)];

    private static object Constructor(Delegate value)
    {
        var adapter = BuiltinDelegateFunctionAdapter.FromDelegate(value);
        JavaScriptRuntime.Function.MarkConstructible(adapter);
        return adapter;
    }

    private static object ArrayOf(IEnumerable<object?> values)
        => new JavaScriptRuntime.Array(values.ToArray());

    private static IEnumerable<object?> EnumerateArrayLike(object value)
    {
        var length = global::System.Math.Max(0, TypeUtilities.ToInt32(ObjectRuntime.GetItem(value, "length")));
        for (var i = 0; i < length; i++)
        {
            yield return ObjectRuntime.GetItem(value, (double)i);
        }
    }

    private static object? Argument(object?[] args, int index)
        => index < args.Length ? args[index] : null;

    private static BuiltinDelegateFunctionAdapter Function(
        Func<object[], object?[]?, object?> function,
        string name,
        double length)
        => Test262HostRuntimeIntrinsics.CreateFunction(function, name, length);
}
