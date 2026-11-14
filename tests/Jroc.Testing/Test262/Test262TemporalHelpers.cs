using JavaScriptRuntime;
using JavaScriptRuntime.Node;
using System.Numerics;
using System.Text.RegularExpressions;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// The observation helpers from the pinned temporalHelpers.js are also used by Array.fromAsync.
internal static class Test262TemporalHelpers
{
    private static readonly AssertModule Assert = new();

    internal static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
        => builder.AddGlobalFactory("TemporalHelpers", CreateHelpers);

    private static object CreateHelpers()
    {
        var helpers = new JsObject();
        ObjectRuntime.SetItem(helpers, "observeProperty", CreateFunction(
            (Action<object?, object?, object?, object?, object?>)ObserveProperty, "observeProperty", 4));
        ObjectRuntime.SetItem(helpers, "observeMethod", CreateFunction(
            (Action<object?, object?, object?, object?>)ObserveMethod, "observeMethod", 3));
        ObjectRuntime.SetItem(helpers, "toPrimitiveObserver", CreateFunction(
            (Func<object?, object?, object?, object>)ToPrimitiveObserver, "toPrimitiveObserver", 3));
        ObjectRuntime.SetItem(helpers, "propertyBagObserver", CreateFunction(
            (Func<object?, object?, object?, object?, object>)((calls, bag, name, skip) =>
                PropertyBagObserver(helpers, calls, bag, name, skip)), "propertyBagObserver", 4));
        ObjectRuntime.SetItem(helpers, "assertPlainTime", CreateFunction(
            (Action<object?, object?, object?, object?, object?, object?, object?, object?>)AssertPlainTime,
            "assertPlainTime", 7));
        ObjectRuntime.SetItem(helpers, "assertPlainTimesEqual", CreateFunction(
            (Action<object?, object?, object?>)AssertPlainTimesEqual, "assertPlainTimesEqual", 2));
        ObjectRuntime.SetItem(helpers, "checkStringOptionWrongType", CreateFunction(
            (Action<object?, object?, object?, object?>)((name, value, check, assertion) =>
                CheckStringOptionWrongType(helpers, name, value, check, assertion)),
            "checkStringOptionWrongType", 4));
        ObjectRuntime.SetItem(helpers, "checkRoundingIncrementOptionWrongType", CreateFunction(
            (Action<object?, object?, object?>)((check, trueAssertion, objectAssertion) =>
                CheckRoundingIncrementOptionWrongType(helpers, check, trueAssertion, objectAssertion)),
            "checkRoundingIncrementOptionWrongType", 3));
        ObjectRuntime.SetItem(helpers, "checkPluralUnitsAccepted", CreateFunction(
            (Action<object?, object?>)((callback, units) => CheckPluralUnitsAccepted(helpers, callback, units)),
            "checkPluralUnitsAccepted", 2));
        var iso = new JsObject();
        ObjectRuntime.SetItem(iso, "plainTimeStringsAmbiguous", CreateFunction(
            (Func<object>)PlainTimeStringsAmbiguous, "plainTimeStringsAmbiguous", 0));
        ObjectRuntime.SetItem(iso, "plainTimeStringsUnambiguous", CreateFunction(
            (Func<object>)PlainTimeStringsUnambiguous, "plainTimeStringsUnambiguous", 0));
        ObjectRuntime.SetItem(helpers, "ISO", iso);
        return helpers;
    }

    private static bool IsPlainTime(object? value)
    {
        var temporal = ObjectRuntime.GetItem(GlobalThis.globalThis, "Temporal");
        return Operators.InstanceOf(value, ObjectRuntime.GetItem(temporal!, "PlainTime"));
    }

    private static string DescriptionPrefix(object? description)
        => TypeUtilities.ToBoolean(description) ? StringValue(description) + ": " : "";

