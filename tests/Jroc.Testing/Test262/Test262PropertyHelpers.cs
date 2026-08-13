using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262PropertyHelpers
{
    private static readonly string[] DescriptorFields =
        ["value", "writable", "enumerable", "configurable", "get", "set"];

    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("verifyProperty", () => Function(VerifyProperty, "verifyProperty", 3))
            .AddGlobalFactory("verifyCallableProperty", () => Function(VerifyCallableProperty, "verifyCallableProperty", 4))
            .AddGlobalFactory("verifyEqualTo", () => Function(VerifyEqualTo, "verifyEqualTo", 3))
            .AddGlobalFactory("verifyWritable", () => Function(VerifyWritable, "verifyWritable", 2))
            .AddGlobalFactory("verifyNotWritable", () => Function(VerifyNotWritable, "verifyNotWritable", 2))
            .AddGlobalFactory("verifyEnumerable", () => Function(VerifyEnumerable, "verifyEnumerable", 2))
            .AddGlobalFactory("verifyNotEnumerable", () => Function(VerifyNotEnumerable, "verifyNotEnumerable", 2))
            .AddGlobalFactory("verifyConfigurable", () => Function(VerifyConfigurable, "verifyConfigurable", 2))
            .AddGlobalFactory("verifyNotConfigurable", () => Function(VerifyNotConfigurable, "verifyNotConfigurable", 2))
            .AddGlobalFactory("verifyPrimordialProperty", () => Function(VerifyProperty, "verifyProperty", 3))
            .AddGlobalFactory("verifyPrimordialCallableProperty", () => Function(VerifyCallableProperty, "verifyCallableProperty", 4));
    }

    private static object? VerifyProperty(object[] _, object?[]? args)
    {
        args ??= [];
        if (args.Length < 3)
        {
            Fail("verifyProperty should receive at least 3 arguments: obj, name, and descriptor");
        }

        var target = args[0]!;
        var name = args[1];
        var expected = args[2];
        var options = args.Length > 3 ? args[3] : null;
        var actual = JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, name);
        var nameText = Test262HostRuntimeIntrinsics.ToMessage(name);

        if (expected is null)
        {
            if (actual is not null)
            {
                Fail($"obj['{nameText}'] descriptor should be undefined");
            }

            return true;
        }

        if (expected is JsNull || actual is null || !Test262HostRuntimeIntrinsics.HasOwn(target, name!))
        {
            Fail($"obj should have an own property {nameText}");
        }

        ValidateDescriptorFields(expected);
        var failures = new List<string>();

        CompareDescriptorField(target, name, expected, actual!, "value", failures, comparePropertyValue: true);
        CompareDescriptorField(target, name, expected, actual!, "get", failures);
        CompareDescriptorField(target, name, expected, actual!, "set", failures);

        if (HasDefinedField(expected, "enumerable"))
        {
            var expectedValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(expected, "enumerable"));
            var actualValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(actual!, "enumerable"));
            var observableValue = IsEnumerable(target, name);
            if (expectedValue != actualValue || expectedValue != observableValue)
            {
                failures.Add($"obj['{nameText}'] descriptor should {(expectedValue ? string.Empty : "not ")}be enumerable");
            }
        }

        if (HasDefinedField(expected, "writable"))
        {
            var expectedValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(expected, "writable"));
            var actualValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(actual!, "writable"));
            if (expectedValue != actualValue || expectedValue != IsWritable(target, name, null, null, valueWasProvided: false))
            {
                failures.Add($"obj['{nameText}'] descriptor should {(expectedValue ? string.Empty : "not ")}be writable");
            }
        }

        if (HasDefinedField(expected, "configurable"))
        {
            var expectedValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(expected, "configurable"));
            var actualValue = TypeUtilities.ToBoolean(ObjectRuntime.GetItem(actual!, "configurable"));
            if (expectedValue != actualValue || expectedValue != IsConfigurable(target, name))
            {
                failures.Add($"obj['{nameText}'] descriptor should {(expectedValue ? string.Empty : "not ")}be configurable");
            }
        }

        if (failures.Count > 0)
        {
            Fail(string.Join("; ", failures));
        }

        if (options is not null
            && options is not JsNull
            && TypeUtilities.ToBoolean(ObjectRuntime.GetItem(options, "restore")))
        {
            JavaScriptRuntime.Object.defineProperty(target, name, actual!);
        }

        return true;
    }

    private static object? VerifyCallableProperty(object[] _, object?[]? args)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        var functionName = Argument(args, 2);
        var functionLength = Argument(args, 3);
        var descriptor = Argument(args, 4);
        var options = Argument(args, 5);
        var value = ObjectRuntime.GetItem(target, name!);

        if (!CallableOperations.IsCallable(value))
        {
            Fail($"obj['{Test262HostRuntimeIntrinsics.ToMessage(name)}'] descriptor should be a function");
        }

        descriptor ??= CreateDataDescriptor(value);
        if (!Test262HostRuntimeIntrinsics.HasOwn(descriptor, "value")
            && !Test262HostRuntimeIntrinsics.HasOwn(descriptor, "get"))
        {
            ObjectRuntime.SetItem(descriptor, "value", value);
        }

        VerifyProperty([], [target, name, descriptor, options]);
        functionName ??= Test262HostRuntimeIntrinsics.ToMessage(name);
        var configurable = ObjectRuntime.GetItem(descriptor, "configurable");
        VerifyProperty([], [value, "name", Descriptor(functionName, false, false, configurable), options]);
        VerifyProperty([], [value, "length", Descriptor(functionLength, false, false, configurable), options]);
        return null;
    }

    private static object? VerifyEqualTo(object[] _, object?[]? args)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        var expected = Argument(args, 2);
        var actual = ObjectRuntime.GetItem(target, name!);
        if (!JavaScriptRuntime.Object.@is(actual, expected))
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] to equal {Test262HostRuntimeIntrinsics.ToMessage(expected)}, actually {Test262HostRuntimeIntrinsics.ToMessage(actual)}");
        }

        return null;
    }

    private static object? VerifyWritable(object[] _, object?[]? args)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        var verifyProperty = Argument(args, 2);
        var value = Argument(args, 3);
        if (!TypeUtilities.ToBoolean(verifyProperty))
        {
            VerifyDescriptorAttribute(target, name, "writable", expected: true);
        }

        if (!IsWritable(target, name, verifyProperty, value, args.Length >= 4))
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] to be writable, but was not.");
        }

        return null;
    }

    private static object? VerifyNotWritable(object[] _, object?[]? args)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        var verifyProperty = Argument(args, 2);
        var value = Argument(args, 3);
        if (!TypeUtilities.ToBoolean(verifyProperty))
        {
            VerifyDescriptorAttribute(target, name, "writable", expected: false);
        }

        if (IsWritable(target, name, verifyProperty, value, args.Length >= 4))
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] NOT to be writable, but was.");
        }

        return null;
    }

    private static object? VerifyEnumerable(object[] _, object?[]? args)
        => VerifyEnumerableCore(args, expected: true);

    private static object? VerifyNotEnumerable(object[] _, object?[]? args)
        => VerifyEnumerableCore(args, expected: false);

    private static object? VerifyConfigurable(object[] _, object?[]? args)
        => VerifyConfigurableCore(args, expected: true);

    private static object? VerifyNotConfigurable(object[] _, object?[]? args)
        => VerifyConfigurableCore(args, expected: false);

    private static object? VerifyEnumerableCore(object?[]? args, bool expected)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        VerifyDescriptorAttribute(target, name, "enumerable", expected);
        var actual = IsEnumerable(target, name);
        if (actual != expected)
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] {(expected ? string.Empty : "NOT ")}to be enumerable.");
        }

        return null;
    }

    private static object? VerifyConfigurableCore(object?[]? args, bool expected)
    {
        args ??= [];
        var target = Argument(args, 0)!;
        var name = Argument(args, 1);
        VerifyDescriptorAttribute(target, name, "configurable", expected);
        if (IsConfigurable(target, name) != expected)
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] {(expected ? string.Empty : "NOT ")}to be configurable.");
        }

        return null;
    }

    private static void ValidateDescriptorFields(object expected)
    {
        var names = JavaScriptRuntime.Object.getOwnPropertyNames(expected);
        foreach (var name in EnumerateArrayLike(names))
        {
            var text = Test262HostRuntimeIntrinsics.ToMessage(name);
            if (!DescriptorFields.Contains(text, StringComparer.Ordinal))
            {
                Fail($"Invalid descriptor field: {text}");
            }
        }
    }

    private static void CompareDescriptorField(
        object target,
        object? name,
        object expected,
        object actual,
        string field,
        List<string> failures,
        bool comparePropertyValue = false)
    {
        if (!Test262HostRuntimeIntrinsics.HasOwn(expected, field))
        {
            return;
        }

        var expectedValue = ObjectRuntime.GetItem(expected, field);
        var actualValue = ObjectRuntime.GetItem(actual, field);
        var nameText = Test262HostRuntimeIntrinsics.ToMessage(name);
        if (!JavaScriptRuntime.Object.@is(expectedValue, actualValue))
        {
            failures.Add($"obj['{nameText}'] descriptor {field} should be {Test262HostRuntimeIntrinsics.ToMessage(expectedValue)}");
        }

        if (comparePropertyValue
            && !JavaScriptRuntime.Object.@is(expectedValue, ObjectRuntime.GetItem(target, name!)))
        {
            failures.Add($"obj['{nameText}'] value should be {Test262HostRuntimeIntrinsics.ToMessage(expectedValue)}");
        }
    }

    private static bool IsWritable(
        object target,
        object? name,
        object? verifyProperty,
        object? value,
        bool valueWasProvided)
    {
        object? unlikelyValue = target is JavaScriptRuntime.Array
            && string.Equals(Test262HostRuntimeIntrinsics.ToMessage(name), "length", StringComparison.Ordinal)
                ? 4294967295d
                : "unlikelyValue";
        var newValue = valueWasProvided && TypeUtilities.ToBoolean(value) ? value : unlikelyValue;
        var hadValue = Test262HostRuntimeIntrinsics.HasOwn(target, name!);
        var oldValue = ObjectRuntime.GetItem(target, name!);
        if (!valueWasProvided && JavaScriptRuntime.Object.@is(newValue, oldValue))
        {
            newValue = $"{Test262HostRuntimeIntrinsics.ToMessage(newValue)}2";
        }

        ObjectRuntime.SetItem(target, name!, newValue, throwOnError: false);
        var observedName = TypeUtilities.ToBoolean(verifyProperty) ? verifyProperty : name;
        var succeeded = JavaScriptRuntime.Object.@is(ObjectRuntime.GetItem(target, observedName!), newValue);
        if (succeeded)
        {
            if (hadValue)
            {
                ObjectRuntime.SetItem(target, name!, oldValue, throwOnError: false);
            }
            else
            {
                ObjectRuntime.DeletePropertyNonStrict(target, name);
            }
        }

        return succeeded;
    }

    private static bool IsConfigurable(object target, object? name)
    {
        try
        {
            return ObjectRuntime.DeleteProperty(target, name);
        }
        catch (TypeError)
        {
            return false;
        }
    }

    private static bool IsEnumerable(object target, object? name)
    {
        var descriptor = JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, name);
        return descriptor is not null
            && Test262HostRuntimeIntrinsics.HasOwn(target, name!)
            && TypeUtilities.ToBoolean(ObjectRuntime.GetItem(descriptor, "enumerable"));
    }

    private static void VerifyDescriptorAttribute(object target, object? name, string attribute, bool expected)
    {
        var descriptor = JavaScriptRuntime.Object.getOwnPropertyDescriptor(target, name);
        var actual = descriptor is not null
            && TypeUtilities.ToBoolean(ObjectRuntime.GetItem(descriptor, attribute));
        if (actual != expected)
        {
            Fail($"Expected obj[{Test262HostRuntimeIntrinsics.ToMessage(name)}] to have {attribute}:{expected.ToString().ToLowerInvariant()}.");
        }
    }

    private static bool HasDefinedField(object descriptor, string field)
        => Test262HostRuntimeIntrinsics.HasOwn(descriptor, field)
            && ObjectRuntime.GetItem(descriptor, field) is not null;

    private static JsObject CreateDataDescriptor(object? value)
        => Descriptor(value, true, false, true);

    private static JsObject Descriptor(object? value, object? writable, object? enumerable, object? configurable)
    {
        var descriptor = new JsObject();
        ObjectRuntime.SetItem(descriptor, "value", value);
        ObjectRuntime.SetItem(descriptor, "writable", writable);
        ObjectRuntime.SetItem(descriptor, "enumerable", enumerable);
        ObjectRuntime.SetItem(descriptor, "configurable", configurable);
        return descriptor;
    }

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

    private static void Fail(string message)
        => throw Test262HostRuntimeIntrinsics.CreateTest262Error(message);
}
