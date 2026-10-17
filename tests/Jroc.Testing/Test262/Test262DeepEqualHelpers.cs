using JavaScriptRuntime;
using JavaScriptRuntime.Node;

namespace Jroc.Tests;

internal static class Test262DeepEqualHelpers
{
    internal static void Register(AssertModule assert)
    {
        var deepEqual = Test262HostRuntimeIntrinsics.CreateFunction(
            (Action<object?, object?, object?>)((actual, expected, message) =>
                assert.ok(Compare(actual, expected), message)), "deepEqual", 3);
        ObjectRuntime.SetItem(deepEqual, "_compare", Test262HostRuntimeIntrinsics.CreateFunction(
            (Func<object?, object?, bool>)Compare, "deepEqual", 2));
        ObjectRuntime.SetItem(assert, "deepEqual", deepEqual);
    }

    private static bool IsBoxed(object value)
        => Operators.InstanceOf(value, ObjectRuntime.GetItem(GlobalThis.globalThis, "String"))
            || Operators.InstanceOf(value, ObjectRuntime.GetItem(GlobalThis.globalThis, "Number"))
            || Operators.InstanceOf(value, ObjectRuntime.GetItem(GlobalThis.globalThis, "Boolean"))
            || TypeUtilities.Typeof(ObjectRuntime.GetItem(GlobalThis.globalThis, "Symbol")) == "function"
                && Operators.InstanceOf(value, ObjectRuntime.GetItem(GlobalThis.globalThis, "Symbol"))
            || TypeUtilities.Typeof(ObjectRuntime.GetItem(GlobalThis.globalThis, "BigInt")) == "function"
                && Operators.InstanceOf(value, ObjectRuntime.GetItem(GlobalThis.globalThis, "BigInt"));

    private static bool IsArrayLike(object value)
        => value is JavaScriptRuntime.Array or TypedArrayBase;

    private static bool IsStructural(object value)
        => value is not (Promise or WeakMap or WeakSet or Map or Set);

    private static List<string> EnumerableKeys(object value)
    {
        var keys = new List<string>();
        var iterator = ObjectRuntime.CreateForInIterator(value);
        for (var next = iterator.Next(); !next.done; next = iterator.Next())
            keys.Add((string)next.value!);
        keys.Sort(StringComparer.Ordinal);
        return keys;
    }

    internal static bool Compare(object? actual, object? expected)
    {
        var cache = new Dictionary<object, HashSet<object>>(ReferenceEqualityComparer.Instance);
        bool Equal(object? left, object? right)
        {
            if (left is not null && !TypeUtilities.IsPrimitive(left) && IsBoxed(left))
                left = ObjectRuntime.CallMember0(left, "valueOf");
            if (right is not null && !TypeUtilities.IsPrimitive(right) && IsBoxed(right))
                right = ObjectRuntime.CallMember0(right, "valueOf");
            if (Operators.StrictEqual(left, right)
                || left is double leftNumber && right is double rightNumber
                    && double.IsNaN(leftNumber) && double.IsNaN(rightNumber))
                return true;
            if (left is null || right is null || TypeUtilities.IsPrimitive(left) || TypeUtilities.IsPrimitive(right))
                return false;
            if (!cache.TryGetValue(left, out var seen))
                cache.Add(left, seen = new HashSet<object>(ReferenceEqualityComparer.Instance));
            if (!seen.Add(right))
                return true;
            var leftTag = ObjectRuntime.HasPropertyIn(Symbol.toStringTag, left)
                ? ObjectRuntime.GetItem(left, Symbol.toStringTag) : null;
            var rightTag = ObjectRuntime.HasPropertyIn(Symbol.toStringTag, right)
                ? ObjectRuntime.GetItem(right, Symbol.toStringTag) : null;
            if (!Operators.StrictEqual(leftTag, rightTag))
                return false;
            if (left is Date || right is Date)
                return left is Date && right is Date
                    && Equal(ObjectRuntime.CallMember0(left, "valueOf"), ObjectRuntime.CallMember0(right, "valueOf"));
            if (left is RegExp || right is RegExp)
                return left is RegExp && right is RegExp
                    && Equal(ObjectRuntime.CallMember0(left, "toString"), ObjectRuntime.CallMember0(right, "toString"));
            if (IsArrayLike(left) || IsArrayLike(right))
            {
                if (!IsArrayLike(left) || !IsArrayLike(right))
                    return false;
                if (!Operators.StrictEqual(ObjectRuntime.GetItem(left, "length"), ObjectRuntime.GetItem(right, "length")))
                    return false;
                for (var index = 0d; index < TypeUtilities.ToNumber(ObjectRuntime.GetItem(left, "length")); index++)
                    if (!Equal(ObjectRuntime.GetItem(left, index), ObjectRuntime.GetItem(right, index)))
                        return false;
                return true;
            }
            if (IsStructural(left) != IsStructural(right))
                return false;
            if (IsStructural(left))
            {
                var leftKeys = EnumerableKeys(left);
                var rightKeys = EnumerableKeys(right);
                if (!leftKeys.SequenceEqual(rightKeys, StringComparer.Ordinal)
                    || leftKeys.Any(key => !Equal(ObjectRuntime.GetItem(left, key), ObjectRuntime.GetItem(right, key))))
                    return false;
            }
            var leftIterable = CallableOperations.IsCallable(ObjectRuntime.GetItem(left, Symbol.iterator));
            var rightIterable = CallableOperations.IsCallable(ObjectRuntime.GetItem(right, Symbol.iterator));
            if (!leftIterable || !rightIterable)
                return leftIterable == rightIterable && IsStructural(left);
            if ((left is Map && right is Map || left is Set && right is Set)
                && !Operators.StrictEqual(ObjectRuntime.GetItem(left, "size"), ObjectRuntime.GetItem(right, "size")))
                return false;
            var leftIterator = ObjectRuntime.GetIterator(left);
            var rightIterator = ObjectRuntime.GetIterator(right);
            while (true)
            {
                var first = leftIterator.Next();
                var second = rightIterator.Next();
                if (first.done && second.done)
                    return true;
                if (first.done || second.done || !Equal(first.value, second.value))
                {
                    if (!first.done && leftIterator.HasReturn)
                        leftIterator.Return();
                    if (!second.done && rightIterator.HasReturn)
                        rightIterator.Return();
                    return false;
                }
            }
        }
        return Equal(actual, expected);
    }
}
