using Acornima.Ast;
using Js2IL.Utilities;
using System;

namespace Js2IL.Validation;

internal static class PrototypeFeatureDetector
{
    public static bool UsesPrototypeFeatures(Node ast)
    {
        bool found = false;
        var walker = new AstWalker();

        walker.Visit(ast, node =>
        {
            if (found)
            {
                return;
            }

            switch (node)
            {
                case MemberExpression me:
                    if (IsProtoKey(me.Property, me.Computed))
                    {
                        found = true;
                    }
                    break;

                case Property p:
                    // Object literal prototype mutation via { __proto__: x }
                    // (even if we don't support it yet, treat as a clear prototype signal).
                    if (IsProtoKey(p.Key, p.Computed))
                    {
                        found = true;
                    }
                    break;

                case CallExpression call:
                    if (IsWellKnownPrototypeCall(call))
                    {
                        found = true;
                    }
                    break;
            }
        });

        return found;
    }

    private static bool IsProtoKey(Node? keyNode, bool computed)
    {
        if (!computed)
        {
            return keyNode is Identifier id && string.Equals(id.Name, "__proto__", StringComparison.Ordinal);
        }

        // Computed: obj["__proto__"]
        if (keyNode is Literal lit && lit.Value is string s)
        {
            return string.Equals(s, "__proto__", StringComparison.Ordinal);
        }

        return false;
    }

    private static bool IsWellKnownPrototypeCall(CallExpression call)
    {
        // Detect: Object.getPrototypeOf(x), Object.setPrototypeOf(x, y)
        // and the computed-string equivalents.
        if (call.Callee is not MemberExpression me)
        {
            return false;
        }

        if (me.Object is not Identifier objIdent || !string.Equals(objIdent.Name, "Object", StringComparison.Ordinal))
        {
            return false;
        }

        string? memberName = me.Computed
            ? (me.Property as Literal)?.Value as string
            : (me.Property as Identifier)?.Name;

        if (memberName == null)
        {
            return false;
        }

        return string.Equals(memberName, "getPrototypeOf", StringComparison.Ordinal)
            || string.Equals(memberName, "setPrototypeOf", StringComparison.Ordinal);
    }
}
