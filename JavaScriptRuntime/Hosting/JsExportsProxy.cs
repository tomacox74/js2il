using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace Js2IL.Runtime;

/// <summary>
/// DispatchProxy that projects a module's CommonJS exports object onto a .NET interface.
/// - Interface methods map to callable exports (functions).
/// - Interface properties map to exports members (get_/set_).
/// All work is marshalled to the owning <see cref="JsRuntimeInstance"/> thread.
/// </summary>
internal class JsExportsProxy : DispatchProxy
{
    private JsRuntimeInstance? _runtime;

    internal void Initialize(JsRuntimeInstance runtime)
    {
        // Late-bound initialization used by DispatchProxy.Create<T, TProxy>() patterns.
        _runtime = runtime;
    }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        // DispatchProxy can pass null if something goes wrong with reflection.
        if (targetMethod == null)
        {
            throw new ArgumentNullException(nameof(targetMethod));
        }

        // Treat missing runtime as disposed/uninitialized proxy.
        var runtime = _runtime ?? throw new ObjectDisposedException(nameof(JsExportsProxy));

        // Allow consumers to dispose the runtime by disposing the proxy interface.
        if (targetMethod.DeclaringType == typeof(IDisposable) && targetMethod.Name == nameof(IDisposable.Dispose))
        {
            runtime.Dispose();
            return null;
        }

        // Handle Object virtuals locally (no JS interaction).
        if (targetMethod.DeclaringType == typeof(object))
        {
            return HandleObjectMethod(targetMethod, args);
        }

        // Map interface properties to exports members:
        //   get_Foo -> exports.Foo
        //   set_Foo(v) -> exports.Foo = v
        if (targetMethod.IsSpecialName)
        {
            if (targetMethod.Name.StartsWith("get_", StringComparison.Ordinal))
            {
                var name = targetMethod.Name.Substring(4);
                return runtime.Invoke(() => ConvertReturn(GetExportMember(runtime.Exports, name), targetMethod.ReturnType));
            }

            if (targetMethod.Name.StartsWith("set_", StringComparison.Ordinal))
            {
                var name = targetMethod.Name.Substring(4);
                var value = args != null && args.Length > 0 ? args[0] : null;
                runtime.Invoke(() =>
                {
                    SetExportMember(runtime.Exports, name, value);
                });
                return null;
            }
        }

        // Contract note: JS Promises are not projected to Task yet.
        if (targetMethod.ReturnType == typeof(Task) ||
            (targetMethod.ReturnType.IsGenericType && targetMethod.ReturnType.GetGenericTypeDefinition() == typeof(Task<>)))
        {
            throw new NotSupportedException("Task-returning exports are not supported yet (Promise-to-Task projection pending).");
        }

        // Default path: method name maps to an exported callable function.
        return runtime.Invoke(() =>
        {
            var exportName = targetMethod.Name;
            var callable = GetExportMember(runtime.Exports, exportName);
            if (callable is not Delegate d)
            {
                throw new MissingMethodException($"Export '{exportName}' is not a callable function.");
            }

            var result = InvokeJsDelegate(d, args ?? Array.Empty<object?>());
            return ConvertReturn(result, targetMethod.ReturnType);
        });
    }

    private object? HandleObjectMethod(MethodInfo targetMethod, object?[]? args)
    {
        // Keep proxy behavior predictable for diagnostics/logging/collections.
        return targetMethod.Name switch
        {
            nameof(ToString) => nameof(JsExportsProxy),
            nameof(GetHashCode) => base.GetHashCode(),
            nameof(Equals) => ReferenceEquals(this, args != null && args.Length > 0 ? args[0] : null),
            _ => null,
        };
    }

    private static object? GetExportMember(object? exports, string contractName)
    {
        if (exports == null)
        {
            throw new InvalidOperationException("Module exports is null.");
        }

        // Try the exact name first, then a common .NET->JS convention (Foo -> foo).
        foreach (var candidate in GetNameCandidates(contractName))
        {
            // Fast path: exports as dictionary (typical for JS objects surfaced to .NET).
            if (exports is IDictionary<string, object?> dict && dict.TryGetValue(candidate, out var value))
            {
                return value;
            }

            // Fallback: reflection over a CLR type that represents exports.
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

    private static void SetExportMember(object? exports, string contractName, object? value)
    {
        if (exports == null)
        {
            throw new InvalidOperationException("Module exports is null.");
        }

        // Same candidate strategy as GetExportMember.
        foreach (var candidate in GetNameCandidates(contractName))
        {
            // If exports is a dictionary, assignment is straightforward.
            if (exports is IDictionary<string, object?> dict)
            {
                dict[candidate] = value;
                return;
            }

            // Otherwise attempt to set a CLR property/field representing the export.
            var type = exports.GetType();
            var prop = type.GetProperty(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null && prop.CanWrite)
            {
                prop.SetValue(exports, value);
                return;
            }

            var field = type.GetField(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (field != null)
            {
                field.SetValue(exports, value);
                return;
            }
        }

        throw new MissingMemberException($"Export '{contractName}' not found.");
    }

    private static IEnumerable<string> GetNameCandidates(string contractName)
    {
        // Exact match first (interface method/property name).
        yield return contractName;

        // Convenience: allow PascalCase contract members to bind to camelCase JS members.
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

        // Adapter for compiler-emitted callables:
        // JS2IL often emits a leading `object[] scopes` parameter for closure resolution.
        var invokeArgs = new object?[parameters.Length];

        var argIndex = 0;
        if (parameters[0].ParameterType == typeof(object[]))
        {
            // For direct export calls, there is no parent-scope chain to pass.
            invokeArgs[0] = Array.Empty<object>();
            argIndex = 1;
        }

        // Copy provided args; extra interface args are ignored, missing args become null.
        for (var i = 0; i < args.Length && argIndex < invokeArgs.Length; i++, argIndex++)
        {
            invokeArgs[argIndex] = args[i];
        }

        // Fill missing args with null.
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
            // Surface the underlying runtime exception rather than the reflection wrapper.
            throw tie.InnerException;
        }
    }

    private static object? ConvertReturn(object? value, Type returnType)
    {
        // Void-returning interface methods ignore JS return values.
        if (returnType == typeof(void))
        {
            return null;
        }

        // Null maps to default(T) for value types, null for reference types.
        if (value == null)
        {
            return returnType.IsValueType ? Activator.CreateInstance(returnType) : null;
        }

        // If already assignable, return as-is.
        if (returnType.IsInstanceOfType(value))
        {
            return value;
        }

        // Enum projection: Convert underlying numeric/string to enum value.
        if (returnType.IsEnum)
        {
            return Enum.ToObject(returnType, value);
        }

        // Last resort: use standard .NET conversions (e.g. boxed number -> int/double).
        return Convert.ChangeType(value, returnType);
    }
}
