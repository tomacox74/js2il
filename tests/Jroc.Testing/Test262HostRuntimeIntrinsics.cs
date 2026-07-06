using System.Dynamic;
using JavaScriptRuntime;

namespace Jroc.Tests;

public static class Test262HostRuntimeIntrinsics
{
    public static HostRuntimeIntrinsicDescriptors Create()
        => new HostRuntimeIntrinsicDescriptorsBuilder()
            .AddGlobalFactory("assert", CreateAssert)
            .AddGlobalFactory("Test262Error", CreateTest262ErrorConstructor)
            .AddGlobalValue("$ERROR", (Action<object?>)(message => throw CreateTest262Error(message)))
            .AddGlobalValue("$DONE", CreateDoneFunction())
            .AddGlobalFactory("$262", Create262Object)
            .AddGlobalValue("compareArray", (Func<object?, object?, bool>)CompareArray)
            .AddGlobalValue("verifyProperty", (Action<object?, object?, object?>)VerifyProperty)
            .AddGlobalValue("verifyWritable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "writable", true)))
            .AddGlobalValue("verifyNotWritable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "writable", false)))
            .AddGlobalValue("verifyEnumerable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "enumerable", true)))
            .AddGlobalValue("verifyNotEnumerable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "enumerable", false)))
            .AddGlobalValue("verifyConfigurable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "configurable", true)))
            .AddGlobalValue("verifyNotConfigurable", (Action<object?, object?>)((target, name) => VerifyAttribute(target, name, "configurable", false)))
            .AddGlobalValue("assertRelativeDateMs", (Action<object?, object?>)AssertRelativeDateMs)
            .AddGlobalValue("getWellKnownIntrinsicObject", (Func<object?, object?>)GetWellKnownIntrinsicObject)
            .AddGlobalValue("isConstructor", (Func<object?, bool>)JavaScriptRuntime.Object.IsConstructibleValue)
            .AddGlobalValue("asyncTest", (Action<object?>)AsyncTest)
            .Build();

    private static object CreateAssert()
    {
        var assert = (Action<object?, object?>)((condition, message) =>
        {
            var passed = TypeUtilities.ToBoolean(condition);
            Log(passed);
            if (!passed)
            {
                ThrowAssertion(message, "Assertion failed");
            }
        });

        var sameValue = (Action<object?, object?, object?>)((actual, expected, message) =>
        {
            var passed = JavaScriptRuntime.Object.@is(actual, expected);
            Log(passed);
            if (!passed)
            {
                ThrowAssertion(message, "Expected SameValue");
            }
        });

        var notSameValue = (Action<object?, object?, object?>)((actual, unexpected, message) =>
        {
            var passed = !JavaScriptRuntime.Object.@is(actual, unexpected);
            Log(passed);
            if (!passed)
            {
                ThrowAssertion(message, "Expected values to differ");
            }
        });

        var throws = (Action<object?, object?, object?>)((expectedErrorConstructor, fn, message) =>
        {
            var passed = false;
            try
            {
                Closure.InvokeWithArgs(fn!, RuntimeServices.EmptyScopes);
            }
            catch (Exception error)
            {
                passed = IsExpectedError(error is JsThrownValueException thrown ? thrown.Value : error, expectedErrorConstructor);
            }

            Log(passed);
            if (!passed)
            {
                ThrowAssertion(message, "Expected function to throw");
            }
        });

        var compareArray = (Action<object?, object?, object?>)((actual, expected, message) =>
        {
            var passed = CompareArray(actual, expected);
            Log(passed);
            if (!passed)
            {
                ThrowAssertion(message, "Expected arrays to match");
            }
        });

        InitializeFunction(assert, "assert", 2);
        InitializeFunction(sameValue, "sameValue", 3);
        InitializeFunction(notSameValue, "notSameValue", 3);
        InitializeFunction(throws, "throws", 3);
        InitializeFunction(compareArray, "compareArray", 3);

        ObjectRuntime.SetItem(assert, "sameValue", sameValue);
        ObjectRuntime.SetItem(assert, "notSameValue", notSameValue);
        ObjectRuntime.SetItem(assert, "strictEqual", sameValue);
        ObjectRuntime.SetItem(assert, "notStrictEqual", notSameValue);
        ObjectRuntime.SetItem(assert, "throws", throws);
        ObjectRuntime.SetItem(assert, "compareArray", compareArray);

        return assert;
    }

    private static object CreateTest262ErrorConstructor()
    {
        Func<object[], object?[], object?> constructor = (_, args) =>
        {
            var instance = RuntimeServices.GetCurrentThis();
            if (instance is null)
            {
                return CreateTest262Error(args.Length > 0 ? args[0] : null);
            }

            ObjectRuntime.SetItem(instance, "name", "Test262Error");
            ObjectRuntime.SetItem(instance, "message", ToMessage(args.Length > 0 ? args[0] : null));
            return null;
        };

        InitializeFunction(constructor, "Test262Error", 1);

        var prototype = new ExpandoObject();
        ObjectRuntime.SetItem(prototype, "constructor", constructor);
        ObjectRuntime.SetItem(constructor, "prototype", prototype);

        return constructor;
    }

    private static object Create262Object()
    {
        var result = new ExpandoObject();
        ObjectRuntime.SetItem(result, "createRealm", (Func<object>)CreateRealm);
        ObjectRuntime.SetItem(result, "detachArrayBuffer", Unsupported262("$262.detachArrayBuffer"));
        ObjectRuntime.SetItem(result, "evalScript", Unsupported262("$262.evalScript"));
        ObjectRuntime.SetItem(result, "gc", Unsupported262("$262.gc"));
        return result;
    }

    private static object CreateRealm()
    {
        var realm = new ExpandoObject();
        ObjectRuntime.SetItem(realm, "global", GlobalThis.globalThis);
        return realm;
    }

    private static Action Unsupported262(string name)
    {
        return () => throw CreateTest262Error($"{name} is not supported by the JROC C# test262 harness.");
    }

    private static void VerifyProperty(object? target, object? name, object? expectedDescriptor)
    {
        var actualDescriptor = JavaScriptRuntime.Object.getOwnPropertyDescriptor(target!, name);
        var passed = actualDescriptor is not null;
        if (passed && HasOwn(expectedDescriptor, "value"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "value"),
                ObjectRuntime.GetItem(expectedDescriptor!, "value"));
        }

        if (passed && HasOwn(expectedDescriptor, "writable"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "writable"),
                ObjectRuntime.GetItem(expectedDescriptor!, "writable"));
        }

        if (passed && HasOwn(expectedDescriptor, "enumerable"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "enumerable"),
                ObjectRuntime.GetItem(expectedDescriptor!, "enumerable"));
        }

        if (passed && HasOwn(expectedDescriptor, "configurable"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "configurable"),
                ObjectRuntime.GetItem(expectedDescriptor!, "configurable"));
        }

        if (passed && HasOwn(expectedDescriptor, "get"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "get"),
                ObjectRuntime.GetItem(expectedDescriptor!, "get"));
        }

        if (passed && HasOwn(expectedDescriptor, "set"))
        {
            passed = JavaScriptRuntime.Object.@is(
                ObjectRuntime.GetItem(actualDescriptor!, "set"),
                ObjectRuntime.GetItem(expectedDescriptor!, "set"));
        }

        Log(passed);
        if (!passed)
        {
            ThrowAssertion($"verifyProperty failed for {ToMessage(name)}");
        }
    }

