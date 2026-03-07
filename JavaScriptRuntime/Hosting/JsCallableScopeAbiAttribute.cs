using System;
using System.Reflection;

namespace Js2IL.Runtime;

public enum CallableScopeAbiKind
{
    NoScopes = 0,
    SingleScope = 1,
    ScopeArray = 2
}

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class JsCallableScopeAbiAttribute : Attribute
{
    public JsCallableScopeAbiAttribute(CallableScopeAbiKind kind)
    {
        Kind = kind;
    }

    public CallableScopeAbiKind Kind { get; }

    public Type? SingleScopeType { get; set; }

    public int SingleScopeTypeMetadataToken { get; set; }
}

internal readonly record struct JsCallableScopeAbiDescriptor(
    CallableScopeAbiKind Kind,
    Type? SingleScopeType,
    bool IsFromAttribute)
{
    public bool HasExplicitScopePayload => Kind != CallableScopeAbiKind.NoScopes;
}

internal static class JsCallableScopeAbiResolver
{
    public static JsCallableScopeAbiDescriptor Resolve(Delegate del)
    {
        ArgumentNullException.ThrowIfNull(del);

        var invoke = del.GetType().GetMethod("Invoke")
            ?? throw new ArgumentException($"Delegate type '{del.GetType()}' does not define Invoke().", nameof(del));

        var invokeParameters = invoke.GetParameters();
        var abiSource = GetAbiSourceDelegate(del);

        if (TryResolveFromAttribute(abiSource.Method, out var descriptor))
        {
            if (descriptor.Kind == CallableScopeAbiKind.SingleScope)
            {
                var sourceParameters = abiSource.Method.GetParameters();
                bool firstParameterAlreadyBound = abiSource.Target != null
                    && abiSource.Method.IsStatic
                    && invokeParameters.Length == Math.Max(0, sourceParameters.Length - 1);

                if (!firstParameterAlreadyBound)
                {
                    return descriptor;
                }
            }
            else
            {
                return descriptor;
            }
        }

        return InferFromParameters(invokeParameters);
    }

    public static JsCallableScopeAbiDescriptor Resolve(MethodInfo method)
    {
        ArgumentNullException.ThrowIfNull(method);

        if (TryResolveFromAttribute(method, out var descriptor))
        {
            return descriptor;
        }

        return InferFromParameters(method.GetParameters());
    }

    public static bool HasNewTargetParameter(ParameterInfo[] parameters, CallableScopeAbiKind kind)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        int newTargetIndex = kind == CallableScopeAbiKind.NoScopes ? 0 : 1;
        return parameters.Length > newTargetIndex
            && parameters[newTargetIndex].ParameterType == typeof(object)
            && string.Equals(parameters[newTargetIndex].Name, "newTarget", StringComparison.Ordinal);
    }

    public static object? GetSingleScopeArgument(object[] scopes, Type? singleScopeType)
    {
        ArgumentNullException.ThrowIfNull(scopes);

        if (singleScopeType == null)
        {
            if (scopes.Length == 1)
            {
                return scopes[0];
            }

            throw new InvalidOperationException(
                "SingleScope ABI requires either a SingleScopeType or a single-entry scopes payload.");
        }

        foreach (var candidate in scopes)
        {
            if (candidate != null && singleScopeType.IsInstanceOfType(candidate))
            {
                return candidate;
            }
        }

        throw new InvalidOperationException(
            $"SingleScope ABI could not locate a scope instance assignable to '{singleScopeType.FullName}'.");
    }

    private static Delegate GetAbiSourceDelegate(Delegate del)
    {
        return JavaScriptRuntime.Closure.TryGetBoundTarget(del, out var target)
            ? target
            : del;
    }

    private static bool TryResolveFromAttribute(MethodInfo method, out JsCallableScopeAbiDescriptor descriptor)
    {
        var attribute = method.GetCustomAttribute<JsCallableScopeAbiAttribute>(inherit: false);
        if (attribute == null)
        {
            descriptor = default;
            return false;
        }

        Type? singleScopeType = attribute.SingleScopeType;
        if (singleScopeType == null
            && attribute.Kind == CallableScopeAbiKind.SingleScope
            && attribute.SingleScopeTypeMetadataToken != 0)
        {
            singleScopeType = method.Module.ResolveType(attribute.SingleScopeTypeMetadataToken);
        }

        descriptor = new JsCallableScopeAbiDescriptor(attribute.Kind, singleScopeType, IsFromAttribute: true);
        return true;
    }

    private static JsCallableScopeAbiDescriptor InferFromParameters(ParameterInfo[] parameters)
    {
        ArgumentNullException.ThrowIfNull(parameters);

        if (parameters.Length > 0 && parameters[0].ParameterType == typeof(object[]))
        {
            return new JsCallableScopeAbiDescriptor(CallableScopeAbiKind.ScopeArray, SingleScopeType: null, IsFromAttribute: false);
        }

        return new JsCallableScopeAbiDescriptor(CallableScopeAbiKind.NoScopes, SingleScopeType: null, IsFromAttribute: false);
    }
}
