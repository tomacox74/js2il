using JavaScriptRuntime;
using JavaScriptRuntime.Node;

namespace Jroc.Tests;

public static class Test262HostRuntimeIntrinsics
{
    public static HostRuntimeIntrinsicDescriptors Create(IEnumerable<string>? harnessFiles = null)
    {
        var included = harnessFiles?.ToHashSet(StringComparer.Ordinal)
            ?? [];
        var builder = new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalFactory("assert", CreateNodeAssertAdapter)
            .AddGlobalFactory("Test262Error", CreateTest262ErrorConstructor)
            .AddGlobalFactory("$ERROR", () => CreateFunction(
                (Action<object?>)(message => throw CreateTest262Error(message)),
                "$ERROR",
                1))
            .AddGlobalFactory("$DONE", CreateDoneFunction)
            .AddGlobalFactory("$262", Create262Object)
            .AddGlobalFactory("compareArray", () => CreateFunction(
                (Func<object?, object?, bool>)CompareArray,
                "compareArray",
                2))
            .AddGlobalFactory("assertRelativeDateMs", () => CreateFunction(
                (Action<object?, object?>)AssertRelativeDateMs,
                "assertRelativeDateMs",
                2))
            .AddGlobalFactory("getWellKnownIntrinsicObject", () => CreateFunction(
                (Func<object?, object?>)GetWellKnownIntrinsicObject,
                "getWellKnownIntrinsicObject",
                1))
            .AddGlobalFactory("isConstructor", () => CreateFunction(
                (Func<object?, bool>)JavaScriptRuntime.ObjectRuntime.IsConstructibleValue,
                "isConstructor",
                1))
            .AddGlobalFactory("asyncTest", () => CreateFunction(
                (Action<object?>)AsyncTest,
                "asyncTest",
                1));

        Test262PropertyHelpers.Register(builder);

        if (included.Contains("testTypedArray.js"))
        {
            Test262TypedArrayHelpers.Register(builder);
        }

        if (included.Contains("testAtomics.js"))
        {
            Test262AtomicsHelpers.Register(builder);
        }

        if (included.Contains("decimalToHexString.js"))
        {
            Test262EncodingHelpers.Register(builder);
        }

        if (included.Contains("promiseHelper.js"))
        {
            Test262PromiseHelpers.Register(builder);
        }

        if (included.Contains("tcoHelper.js"))
        {
            builder.AddGlobalValue("$MAX_ITERATIONS", 100000d);
        }

        if (included.Contains("nans.js"))
        {
            builder.AddGlobalFactory("NaNs", CreateNaNs);
        }

        return builder.Build();
    }

    private static object CreateNodeAssertAdapter()
    {
        var assert = new AssertModule();
        ObjectRuntime.SetItem(assert, "sameValue", CreateFunction(
            (Action<object?, object?, object?>)((actual, expected, message) =>
                assert.strictEqual(actual, expected, message)),
            "sameValue",
            3));
        ObjectRuntime.SetItem(assert, "notSameValue", CreateFunction(
            (Action<object?, object?, object?>)((actual, unexpected, message) =>
                assert.notStrictEqual(actual, unexpected, message)),
            "notSameValue",
            3));
        ObjectRuntime.SetItem(assert, "throws", CreateFunction(
            (Action<object?, object?, object?>)((expectedErrorConstructor, fn, message) =>
                assert.throws(
                    fn,
                    expectedErrorConstructor,
                    message is null or JsNull ? null : ToMessage(message))),
            "throws",
            3));
        ObjectRuntime.SetItem(assert, "compareArray", CreateFunction(
            (Action<object?, object?, object?>)((actual, expected, message) =>
                assert.ok(CompareArray(actual, expected), message)),
            "compareArray",
            3));
        return assert;
    }

    private static object CreateTest262ErrorConstructor()
    {
        var constructorIdentity = new object();
        var constructor = CreateFunction((Func<object[], object?[], object?>)((_, args) =>
        {
            GC.KeepAlive(constructorIdentity);
            var instance = RuntimeServices.GetCurrentThis();
            if (instance is null)
            {
                return CreateTest262Error(args.Length > 0 ? args[0] : null);
            }

            ObjectRuntime.SetItem(instance, "name", "Test262Error");
            ObjectRuntime.SetItem(instance, "message", ToMessage(args.Length > 0 ? args[0] : null));
            return null;
        }), "Test262Error", 1);
        Function.MarkConstructible(constructor);

        var prototype = new JsObject();
        ObjectRuntime.SetItem(prototype, "constructor", constructor);
        ObjectRuntime.SetItem(constructor, "prototype", prototype);

        return constructor;
    }

    private static object Create262Object()
    {
        var result = new JsObject();
        ObjectRuntime.SetItem(result, "createRealm", CreateFunction(
            (Func<object>)CreateRealm,
            "createRealm",
            0));
        ObjectRuntime.SetItem(result, "detachArrayBuffer", Unsupported262("$262.detachArrayBuffer"));
        ObjectRuntime.SetItem(result, "evalScript", Unsupported262("$262.evalScript"));
        ObjectRuntime.SetItem(result, "gc", Unsupported262("$262.gc"));
        return result;
    }

    private static object CreateRealm()
    {
        var realm = new JsObject();
        ObjectRuntime.SetItem(realm, "global", GlobalThis.globalThis);
        return realm;
    }

