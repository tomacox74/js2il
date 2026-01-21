using System.Collections.Generic;
using System.Reflection;

namespace Js2IL.Runtime;

internal static class ExportMemberResolver
{
    public static object? GetExportMember(object? exports, string contractName)
    {
        if (exports == null)
        {
            throw new InvalidOperationException("Module exports is null.");
        }

        foreach (var candidate in GetNameCandidates(contractName))
        {
            if (exports is IDictionary<string, object?> dict && dict.TryGetValue(candidate, out var value))
            {
                return value;
            }

            var type = exports.GetType();
            var prop = type.GetProperty(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null)
            {
                return prop.GetValue(exports);
            }

            var field = type.GetField(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
            {
                return field.GetValue(exports);
            }
        }

        throw new MissingMemberException($"Export '{contractName}' not found.");
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
}