    private static void AssertPlainTime(object? time, object? hour, object? minute, object? second,
        object? millisecond, object? microsecond, object? nanosecond, object? description)
    {
        var prefix = DescriptionPrefix(description);
        Assert.strictEqual(IsPlainTime(time), true, prefix + "instanceof");
        var fields = new[] { "hour", "minute", "second", "millisecond", "microsecond", "nanosecond" };
        object?[] expected = [hour, minute, second, millisecond, microsecond, nanosecond];
        for (var i = 0; i < fields.Length; i++)
        {
            Assert.strictEqual(ObjectRuntime.GetItem(time!, fields[i]), expected[i],
                prefix + fields[i] + " result:");
        }
    }

    private static void AssertPlainTimesEqual(object? actual, object? expected, object? description)
    {
        var prefix = DescriptionPrefix(description);
        Assert.strictEqual(IsPlainTime(expected), true, prefix + "expected value should be a Temporal.PlainTime");
        Assert.strictEqual(IsPlainTime(actual), true, prefix + "instanceof");
        Assert.strictEqual(ObjectRuntime.CallMember(actual!, "equals", [expected!]), true, prefix + "equals method");
    }

    private static object PlainTimeStringsAmbiguous()
    {
        string[] original =
        [
            "2021-12", "2021-12[-12:00]", "1214", "0229", "1130", "12-14",
            "12-14[-14:00]", "202112", "202112[UTC]"
        ];
        return new JavaScriptRuntime.Array(original.Cast<object?>()
            .Concat(original.Select(value => (object?)(value + "[u-ca=iso8601]"))).ToArray());
    }

    private static object PlainTimeStringsUnambiguous()
        => new JavaScriptRuntime.Array(new object?[]
        {
            "2021-13", "202113", "2021-13[-13:00]", "202113[-13:00]",
            "0000-00", "000000", "0000-00[UTC]", "000000[UTC]",
            "1314", "13-14", "1232", "0230", "0631", "0000", "00-00"
        });

    private static object? Call(object? callback, params object?[] arguments)
        => CallableOperations.Call(callback, null, arguments);

    private static void CheckThrows(object? callback, object? value, string error, string description)
    {
        var expected = ObjectRuntime.GetItem(GlobalThis.globalThis, error);
        var check = CreateFunction((Func<object?>)(() => Call(callback, value)), "", 0);
        Assert.throws(check, expected, description);
    }

    private static object PlainObject()
    {
        // {} in the upstream matrix has Object.prototype conversions. A bare
        // host JsObject must not turn that RangeError partition into TypeError.
        var result = new JsObject();
        JavaScriptRuntime.Object.setPrototypeOf(result, ObjectRuntime.GetItem(GlobalThis.Object, "prototype"));
        return result;
    }

    private static void AssertConversionTrace(object calls, string name, string method)
    {
        Assert.strictEqual(ObjectRuntime.GetItem(calls, "length"), 2d, "order of operations");
        Assert.strictEqual(ObjectRuntime.GetItem(calls, 0d), $"get {name}.{method}", "order of operations");
        Assert.strictEqual(ObjectRuntime.GetItem(calls, 1d), $"call {name}.{method}", "order of operations");
    }

    private static void CheckStringOptionWrongType(
        object helpers, object? name, object? value, object? check, object? assertion)
    {
        CheckThrows(check, JsNull.Null, "RangeError", "null");
        CheckThrows(check, true, "RangeError", "true");
        CheckThrows(check, false, "RangeError", "false");
        CheckThrows(check, new Symbol(), "TypeError", "symbol");
        CheckThrows(check, 2d, "RangeError", "number");
        CheckThrows(check, new BigInteger(2), "RangeError", "bigint");
        CheckThrows(check, PlainObject(), "RangeError", "plain object");
        var calls = new JavaScriptRuntime.Array();
        var observer = ObjectRuntime.CallMember(helpers, "toPrimitiveObserver", [calls, value!, name!]);
        var result = Call(check, observer);
        Call(assertion, result, "object with toString");
        AssertConversionTrace(calls, StringValue(name), "toString");
    }

