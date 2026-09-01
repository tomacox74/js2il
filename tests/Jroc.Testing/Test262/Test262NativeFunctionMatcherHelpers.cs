using System.Text.RegularExpressions;
using JavaScriptRuntime;

namespace Jroc.Tests;

internal static class Test262NativeFunctionMatcherHelpers
{
    private static readonly Regex NativeFunctionPattern = new(
        @"\A\s*function(?:\s+(?:get|set))?(?:\s+(?:[\p{L}_$][\p{L}\p{Nd}_$]*|\[[\s\S]*\]))?\s*\(\s*\)\s*\{\s*\[\s*native\s+code\s*\]\s*\}\s*\z",
        RegexOptions.CultureInvariant);

    public static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
    {
        builder
            .AddGlobalFactory("validateNativeFunctionSource", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Action<object?>)ValidateNativeFunctionSource,
                "validateNativeFunctionSource",
                1))
            .AddGlobalFactory("assertNativeFunction", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Action<object?, object?>)AssertNativeFunction,
                "assertNativeFunction",
                2))
            .AddGlobalFactory("assertToStringOrNativeFunction", () => Test262HostRuntimeIntrinsics.CreateFunction(
                (Action<object?, object?>)AssertToStringOrNativeFunction,
                "assertToStringOrNativeFunction",
                2));
    }

    private static void ValidateNativeFunctionSource(object? source)
    {
        if (!NativeFunctionPattern.IsMatch(Test262HostRuntimeIntrinsics.ToMessage(source)))
        {
            throw new SyntaxError();
        }
    }

    private static void AssertToStringOrNativeFunction(object? function, object? expected)
    {
        var actual = FunctionToString(function);
        if (!string.Equals(actual, Test262HostRuntimeIntrinsics.ToMessage(expected), StringComparison.Ordinal))
        {
            AssertNativeFunction(function, expected);
        }
    }

    private static void AssertNativeFunction(object? function, object? special)
    {
        var actual = FunctionToString(function);
        try
        {
            ValidateNativeFunctionSource(actual);
        }
        catch (SyntaxError)
        {
            var suffix = special is null or JsNull
                ? string.Empty
                : $" ({Test262HostRuntimeIntrinsics.ToMessage(special)})";
            throw Test262HostRuntimeIntrinsics.CreateTest262Error(
                $"Conforms to NativeFunction Syntax: {System.Text.Json.JsonSerializer.Serialize(actual)}{suffix}");
        }
    }

    private static string FunctionToString(object? function)
        => Test262HostRuntimeIntrinsics.ToMessage(
            ObjectRuntime.CallMember(function!, "toString", global::System.Array.Empty<object>()));
}