    private static JsFunctionObject Unsupported262(string name)
        => CreateFunction(
            (Action)(() => throw CreateTest262Error($"{name} is not supported by the JROC C# test262 harness.")),
            name,
            0);

    private static bool CompareArray(object? actual, object? expected)
    {
        if (actual is null || expected is null)
        {
            return false;
        }

        var actualLength = ToLength(ObjectRuntime.GetItem(actual, "length"));
        var expectedLength = ToLength(ObjectRuntime.GetItem(expected, "length"));
        if (actualLength != expectedLength)
        {
            return false;
        }

        for (var i = 0L; i < actualLength; i++)
        {
            if (!JavaScriptRuntime.Object.@is(ObjectRuntime.GetItem(actual, (double)i), ObjectRuntime.GetItem(expected, (double)i)))
            {
                return false;
            }
        }

        return true;
    }

    private static void AssertRelativeDateMs(object? date, object? expectedMs)
    {
        var actualMs = ObjectRuntime.CallMember(date!, "valueOf", global::System.Array.Empty<object>());
        var timezoneOffsetMinutes = ObjectRuntime.CallMember(date!, "getTimezoneOffset", global::System.Array.Empty<object>());
        var normalizedActualMs = TypeUtilities.ToNumber(actualMs) - TypeUtilities.ToNumber(timezoneOffsetMinutes) * 60_000d;
        var passed = JavaScriptRuntime.Object.@is(normalizedActualMs, TypeUtilities.ToNumber(expectedMs));
        if (!passed)
        {
            ThrowAssertion($"Expected date value {ToMessage(expectedMs)}");
        }
    }

    private static object? GetWellKnownIntrinsicObject(object? name)
    {
        return ToMessage(name) switch
        {
            "%AsyncFunction%" => GetStaticFieldValue(typeof(AsyncFunction), "ConstructorValue"),
            "%GeneratorFunction%" => GetStaticFieldValue(typeof(GeneratorObject), "_generatorFunctionConstructor"),
            var unsupported => throw CreateTest262Error($"Unsupported intrinsic {unsupported}")
        };
    }

    private static object GetStaticFieldValue(Type type, string fieldName)
    {
        var field = type.GetField(fieldName, System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static)
            ?? throw new InvalidOperationException($"Unable to resolve {type.FullName}.{fieldName}.");
        return field.GetValue(null)
            ?? throw new InvalidOperationException($"Resolved {type.FullName}.{fieldName} to null.");
    }

    private static void AsyncTest(object? testFunc)
    {
        var result = Closure.InvokeWithArgs(testFunc!, RuntimeServices.EmptyScopes);
        var done = CreateDoneFunction();
        ObjectRuntime.CallMember(result!, "then", new object[] { done, done });
    }

    private static JsFunctionObject CreateDoneFunction()
        => CreateFunction(
            (Action<object?>)(error =>
            {
            if (error is not null)
            {
                throw error as Exception ?? new Error(ToMessage(error));
            }
            }),
            "$DONE",
            1);

    internal static bool HasOwn(object? target, object name)
        => target is not null && target is not JsNull && JavaScriptRuntime.Object.hasOwn(target, name);

    private static long ToLength(object? value)
    {
        var number = TypeUtilities.ToNumber(value);
        if (double.IsNaN(number) || number <= 0)
        {
            return 0;
        }

        if (double.IsPositiveInfinity(number))
        {
            return long.MaxValue;
        }

        return (long)global::System.Math.Min(global::System.Math.Floor(number), long.MaxValue);
    }

    internal static void ThrowAssertion(object? message, string fallback = "Assertion failed")
        => throw CreateTest262Error(string.IsNullOrEmpty(ToMessage(message)) ? fallback : ToMessage(message));

    internal static Test262Error CreateTest262Error(object? message)
        => new(ToMessage(message));

    internal static BuiltinDelegateFunctionAdapter CreateFunction(
        Delegate function,
        string name,
        double length)
    {
        var adapter = BuiltinDelegateFunctionAdapter.FromDelegate(function);
        Function.InitializeFunctionInstance(
            adapter,
            length,
            name,
            requiresInvocationContext: false);
        Function.MarkUndefinedPrototype(adapter);
        return adapter;
    }

    internal static string ToMessage(object? value)
        => value switch
        {
            null or JsNull => string.Empty,
            _ => DotNet2JSConversions.ToString(value)
        };

    internal static object? Invoke(object? callback, params object?[] args)
        => Closure.InvokeWithArgs(callback!, RuntimeServices.EmptyScopes, args);

    private static object CreateNaNs()
    {
        var zero = 0d;
        var invalidDivision = zero / zero;
        var invalidPower = global::System.Math.Pow(-1d, 0.5d);
        return new JavaScriptRuntime.Array(
        new object?[]
        {
            double.NaN,
            double.NaN,
            double.NaN * zero,
            invalidDivision,
            double.PositiveInfinity / double.PositiveInfinity,
            -invalidDivision,
            invalidPower,
            -invalidPower,
            double.NaN
        });
    }

    internal sealed class Test262Error : Error
    {
        public Test262Error(string message)
            : base(message)
        {
            Name = "Test262Error";
        }
    }
}
