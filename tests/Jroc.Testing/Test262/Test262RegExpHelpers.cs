using JavaScriptRuntime;
using System.Numerics;
using System.Text;

namespace Jroc.Tests;

internal static class Test262RegExpHelpers
{
    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder.AddGlobalFactory("buildString", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Func<object?, string>)BuildString,
            "buildString",
            1));
        builder.AddGlobalFactory("testPropertyEscapes", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Action<object?, object?, object?>)TestPropertyEscapes,
            "testPropertyEscapes",
            3));
        builder.AddGlobalFactory("testPropertyOfStrings", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Action<object?>)TestPropertyOfStrings,
            "testPropertyOfStrings",
            1));
        builder.AddGlobalFactory("testExtendedCharacterClass",
            () => ObjectRuntime.GetItem(GlobalThis.GetGlobalThis(), "testPropertyOfStrings"));
        builder.AddGlobalFactory("matchValidator", () => Test262HostRuntimeIntrinsics.CreateFunction(
            (Func<object?, object?, object?, object?>)MatchValidator,
            "matchValidator",
            3));
    }

    private static string BuildString(object? arguments)
    {
        var loneCodePoints = ObjectRuntime.GetItem(arguments!, "loneCodePoints");
        var ranges = ObjectRuntime.GetItem(arguments!, "ranges");
        var initial = JavaScriptRuntime.Function.Apply(
            Test262HostRuntimeIntrinsics.CreateFunction(
                (Func<object[], object?[], object?>)(static (_, values) => JavaScriptRuntime.String.FromCodePoint(values)),
                "fromCodePoint",
                1),
            null,
            loneCodePoints);
        var result = new StringBuilder((string)initial!);
        for (double index = 0; Operators.LessThan(index, ObjectRuntime.GetItem(ranges!, "length")); index++)
        {
            var range = ObjectRuntime.GetItem(ranges!, index);
            var start = ObjectRuntime.GetItem(range!, 0d);
            var end = ObjectRuntime.GetItem(range!, 1d);
            if (start is double first && end is double last
                && first >= 0 && first <= last && last <= 0x10FFFF
                && first == global::System.Math.Truncate(first))
            {
                for (var codePoint = (int)first; codePoint <= last; codePoint++)
                {
                    if (codePoint <= 0xFFFF)
                    {
                        result.Append((char)codePoint);
                    }
                    else
                    {
                        var scalar = codePoint - 0x10000;
                        result.Append((char)(0xD800 + (scalar >> 10)));
                        result.Append((char)(0xDC00 + (scalar & 0x3FF)));
                    }
                }
                continue;
            }

            var points = new List<object?>();
            for (var codePoint = start; Operators.LessThanOrEqual(codePoint, end);)
            {
                points.Add(codePoint);
                if (points.Count == 10000)
                {
                    result.Append(JavaScriptRuntime.String.FromCodePoint(points.ToArray()));
                    points.Clear();
                }
                var numeric = TypeUtilities.ToNumeric(codePoint);
                codePoint = numeric is BigInteger bigInt
                    ? bigInt + BigInteger.One
                    : (double)numeric + 1d;
            }
            result.Append(JavaScriptRuntime.String.FromCodePoint(points.ToArray()));
        }
        return result.ToString();
    }

    private static void TestPropertyEscapes(object? regExp, object? value, object? expression)
    {
        if (TypeUtilities.ToBoolean(ObjectRuntime.CallMember(regExp!, "test", [value!])))
        {
            return;
        }

        var iterator = ObjectRuntime.GetIterator(value);
        while (true)
        {
            var next = iterator.Next();
            if (next.done)
            {
                return;
            }
            try
            {
                var codePoint = ObjectRuntime.CallMember(next.value!, "codePointAt", [0d]);
                var hex = ObjectRuntime.CallMember(codePoint!, "toString", [16d]);
                var upper = ObjectRuntime.CallMember(hex!, "toUpperCase", []);
                var padded = ObjectRuntime.CallMember(upper!, "padStart", [6d, "0"]);
                var formatted = "U+" + DotNet2JSConversions.ToString(padded);
                var matches = TypeUtilities.ToBoolean(ObjectRuntime.CallMember(regExp!, "test", [next.value!]));
                var message = $"`{DotNet2JSConversions.ToString(expression)}` should match {formatted} (`{DotNet2JSConversions.ToString(next.value)}`)";
                if (!matches)
                {
                    Test262HostRuntimeIntrinsics.ThrowAssertion(null, message);
                }
            }
            catch
            {
                ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                throw;
            }
        }
    }

    private static void TestPropertyOfStrings(object? arguments)
    {
        var regExp = ObjectRuntime.GetItem(arguments!, "regExp");
        var expression = ObjectRuntime.GetItem(arguments!, "expression");
        var matches = ObjectRuntime.GetItem(arguments!, "matchStrings");
        var nonMatches = ObjectRuntime.GetItem(arguments!, "nonMatchStrings");
        CheckStrings(matches, true);
        if (TypeUtilities.ToBoolean(nonMatches)) CheckStrings(nonMatches, false);

        void CheckStrings(object? strings, bool expected)
        {
            var joined = ObjectRuntime.CallMember(strings!, "join", [""]);
            if (TypeUtilities.ToBoolean(ObjectRuntime.CallMember(regExp!, "test", [joined!])) == expected) return;
            var iterator = ObjectRuntime.GetIterator(strings);
            while (true)
            {
                var next = iterator.Next();
                if (next.done) return;
                try
                {
                    var matched = TypeUtilities.ToBoolean(ObjectRuntime.CallMember(regExp!, "test", [next.value!]));
                    var text = DotNet2JSConversions.ToString(expression);
                    var value = DotNet2JSConversions.ToString(next.value);
                    var points = new List<string>();
                    var symbols = ObjectRuntime.GetIterator(next.value);
                    while (true)
                    {
                        var symbol = symbols.Next();
                        if (symbol.done) break;
                        try
                        {
                            var point = ObjectRuntime.CallMember(symbol.value!, "codePointAt", [0d]);
                            var hex = ObjectRuntime.CallMember(point!, "toString", [16d]);
                            var upper = ObjectRuntime.CallMember(hex!, "toUpperCase", []);
                            var padded = ObjectRuntime.CallMember(upper!, "padStart", [6d, "0"]);
                            points.Add("U+" + DotNet2JSConversions.ToString(padded));
                        }
                        catch
                        {
                            ObjectRuntime.IteratorCloseForThrowCompletion(symbols);
                            throw;
                        }
                    }
                    if (matched != expected)
                    {
                        Test262HostRuntimeIntrinsics.ThrowAssertion(null,
                            $"`{text}` should {(expected ? "" : "not ")}match {value} ({string.Join(" ", points)})");
                    }
                }
                catch
                {
                    ObjectRuntime.IteratorCloseForThrowCompletion(iterator);
                    throw;
                }
            }
        }
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
