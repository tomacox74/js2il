using System.Collections.Generic;
using System.Reflection;

namespace Jroc.Runtime;

internal static class ExportMemberResolver
{
    public static object? InvokeJsCallable(
        JsRuntimeInstance runtime,
        object callable,
        object?[] args,
        object? receiver = null)
    {
        if (JavaScriptRuntime.CallableOperations.IsCallable(callable))
        {
            return JavaScriptRuntime.CallableOperations.Call(
                callable,
                runtime.NormalizeHostValue(receiver),
                runtime.NormalizeHostArguments(args));
        }

        throw new ArgumentException("Value is not callable.", nameof(callable));
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
            if (exports is IDictionary<string, object?> dict
                && (dict.ContainsKey(candidate) || JavaScriptRuntime.PropertyDescriptorStore.TryGetOwn(exports, candidate, out _)))
            {
                // Read through the runtime property path so accessor properties
                // (e.g. ES module export getters) are evaluated instead of
                // returning the raw backing slot.
                value = JavaScriptRuntime.ObjectRuntime.GetProperty(exports, candidate);
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

    public static void SetExportMember(
        JsRuntimeInstance runtime,
        object? exports,
        string contractName,
        object? value)
    {
        if (exports == null)
        {
            throw new InvalidOperationException("Module exports is null.");
        }

        var targetName = ResolveExportWriteName(exports, contractName);
        _ = JavaScriptRuntime.ObjectRuntime.SetItem(
            exports,
            targetName,
            runtime.NormalizeHostValue(value));
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

    private static string ResolveExportWriteName(object exports, string contractName)
    {
        string? fallback = null;

        foreach (var candidate in GetNameCandidates(contractName))
        {
            fallback = candidate;
            if (HasExportMember(exports, candidate))
            {
                return candidate;
            }
        }

        return fallback ?? contractName;
    }

    private static bool HasExportMember(object exports, string candidate)
    {
        if (exports is IDictionary<string, object?> dict && dict.ContainsKey(candidate))
        {
            return true;
        }

        if (JavaScriptRuntime.PropertyDescriptorStore.TryGetOwn(exports, candidate, out _))
        {
            return true;
        }

        var type = exports.GetType();
        if (type.GetProperty(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) != null)
        {
            return true;
        }

        return type.GetField(candidate, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase) != null;
    }

    public static object? InvokeInstanceMethod(
        JsRuntimeInstance runtime,
        object target,
        string methodName,
        object?[] args)
    {
        ArgumentNullException.ThrowIfNull(target);
        ArgumentException.ThrowIfNullOrWhiteSpace(methodName);

        foreach (var candidate in GetNameCandidates(methodName))
        {
            var member = JavaScriptRuntime.ObjectRuntime.GetProperty(target, candidate);
            if (JavaScriptRuntime.CallableOperations.IsCallable(member))
            {
                return InvokeJsCallable(
                    runtime,
                    member!,
                    args,
                    receiver: target);
            }

            var method = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .FirstOrDefault(candidateMethod =>
                    string.Equals(
                        candidateMethod.Name,
                        candidate,
                        StringComparison.OrdinalIgnoreCase)
                    && HostedMethodFunctionObject.CanAccept(
                        candidateMethod,
                        args.Length));
            if (method != null)
            {
                var adapter = runtime.GetOrCreateHostMethodAdapter(target, method);
                return InvokeJsCallable(
                    runtime,
                    adapter,
                    args,
                    receiver: target);
            }
        }

        throw new MissingMethodException($"Member method '{methodName}' not found on '{target.GetType().FullName}'.");
    }

    public static object? Construct(
        JsRuntimeInstance runtime,
        object constructor,
        object?[] args,
        object? newTarget = null)
    {
        ArgumentNullException.ThrowIfNull(constructor);

        var effectiveNewTarget = newTarget == null
            ? constructor
            : runtime.NormalizeHostValue(newTarget);
        return JavaScriptRuntime.CallableOperations.Construct(
            constructor,
            runtime.NormalizeHostArguments(args),
            effectiveNewTarget);
    }
}
