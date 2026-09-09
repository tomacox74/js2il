using JavaScriptRuntime;
using System.Text.RegularExpressions;
using static Jroc.Tests.Test262HostRuntimeIntrinsics;

namespace Jroc.Tests;

// The observation helpers from the pinned temporalHelpers.js are also used by Array.fromAsync.
internal static class Test262TemporalHelpers
{
    internal static void Register(HostRuntimeIntrinsicDescriptorsBuilder builder)
        => builder.AddGlobalFactory("TemporalHelpers", CreateHelpers);

    private static object CreateHelpers()
    {
        var helpers = new JsObject();
        ObjectRuntime.SetItem(helpers, "observeProperty", CreateFunction(
            (Action<object?, object?, object?, object?, object?>)ObserveProperty, "observeProperty", 4));
        ObjectRuntime.SetItem(helpers, "toPrimitiveObserver", CreateFunction(
            (Func<object?, object?, object?, object>)ToPrimitiveObserver, "toPrimitiveObserver", 3));
        ObjectRuntime.SetItem(helpers, "propertyBagObserver", CreateFunction(
            (Func<object?, object?, object?, object?, object>)((calls, bag, name, skip) =>
                PropertyBagObserver(helpers, calls, bag, name, skip)), "propertyBagObserver", 4));
        return helpers;
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