    private static void CheckRoundingIncrementOptionWrongType(
        object helpers, object? check, object? trueAssertion, object? objectAssertion)
    {
        CheckThrows(check, JsNull.Null, "RangeError", "null");
        var trueResult = Call(check, true);
        Call(trueAssertion, trueResult, "true");
        CheckThrows(check, false, "RangeError", "false");
        CheckThrows(check, new Symbol(), "TypeError", "symbol");
        CheckThrows(check, new BigInteger(2), "TypeError", "bigint");
        CheckThrows(check, PlainObject(), "RangeError", "plain object");
        var calls = new JavaScriptRuntime.Array();
        var observer = ObjectRuntime.CallMember(helpers, "toPrimitiveObserver", [calls, 2d, "roundingIncrement"]);
        var objectResult = Call(check, observer);
        Call(objectAssertion, objectResult, "object with valueOf");
        AssertConversionTrace(calls, "roundingIncrement", "valueOf");
    }

    private static void CheckPluralUnitsAccepted(object helpers, object? callback, object? units)
    {
        var plurals = new JsObject();
        foreach (var unit in new[] { "year", "month", "week", "day", "hour", "minute", "second",
            "millisecond", "microsecond", "nanosecond" })
        {
            ObjectRuntime.SetItem(plurals, unit, unit + "s");
        }
        ObjectRuntime.CallMember(units!, "forEach", [CreateFunction((Action<object?>)(unit =>
        {
            var plural = ObjectRuntime.GetItem(plurals, unit!);
            var singularValue = Call(callback, unit);
            var pluralValue = Call(callback, plural);
            var description = $"Plural {StringValue(plural)} produces the same result as singular {StringValue(unit)}";
            if (IsPlainTime(singularValue))
            {
                ObjectRuntime.CallMember(helpers, "assertPlainTimesEqual", [pluralValue!, singularValue!, description]);
            }
            else if (TypeUtilities.IsPrimitive(singularValue))
            {
                Assert.strictEqual(pluralValue, singularValue, description);
            }
            else
            {
                // Selected PlainTime callbacks return times or strings only. Do not silently
                // replace the upstream Duration/Instant/date-type equality branches with identity.
                throw new TypeError("TemporalHelpers.checkPluralUnitsAccepted: unsupported callback result type");
            }
        }), "", 1)]);
    }

    private static void ObserveMethod(object? calls, object? target, object? key, object? name)
    {
        var method = ObjectRuntime.GetItem(target!, key!);
        var observed = CreateFunction((BuiltinFunctionVariadic)((object? _, in JsCallArguments arguments) =>
        {
            Log(calls, "call " + FormatPropertyName(key, name));
            return CallableOperations.Call(method, target, arguments);
        }), "", 0);
        Function.MarkConstructible(observed);
        ObjectRuntime.SetItem(target!, key!, observed);
    }

    private static void ObserveProperty(object? calls, object? target, object? key, object? value, object? name)
    {
        var descriptor = new JsObject();
        ObjectRuntime.SetItem(descriptor, "get", CreateFunction((Func<object?>)(() =>
        {
            Log(calls, "get " + FormatPropertyName(key, name));
            return value;
        }), "get", 0));
        ObjectRuntime.SetItem(descriptor, "set", CreateFunction(
            (Action)(() => Log(calls, "set " + FormatPropertyName(key, name))), "set", 0));
        JavaScriptRuntime.Object.defineProperty(target!, key!, descriptor);
    }

