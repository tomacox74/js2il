using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;

namespace Js2IL.Runtime;

internal static class ExportMemberResolver
{
    private static object?[] NormalizeArgs(object?[] args)
    {
        if (args.Length == 0)
        {
            return Array.Empty<object?>();
        }

        var normalized = new object?[args.Length];
        for (var i = 0; i < args.Length; i++)
        {
            normalized[i] = NormalizeArg(args[i]);
        }

        return normalized;
    }

    private static object? NormalizeArg(object? arg)
    {
        if (arg is null)
        {
            return null;
        }

        // JS numbers are represented as System.Double throughout the runtime.
        // Normalize common CLR numeric primitives to double so arithmetic behaves as expected.
        return arg switch
        {
            double d => d,
            float f => (double)f,
            decimal m => (double)m,

            sbyte or byte or short or ushort or int or uint or long or ulong
                => Convert.ToDouble(arg, CultureInfo.InvariantCulture),

            char c => c.ToString(),
            _ => arg,
        };
    }

    public static bool TryGetExportMember(object? exports, string contractName, out object? value)
    {
        if (exports == null)
        {
            value = null;
            return false;
        }

        foreach (var candidate in GetNameCandidates(contractName))
        {
            if (exports is IDictionary<string, object?> dict && dict.TryGetValue(candidate, out value))
            {
                return true;
            }

            var type = exports.GetType();
            var prop = type.GetProperty(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null)
            {
                value = prop.GetValue(exports);
                return true;
            }

            var field = type.GetField(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
            {
                value = field.GetValue(exports);
                return true;
            }
        }

        value = null;
        return false;
    }

    public static object? GetExportMember(object? exports, string contractName)
    {
        if (!TryGetExportMember(exports, contractName, out var value))
        {
            if (exports == null)
            {
                throw new InvalidOperationException("Module exports is null.");
            }

            throw new MissingMemberException($"Export '{contractName}' not found.");
        }

        return value;
    }

    public static IEnumerable<string> GetNameCandidates(string contractName)
    {
        yield return contractName;

        if (contractName.Length > 0)
        {
            yield return string.Create(contractName.Length, contractName, (span, name) =>
            {
                name.AsSpan().CopyTo(span);
                span[0] = char.ToLowerInvariant(span[0]);
            });
        }
    }

    public static object? InvokeJsDelegate(Delegate d, object?[] args)
    {
        args = NormalizeArgs(args);

        var parameters = d.Method.GetParameters();
        if (parameters.Length == 0)
        {
            return d.DynamicInvoke(Array.Empty<object?>());
        }

        var invokeArgs = new object?[parameters.Length];

        var argIndex = 0;
        if (parameters[0].ParameterType == typeof(object[]))
        {
            invokeArgs[0] = Array.Empty<object>();
            argIndex = 1;
        }

        for (var i = 0; i < args.Length && argIndex < invokeArgs.Length; i++, argIndex++)
        {
            invokeArgs[argIndex] = args[i];
        }

        while (argIndex < invokeArgs.Length)
        {
            invokeArgs[argIndex++] = null;
        }

        try
        {
            return d.DynamicInvoke(invokeArgs);
        }
        catch (TargetInvocationException tie) when (tie.InnerException != null)
        {
            throw tie.InnerException;
        }
    }

    public static object? InvokeInstanceMethod(object target, string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        args = NormalizeArgs(args);

        foreach (var candidate in GetNameCandidates(methodName))
        {
            if (TryInvokeMethod(target, candidate, args, out var result))
            {
                return result;
            }
        }

        throw new MissingMethodException($"Member method '{methodName}' not found on '{target.GetType().FullName}'.");
    }

    public static object? Construct(object constructor, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(constructor);

        args = NormalizeArgs(args);

        if (constructor is Delegate d)
        {
            return InvokeJsDelegate(d, args);
        }

        if (constructor is Type type)
        {
            if (TryInvokeStaticConstruct(type, args, out var constructed))
            {
                return constructed;
            }

            if (TryInvokeConstructor(type, args, out constructed))
            {
                return constructed;
            }

            throw new MissingMethodException($"No matching constructor found for '{type.FullName}'.");
        }

        if (constructor is ConstructorInfo ctorInfo)
        {
            if (TryBuildInvokeArgs(ctorInfo.GetParameters(), args, out var invokeArgs))
            {
                return ctorInfo.Invoke(invokeArgs);
            }

            throw new MissingMethodException($"No matching constructor found for '{ctorInfo.DeclaringType?.FullName}'.");
        }

        if (constructor is MethodInfo methodInfo)
        {
            if (TryBuildInvokeArgs(methodInfo.GetParameters(), args, out var invokeArgs))
            {
                return methodInfo.Invoke(null, invokeArgs);
            }
        }

        if (TryInvokeMethod(constructor, "Construct", args, out var result))
        {
            return result;
        }

        throw new MissingMethodException($"Export '{constructor.GetType().FullName}' is not constructible.");
    }

    private static bool TryInvokeMethod(object target, string methodName, object?[] args, out object? result)
    {
        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase));

        foreach (var method in methods)
        {
            if (TryBuildInvokeArgs(method.GetParameters(), args, out var invokeArgs))
            {
                result = method.Invoke(target, invokeArgs);
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryInvokeStaticConstruct(Type type, object?[] args, out object? result)
    {
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
            .Where(m => string.Equals(m.Name, "Construct", StringComparison.OrdinalIgnoreCase));

        foreach (var method in methods)
        {
            if (TryBuildInvokeArgs(method.GetParameters(), args, out var invokeArgs))
            {
                result = method.Invoke(null, invokeArgs);
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryInvokeConstructor(Type type, object?[] args, out object? result)
    {
        foreach (var ctor in type.GetConstructors(BindingFlags.Public | BindingFlags.Instance))
        {
            if (TryBuildInvokeArgs(ctor.GetParameters(), args, out var invokeArgs))
            {
                result = ctor.Invoke(invokeArgs);
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryBuildInvokeArgs(ParameterInfo[] parameters, object?[] args, out object?[] invokeArgs)
    {
        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object[]))
        {
            invokeArgs = new object?[] { args };
            return true;
        }

        if (args.Length > parameters.Length)
        {
            invokeArgs = Array.Empty<object?>();
            return false;
        }

        invokeArgs = new object?[parameters.Length];
        for (var i = 0; i < args.Length; i++)
        {
            invokeArgs[i] = args[i];
        }

        for (var i = args.Length; i < parameters.Length; i++)
        {
            invokeArgs[i] = parameters[i].HasDefaultValue ? Type.Missing : null;
        }

        return true;
    }
}
