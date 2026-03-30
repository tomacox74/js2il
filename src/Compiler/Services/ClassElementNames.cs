using Acornima.Ast;
using System;

namespace Js2IL.Services;

internal static class ClassElementNames
{
    public static bool IsConstructor(MethodDefinition method)
    {
        return method.Key is Identifier id
            && string.Equals(id.Name, "constructor", StringComparison.Ordinal);
    }

    public static bool TryGetPropertyName(Node? keyNode, bool computed, out string? name)
    {
        if (!computed)
        {
            switch (keyNode)
            {
                case Identifier id:
                    name = id.Name;
                    return true;
                case PrivateIdentifier privateId:
                    name = privateId.Name;
                    return true;
            }
        }

        switch (keyNode)
        {
            case StringLiteral stringLiteral:
                name = stringLiteral.Value;
                return true;
            case NumericLiteral numericLiteral:
                name = numericLiteral.Value.ToString(System.Globalization.CultureInfo.InvariantCulture);
                return true;
            case TemplateLiteral templateLiteral when templateLiteral.Expressions.Count == 0 && templateLiteral.Quasis.Count == 1:
                var quasi = templateLiteral.Quasis[0].Value;
                name = quasi.Cooked ?? quasi.Raw;
                return !string.IsNullOrWhiteSpace(name);
            default:
                name = null;
                return false;
        }
    }

    public static string ManglePrivateFieldName(string name)
        => "__js2il_priv_" + name;

    public static string ManglePrivateMethodName(string name)
        => "__js2il_priv_method_" + name;

    public static string ManglePrivateAccessorMethodName(string accessorKind, string name)
        => $"__js2il_priv_{accessorKind}_{name}";

    public static string GetMethodRegistryName(MethodDefinition method)
    {
        if (method.Key is PrivateIdentifier privateId)
        {
            return method.Kind switch
            {
                PropertyKind.Get => ManglePrivateAccessorMethodName("get", privateId.Name),
                PropertyKind.Set => ManglePrivateAccessorMethodName("set", privateId.Name),
                _ => ManglePrivateMethodName(privateId.Name)
            };
        }

        var publicName = TryGetPropertyName(method.Key, method.Computed, out var resolvedName) && !string.IsNullOrWhiteSpace(resolvedName)
            ? resolvedName!
            : "method";
        return method.Kind switch
        {
            PropertyKind.Get => $"get_{publicName}",
            PropertyKind.Set => $"set_{publicName}",
            _ => publicName
        };
    }
}
