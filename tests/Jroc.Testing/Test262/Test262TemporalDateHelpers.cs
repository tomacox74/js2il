using JavaScriptRuntime;
using System.Numerics;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// Native equivalents of the PlainDate assertions and calendar-slot matrix in
// the pinned temporalHelpers.js. No upstream helper JavaScript is executed.
internal static partial class Test262TemporalHelpers
{
    private static void RegisterPlainDateHelpers(JsObject helpers)
    {
        ObjectRuntime.SetItem(helpers, "canonicalizeCalendarEra", CreateFunction(
            (Func<object?, object?, object?>)CanonicalizeCalendarEra, "canonicalizeCalendarEra", 2));
        ObjectRuntime.SetItem(helpers, "assertPlainDate", CreateFunction(
            (BuiltinFunctionVariadic)((object? _, in JsCallArguments args) =>
                AssertPlainDate(helpers, args)), "assertPlainDate", 5));
        ObjectRuntime.SetItem(helpers, "assertPlainDatesEqual", CreateFunction(
            (Action<object?, object?, object?>)AssertPlainDatesEqual, "assertPlainDatesEqual", 2));
        ObjectRuntime.SetItem(helpers, "checkToTemporalCalendarFastPath", CreateFunction(
            (Action<object?>)CheckToTemporalCalendarFastPath, "checkToTemporalCalendarFastPath", 1));
    }

    private static bool IsPlainDate(object? value)
    {
        var temporal = ObjectRuntime.GetItem(GlobalThis.globalThis, "Temporal");
        return Operators.InstanceOf(value, ObjectRuntime.GetItem(temporal!, "PlainDate"));
    }

    private static object? CanonicalizeCalendarEra(object? calendar, object? era)
    {
        Assert.strictEqual(calendar is string, true, "calendar must be string in canonicalizeCalendarEra");
        string[]? known = calendar switch
        {
            "buddhist" => ["be"],
            "coptic" or "hebrew" => ["am"],
            "ethiopic" => ["aa", "am"],
            "ethioaa" => ["aa"],
            "gregory" => ["bce", "ce"],
            "indian" => ["shaka"],
            "islamic-civil" or "islamic-tbla" or "islamic-umalqura" => ["bh", "ah"],
            "japanese" => ["bce", "ce", "heisei", "meiji", "reiwa", "showa", "taisho"],
            "persian" => ["ap"],
            "roc" => ["roc", "broc"],
            _ => null
        };
        if (known is null)
        {
            Assert.strictEqual(era, null);
            return null;
        }
        Assert.strictEqual(era is string, true, "eraName must be string or undefined in canonicalizeCalendarEra");
        var canonical = calendar is "gregory" or "japanese"
            ? era switch { "bc" => "bce", "ad" => "ce", _ => era }
            : era;
        if (canonical is string name && known.Contains(name))
        {
            return name;
        }
        throw CreateTest262Error("Unsupported era name: " + StringValue(era));
    }

    private static object? AssertPlainDate(object helpers, in JsCallArguments args)
    {
        var date = args.GetArgument(0);
        var prefix = DescriptionPrefix(args.GetArgument(5));
        Assert.strictEqual(IsPlainDate(date), true, prefix + "instanceof");
        var actualEra = ObjectRuntime.CallMember(helpers, "canonicalizeCalendarEra",
            [ObjectRuntime.GetItem(date!, "calendarId")!, ObjectRuntime.GetItem(date!, "era")!]);
        var expectedEra = ObjectRuntime.CallMember(helpers, "canonicalizeCalendarEra",
            [ObjectRuntime.GetItem(date!, "calendarId")!, args.GetArgument(6)!]);
        Assert.strictEqual(actualEra, expectedEra, prefix + "era result:");
        Assert.strictEqual(ObjectRuntime.GetItem(date!, "eraYear"), args.GetArgument(7), prefix + "eraYear result:");
        string[] fields = ["year", "month", "monthCode", "day"];
        for (var i = 0; i < fields.Length; i++)
        {
            Assert.strictEqual(ObjectRuntime.GetItem(date!, fields[i]), args.GetArgument(i + 1), prefix + fields[i] + " result:");
        }
        return null;
    }

    private static void AssertPlainDatesEqual(object? actual, object? expected, object? description)
    {
        var prefix = DescriptionPrefix(description);
        Assert.strictEqual(IsPlainDate(expected), true, prefix + "expected value should be a Temporal.PlainDate");
        Assert.strictEqual(IsPlainDate(actual), true, prefix + "instanceof");
        Assert.strictEqual(ObjectRuntime.CallMember(actual!, "equals", [expected!]), true, prefix + "equals method");
        Assert.strictEqual(ObjectRuntime.GetItem(actual!, "calendarId"), ObjectRuntime.GetItem(expected!, "calendarId"),
            prefix + "calendar same value:");
    }

    private static void CheckToTemporalCalendarFastPath(object? callback)
    {
        var temporal = ObjectRuntime.GetItem(GlobalThis.globalThis, "Temporal")!;
        object Construct(string name, params object?[] args)
            => CallableOperations.Construct(ObjectRuntime.GetItem(temporal, name), args)!;
        object[] objects =
        [
            Construct("PlainDate", 2000d, 5d, 2d, "iso8601"),
            Construct("PlainDateTime", 2000d, 5d, 2d, 12d, 34d, 56d, 987d, 654d, 321d, "iso8601"),
            Construct("PlainMonthDay", 5d, 2d, "iso8601"),
            Construct("PlainYearMonth", 2000d, 5d, "iso8601"),
            Construct("ZonedDateTime", BigInteger.Parse("1000000000000000000"), "UTC", "iso8601")
        ];
        foreach (var value in objects)
        {
            foreach (var property in new[] { "calendar", "calendarId" })
            {
                var name = property;
                PropertyDescriptorStore.DefineOrUpdate(value, name, new JsPropertyDescriptor
                {
                    Kind = JsPropertyDescriptorKind.Accessor,
                    Get = CreateFunction((Func<object?>)(() => throw CreateTest262Error("should not get '" + name + "' property")), "get", 0),
                    Enumerable = false,
                    Configurable = false
                });
            }
            Call(callback, value);
        }
    }
}
