using System.Linq.Expressions;

namespace Jroc.Services.TwoPhaseCompilation;

internal static class CallableDelegateTypeResolver
{
    public static Type GetMaterializedDelegateType(CallableId callableId, CallableSignature? signature)
    {
        var requiresScopeArray = callableId.Kind == CallableKind.Arrow
            && signature?.RequiresScopesParameter == true
            && signature.ScopeAbiKind != Jroc.Runtime.CallableScopeAbiKind.SingleScope;
        return GetDelegateType(callableId.JsParamCount, requiresScopeArray, signature);
    }

    public static Type GetMaterializedDelegateType(int jsParamCount, CallableSignature? signature)
        => GetDelegateType(jsParamCount, requiresScopes: false, signature);

    public static Type GetDelegateType(int jsParamCount, bool requiresScopes, CallableSignature? signature)
    {
        if (signature?.ParameterClrTypes.Any(type => type != null && type != typeof(object)) != true)
        {
            return BaseClassLibraryReferences.GetFunctionDelegateType(
                jsParamCount,
                requiresScopes,
                signature?.ReturnClrType);
        }

        var maxTypedJsParamCount = requiresScopes ? 13 : 14;
        if (jsParamCount > maxTypedJsParamCount)
        {
            throw new NotSupportedException(
                $"Typed materialized callable delegates support at most {maxTypedJsParamCount} JavaScript parameters for this ABI.");
        }

        var parameterTypes = new List<Type>(jsParamCount + 3);
        if (requiresScopes)
        {
            parameterTypes.Add(typeof(object[]));
        }
        parameterTypes.Add(typeof(object));

        var jsParameterTypes = signature.ParameterClrTypes;
        for (var i = 0; i < jsParamCount; i++)
        {
            parameterTypes.Add(i < jsParameterTypes.Count ? jsParameterTypes[i] ?? typeof(object) : typeof(object));
        }

        parameterTypes.Add(signature.ReturnClrType ?? typeof(object));
        return Expression.GetFuncType(parameterTypes.ToArray());
    }
}