    private static void VerifyAttribute(object? target, object? name, string attributeName, bool expectedValue)
    {
        var actualDescriptor = JavaScriptRuntime.Object.getOwnPropertyDescriptor(target!, name);
        var passed = actualDescriptor is not null
            && JavaScriptRuntime.Object.@is(ObjectRuntime.GetItem(actualDescriptor!, attributeName), expectedValue);

        Log(passed);
        if (!passed)
        {
            ThrowAssertion($"verify{(expectedValue ? string.Empty : "Not")}{Capitalize(attributeName)} failed for {ToMessage(name)}");
        }
    }

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
        Log(passed);
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

    private static Action<object?> CreateDoneFunction()
        => error =>
        {
            if (error is not null)
            {
                throw error as Exception ?? new Error(ToMessage(error));
            }
        };

    private static bool IsExpectedError(object? error, object? expectedErrorConstructor)
    {
        if (expectedErrorConstructor is null)
        {
            return false;
        }

        var expectedName = ObjectRuntime.GetItem(expectedErrorConstructor, "name") as string;
        if (!string.IsNullOrEmpty(expectedName)
            && string.Equals(GetErrorName(error), expectedName, StringComparison.Ordinal))
        {
            return true;
        }

        return false;
    }

    private static string GetErrorName(object? error)
        => error switch
        {
            Error jsError => jsError.name,
            null => string.Empty,
            JsNull => string.Empty,
            _ => ObjectRuntime.GetItem(error, "name") as string ?? error.GetType().Name
        };

    private static bool HasOwn(object? target, string name)
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

    private static void ThrowAssertion(object? message, string fallback = "Assertion failed")
        => throw CreateTest262Error(string.IsNullOrEmpty(ToMessage(message)) ? fallback : ToMessage(message));

    private static Test262Error CreateTest262Error(object? message)
        => new(ToMessage(message));

    private static void InitializeFunction(Delegate function, string name, double length)
        => Function.InitializeFunctionInstance(function, length, name, requiresInvocationContext: false);

    private static void Log(bool value)
        => GlobalThis.console.log(value);

    private static string ToMessage(object? value)
        => value switch
        {
            null or JsNull => string.Empty,
            _ => DotNet2JSConversions.ToString(value)
        };

    private static string Capitalize(string value)
        => string.IsNullOrEmpty(value) ? value : char.ToUpperInvariant(value[0]) + value[1..];

    private sealed class Test262Error : Error
    {
        public Test262Error(string message)
            : base(message)
        {
            Name = "Test262Error";
        }
    }
}
