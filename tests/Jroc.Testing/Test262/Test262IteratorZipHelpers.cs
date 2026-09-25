using JavaScriptRuntime;
using JavaScriptRuntime.Node;

namespace Jroc.Tests;

internal static class Test262IteratorZipHelpers
{
    private static readonly AssertModule Assert = new();

    internal static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("forEachSequenceCombination", () => Function(
                (Action<object?>)ForEachSequenceCombination, "forEachSequenceCombination", 1))
            .AddGlobalFactory("forEachSequenceCombinationKeyed", () => Function(
                (Action<object?>)ForEachSequenceCombinationKeyed, "forEachSequenceCombinationKeyed", 1))
            .AddGlobalFactory("assertZipped", () => Function(
                (Action<object?, object?, object?, object?>)AssertZipped, "assertZipped", 4))
            .AddGlobalFactory("assertZippedKeyed", () => Function(
                (Action<object?, object?, object?, object?>)AssertZippedKeyed, "assertZippedKeyed", 4))
            .AddGlobalFactory("assertIteratorResult", () => Function(
                (Action<object?, object?, object?, object?>)AssertIteratorResult, "assertIteratorResult", 4))
            .AddGlobalFactory("assertIsPackedArray", () => Function(
                (Action<object?, object?>)AssertIsPackedArray, "assertIsPackedArray", 2));
    }

    private static BuiltinDelegateFunctionAdapter Function(Delegate callback, string name, double length)
        => Test262HostRuntimeIntrinsics.CreateFunction(callback, name, length);

    private static void ForEachSequenceCombination(object? callback)
        => EachCombination(inputs => CallSequence(callback, inputs));

    private static void ForEachSequenceCombinationKeyed(object? callback)
        => EachCombination(inputs =>
        {
            var keyed = new JsObject();
            for (var i = 0; i < inputs.Length; i++)
            {
                ObjectRuntime.SetItem(keyed, $"prop_{i}", inputs[i]);
            }
            CallSequence(callback, inputs, keyed);
        });

    private static void EachCombination(Action<JavaScriptRuntime.Array[]> test)
    {
        // Preserve the upstream nested-loop order, including the empty prefix at each depth.
        test([]);
        var strings = new[] { "abcd", "efgh", "ijkl" };
        for (var first = 0; first <= 4; first++)
        {
            test([Prefix(strings[0], first)]);
        }
        for (var first = 0; first <= 4; first++)
        for (var second = 0; second <= 4; second++)
        {
            test([Prefix(strings[0], first), Prefix(strings[1], second)]);
        }
        for (var first = 0; first <= 4; first++)
        for (var second = 0; second <= 4; second++)
        for (var third = 0; third <= 4; third++)
        {
            test([Prefix(strings[0], first), Prefix(strings[1], second), Prefix(strings[2], third)]);
        }
    }

    private static JavaScriptRuntime.Array Prefix(string source, int length)
        => new(source.Take(length).Select(ch => (object?)ch.ToString()));

    private static void CallSequence(object? callback, JavaScriptRuntime.Array[] inputs, object? keyed = null)
    {
        var argument = keyed ?? new JavaScriptRuntime.Array(inputs);
        var lengths = inputs.Select(input => (int)TypeUtilities.ToNumber(ObjectRuntime.GetItem(input, "length"))).ToArray();
        var min = lengths.Length == 0 ? 0 : lengths.Min();
        var max = lengths.Length == 0 ? 0 : lengths.Max();
        var label = "inputs = " + Test262HostRuntimeIntrinsics.ToMessage(JSON.Stringify(argument));
        CallableOperations.Call(callback, null, [argument, label, (double)min, (double)max]);
    }

    private static void AssertIteratorResult(object? result, object? value, object? done, object? label)
    {
        var prefix = Text(label);
        Assert.strictEqual(JavaScriptRuntime.Object.getPrototypeOf(result!), ObjectRuntime.GetItem(GlobalThis.Object, "prototype"),
            prefix + ": [[Prototype]] of iterator result is Object.prototype");
        Assert.ok(JavaScriptRuntime.Object.isExtensible(result!), prefix + ": iterator result is extensible");
        CompareArray(Reflect.ownKeys(result!), new JavaScriptRuntime.Array(new object?[] { "value", "done" }),
            prefix + ": iterator result properties");
        Verify(result!, "value", Descriptor(value, includeValue: true));
        Verify(result!, "done", Descriptor(done, includeValue: true));
    }

    private static void AssertIsPackedArray(object? array, object? label)
    {
        var prefix = Text(label);
        Assert.ok(JavaScriptRuntime.Array.isArray(array), prefix + ": array is an array exotic object");
        Assert.strictEqual(JavaScriptRuntime.Object.getPrototypeOf(array!), ObjectRuntime.GetItem(GlobalThis.Array, "prototype"),
            prefix + ": [[Prototype]] of array is Array.prototype");
        Assert.ok(JavaScriptRuntime.Object.isExtensible(array!), prefix + ": array is extensible");
        Verify(array!, "length", Descriptor(null, includeValue: false, enumerable: false, configurable: false));
        var length = TypeUtilities.ToNumber(ObjectRuntime.GetItem(array!, "length"));
        for (var i = 0; i < length; i++)
        {
            Verify(array!, (double)i, Descriptor(null, includeValue: false));
        }
    }

    private static void AssertIsNullProtoMutableObject(object? value, object? label)
    {
        var prefix = Text(label);
        Assert.strictEqual(JavaScriptRuntime.Object.getPrototypeOf(value!), JsNull.Null,
            prefix + ": [[Prototype]] of object is null");
        Assert.ok(JavaScriptRuntime.Object.isExtensible(value!), prefix + ": object is extensible");
        var keys = JavaScriptRuntime.Object.getOwnPropertyNames(value!);
        for (var i = 0; i < TypeUtilities.ToNumber(ObjectRuntime.GetItem(keys, "length")); i++)
        {
            Verify(value!, ObjectRuntime.GetItem(keys, (double)i)!, Descriptor(null, includeValue: false));
        }
    }

    private static void AssertZipped(object? zipped, object? inputs, object? count, object? label)
    {
        object? last = JsNull.Null;
        for (var i = 0; i < TypeUtilities.ToNumber(count); i++)
        {
            var itemLabel = Text(label) + ", step " + i;
            var result = ObjectRuntime.CallMember(zipped!, "next", []);
            var value = ObjectRuntime.GetItem(result!, "value");
            AssertIteratorResult(result, value, false, itemLabel);
            Assert.notStrictEqual(value, last, itemLabel + ": returns a new array");
            last = value;
            var expected = new JavaScriptRuntime.Array();
            for (var j = 0; j < TypeUtilities.ToNumber(ObjectRuntime.GetItem(inputs!, "length")); j++)
            {
                var input = ObjectRuntime.GetItem(inputs!, (double)j);
                ObjectRuntime.CallMember(expected, "push", [ObjectRuntime.GetItem(input!, (double)i)!]);
            }
            CompareArray(value, expected, itemLabel + ": values");
            AssertIsPackedArray(value, itemLabel);
        }
    }

    private static void AssertZippedKeyed(object? zipped, object? inputs, object? count, object? label)
    {
        object? last = JsNull.Null;
        var keys = JavaScriptRuntime.Object.keys(inputs!);
        for (var i = 0; i < TypeUtilities.ToNumber(count); i++)
        {
            var itemLabel = Text(label) + ", step " + i;
            var result = ObjectRuntime.CallMember(zipped!, "next", []);
            var value = ObjectRuntime.GetItem(result!, "value");
            AssertIteratorResult(result, value, false, itemLabel);
            Assert.notStrictEqual(value, last, itemLabel + ": returns a new object");
            last = value;
            CompareArray(Reflect.ownKeys(value!), keys, itemLabel + ": result object keys");
            var expected = new JavaScriptRuntime.Array();
            for (var j = 0; j < TypeUtilities.ToNumber(ObjectRuntime.GetItem(keys, "length")); j++)
            {
                var key = ObjectRuntime.GetItem(keys, (double)j);
                var input = ObjectRuntime.GetItem(inputs!, key!);
                ObjectRuntime.CallMember(expected, "push", [ObjectRuntime.GetItem(input!, (double)i)!]);
            }
            CompareArray(JavaScriptRuntime.Object.values(value!), expected, itemLabel + ": result object values");
            AssertIsNullProtoMutableObject(value, itemLabel);
        }
    }

    private static void CompareArray(object? actual, object expected, string label)
    {
        var actualLength = TypeUtilities.ToNumber(ObjectRuntime.GetItem(actual!, "length"));
        var expectedLength = TypeUtilities.ToNumber(ObjectRuntime.GetItem(expected, "length"));
        Assert.strictEqual(actualLength, expectedLength, label);
        for (var i = 0; i < expectedLength; i++)
        {
            Assert.strictEqual(ObjectRuntime.GetItem(actual!, (double)i), ObjectRuntime.GetItem(expected, (double)i), label);
        }
    }

    private static JsObject Descriptor(object? value, bool includeValue, bool enumerable = true, bool configurable = true)
    {
        var descriptor = new JsObject();
        if (includeValue)
        {
            ObjectRuntime.SetItem(descriptor, "value", value);
        }
        ObjectRuntime.SetItem(descriptor, "writable", true);
        ObjectRuntime.SetItem(descriptor, "enumerable", enumerable);
        ObjectRuntime.SetItem(descriptor, "configurable", configurable);
        return descriptor;
    }

    private static void Verify(object target, object name, object descriptor)
        => Test262PropertyHelpers.VerifyProperty([], [target, name, descriptor]);

    private static string Text(object? value)
        => value is null ? "undefined" : DotNet2JSConversions.ToString(value);
}
