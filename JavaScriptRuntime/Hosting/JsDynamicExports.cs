using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace Js2IL.Runtime;

/// <summary>
/// Reflection/dynamic-friendly exports proxy.
/// Member access and invocations are marshalled onto the owning runtime thread.
/// </summary>
public sealed class JsDynamicExports : DynamicObject, IDisposable
{
    private readonly JsRuntimeInstance _runtime;

    internal JsDynamicExports(JsRuntimeInstance runtime)
    {
        _runtime = runtime;
    }

    public void Dispose() => _runtime.Dispose();

    public object? Get(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _runtime.Invoke(() => GetExportMember(_runtime.Exports, name));
    }

    public object? Invoke(string name, params object?[] args)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        return _runtime.Invoke(() =>
        {
            var callable = GetExportMember(_runtime.Exports, name);
            if (callable is not Delegate d)
            {
                throw new MissingMethodException($"Export '{name}' is not a callable function.");
            }

            return InvokeJsDelegate(d, args ?? Array.Empty<object?>());
        });
    }

    public override bool TryGetMember(GetMemberBinder binder, out object? result)
    {
        result = _runtime.Invoke(() => GetExportMember(_runtime.Exports, binder.Name));
        return true;
    }

    public override bool TryInvokeMember(InvokeMemberBinder binder, object?[]? args, out object? result)
    {
        result = _runtime.Invoke(() =>
        {
            var callable = GetExportMember(_runtime.Exports, binder.Name);
            if (callable is not Delegate d)
            {
                throw new MissingMethodException($"Export '{binder.Name}' is not a callable function.");
            }

            return InvokeJsDelegate(d, args ?? Array.Empty<object?>());
        });
        return true;
    }

    public override bool TrySetMember(SetMemberBinder binder, object? value)
    {
        throw new NotSupportedException("Exports are read-only via the hosting API.");
    }

    private static object? GetExportMember(object? exports, string contractName)
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

    private static IEnumerable<string> GetNameCandidates(string contractName)
    {
        yield return contractName;

        if (contractName.Length > 0)
        {
            yield return char.ToLowerInvariant(contractName[0]) + contractName.Substring(1);
        }
    }

    private static object? InvokeJsDelegate(Delegate d, object?[] args)
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
