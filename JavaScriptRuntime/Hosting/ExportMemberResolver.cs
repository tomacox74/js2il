using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Globalization;
using System.Runtime.ExceptionServices;

namespace Js2IL.Runtime;

internal static class ExportMemberResolver
{
    private static object[] CreateDefaultScopes(object? seed)
    {
        // Resumable callables assume scopes[0] exists (they probe it for the state-machine scope).
        // When hosting invokes them directly, there may be no scopes array to thread through.
        // Seed with a single entry so scopes[0] is in-range; use the instance/closure target when available.
        return new object[] { seed! };
    }

    private static object[] NormalizeArgs(object?[] args)
    {
        if (args.Length == 0)
        {
            return Array.Empty<object>();
        }

        object[]? normalized = null;
        for (var i = 0; i < args.Length; i++)
        {
            var original = args[i];
            var converted = NormalizeArg(original);

            if (!ReferenceEquals(converted, original))
            {
                if (normalized == null)
                {
                    normalized = new object[args.Length];

                    for (var j = 0; j < i; j++)
                    {
                        normalized[j] = args[j]!;
                    }
                }

                normalized[i] = converted!;
                continue;
            }

            if (normalized != null)
            {
                normalized[i] = original!;
            }
        }

        return normalized ?? (object[])(object)args;
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
            double => arg,
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
        var callArgs = NormalizeArgs(args);

        // IMPORTANT:
        // Use the delegate type's Invoke signature, not d.Method.
        // For open-instance delegates, d.Method.GetParameters() omits the receiver
        // (because it's an instance method), but Delegate.DynamicInvoke expects the
        // full Invoke parameter list (which includes the receiver).
        var invokeMethod = d.GetType().GetMethod("Invoke")
            ?? throw new ArgumentException($"Delegate type '{d.GetType()}' does not define Invoke().", nameof(d));

        var parameters = invokeMethod.GetParameters();
        if (parameters.Length == 0)
        {
            return d.DynamicInvoke(Array.Empty<object?>());
        }

        var invokeArgs = new object?[parameters.Length];

        var argIndex = 0;
        if (parameters[0].ParameterType == typeof(object[]))
        {
            invokeArgs[0] = CreateDefaultScopes(d.Target);
            argIndex = 1;
        }

        for (var i = 0; i < callArgs.Length && argIndex < invokeArgs.Length; i++, argIndex++)
        {
            invokeArgs[argIndex] = callArgs[i];
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
            ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
            throw;
        }
    }

    public static object? InvokeInstanceMethod(object target, string methodName, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        var callArgs = NormalizeArgs(args);

        foreach (var candidate in GetNameCandidates(methodName))
        {
            if (TryInvokeMethod(target, candidate, callArgs, out var result))
            {
                return result;
            }
        }

        throw new MissingMethodException($"Member method '{methodName}' not found on '{target.GetType().FullName}'.");
    }

    public static object? Construct(object constructor, object?[] args)
    {
        ArgumentNullException.ThrowIfNull(constructor);

        var callArgs = NormalizeArgs(args);
        return JavaScriptRuntime.Object.ConstructValue(constructor, callArgs);
    }

    private static bool TryInvokeMethod(object target, string methodName, object[] args, out object? result)
    {
        var methods = target.GetType()
            .GetMethods(BindingFlags.Instance | BindingFlags.Public)
            .Where(m => string.Equals(m.Name, methodName, StringComparison.OrdinalIgnoreCase));

        foreach (var method in methods)
        {
            if (TryBuildInvokeArgs(method.GetParameters(), args, target, out var invokeArgs))
            {
                try
                {
                    result = method.Invoke(target, invokeArgs);
                }
                catch (TargetInvocationException tie) when (tie.InnerException != null)
                {
                    ExceptionDispatchInfo.Capture(tie.InnerException).Throw();
                    throw;
                }
                return true;
            }
        }

        result = null;
        return false;
    }

    private static bool TryBuildInvokeArgs(ParameterInfo[] parameters, object[] args, object? target, out object?[] invokeArgs)
    {
        // Support varargs-style CLR methods like Foo(object[] args) by passing the entire argument list.
        // But do NOT treat the js2il ABI scopes parameter (object[] scopes) as varargs.
        if (parameters.Length == 1
            && parameters[0].ParameterType == typeof(object[])
            && !string.Equals(parameters[0].Name, "scopes", StringComparison.OrdinalIgnoreCase))
        {
            invokeArgs = new object?[] { args };
            return true;
        }

        // Resumable callables (async/generator) follow the js2il ABI and take a leading scopes array.
        // Hosting calls do not carry scopes, so supply an ABI-compatible empty scopes array.
        var scopesOffset = 0;
        if (parameters.Length > 0 && parameters[0].ParameterType == typeof(object[]))
        {
            scopesOffset = 1;
        }

        if (args.Length > (parameters.Length - scopesOffset))
        {
            invokeArgs = Array.Empty<object?>();
            return false;
        }

        invokeArgs = new object?[parameters.Length];

        if (scopesOffset == 1)
        {
            invokeArgs[0] = CreateDefaultScopes(target);
        }

        for (var i = 0; i < args.Length; i++)
        {
            invokeArgs[i + scopesOffset] = args[i];
        }

        for (var i = args.Length + scopesOffset; i < parameters.Length; i++)
        {
            invokeArgs[i] = parameters[i].HasDefaultValue ? Type.Missing : null;
        }

        return true;
    }
}