    private static object PropertyBagObserver(
        object helpers, object? calls, object? bag, object? name, object? skip)
    {
        var handler = new JsObject();
        ObjectRuntime.SetItem(handler, "ownKeys", CreateFunction((Func<object, object>)(target =>
        {
            Log(calls, "ownKeys " + StringValue(name));
            return Reflect.ownKeys(target);
        }), "ownKeys", 1));
        ObjectRuntime.SetItem(handler, "getOwnPropertyDescriptor", CreateFunction(
            (Func<object, object?, object?>)((target, key) =>
            {
                Log(calls, "getOwnPropertyDescriptor " + FormatPropertyName(key, name));
                return Reflect.getOwnPropertyDescriptor(target, key);
            }), "getOwnPropertyDescriptor", 2));
        ObjectRuntime.SetItem(handler, "get", CreateFunction(
            (Func<object, object?, object?, object?>)((target, key, receiver) =>
            {
                Log(calls, "get " + FormatPropertyName(key, name));
                var result = Reflect.get(target, key, receiver);
                if (result is null)
                {
                    return null;
                }
                var type = TypeUtilities.Typeof(result);
                if ((result is not JsNull && type == "object") || type == "function")
                {
                    return result;
                }
                if (TypeUtilities.ToBoolean(skip)
                    && TypeUtilities.ToNumber(ObjectRuntime.CallMember(skip!, "indexOf", new object[] { key! })) >= 0)
                {
                    return result;
                }
                return ObjectRuntime.CallMember(helpers, "toPrimitiveObserver",
                    new object[] { calls!, result, FormatPropertyName(key, name) });
            }), "get", 3));
        ObjectRuntime.SetItem(handler, "has", CreateFunction(
            (Func<object, object?, bool>)((target, key) =>
            {
                Log(calls, "has " + FormatPropertyName(key, name));
                return Reflect.has(target, key);
            }), "has", 2));
        return new Proxy(bag, handler);
    }

    private static object ToPrimitiveObserver(object? calls, object? value, object? name)
    {
        var result = new JsObject();
        foreach (var method in new[] { "valueOf", "toString" })
        {
            var descriptor = new JsObject();
            ObjectRuntime.SetItem(descriptor, "enumerable", true);
            ObjectRuntime.SetItem(descriptor, "configurable", true);
            ObjectRuntime.SetItem(descriptor, "get", CreateFunction((Func<object>)(() =>
            {
                Log(calls, $"get {StringValue(name)}.{method}");
                return CreateObservedConversion(calls, value, name, method);
            }), "get " + method, 0));
            JavaScriptRuntime.Object.defineProperty(result, method, descriptor);
        }
        return result;
    }

    private static object CreateObservedConversion(object? calls, object? value, object? name, string method)
        => CreateFunction((Func<object?>)(() =>
        {
            Log(calls, $"call {StringValue(name)}.{method}");
            return method == "valueOf" || value is null
                ? value
                : ObjectRuntime.CallMember(value, "toString", System.Array.Empty<object>());
        }), "", 0);

    private static void Log(object? calls, string entry)
        => ObjectRuntime.CallMember(calls!, "push", new object[] { entry });

    private static string StringValue(object? value)
        => DotNet2JSConversions.ToStringRejectingSymbols(value);

    private static string FormatPropertyName(object? key, object? name)
    {
        name ??= "";
        if (key is Symbol symbol)
        {
            var registryKey = Symbol.keyFor(symbol);
            if (registryKey is not null)
            {
                return $"{StringValue(name)}[Symbol.for('{StringValue(registryKey)}')]";
            }
            var description = symbol.description
                ?? throw new TypeError("Cannot read properties of undefined (reading 'startsWith')");
            return description.StartsWith("Symbol.", StringComparison.Ordinal)
                ? $"{StringValue(name)}[{description}]"
                : $"{StringValue(name)}[Symbol('{description}')]";
        }
        if (key is string text && text != StringValue(TypeUtilities.ToNumber(text)))
        {
            if (Regex.IsMatch(text, @"\A[$_a-zA-Z][$_a-zA-Z0-9]*\z"))
            {
                return TypeUtilities.ToBoolean(name) ? $"{StringValue(name)}.{text}" : text;
            }
            return $"{StringValue(name)}['{text.Replace("'", "\\'")}']";
        }
        return $"{StringValue(name)}[{StringValue(key)}]";
    }
}
