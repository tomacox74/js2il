using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private bool IsNonEscapingGlobalThisAlias(Scope root, Scope scope, ThisExpression expression)
    {
        var parents = _globalBindingParentMap ??= BuildParentMap(root.AstNode);
        Node initializer = expression;
        while (parents.TryGetValue(initializer, out var wrapper)
            && wrapper is ParenthesizedExpression)
        {
            initializer = wrapper;
        }

        if (!parents.TryGetValue(initializer, out var parent)
            || parent is not VariableDeclarator { Id: Identifier alias } declaration
            || !ReferenceEquals(declaration.Init, initializer)
            || !parents.TryGetValue(declaration, out var declarationParent)
            || declarationParent is not VariableDeclaration
            {
                Kind: VariableDeclarationKind.Var or VariableDeclarationKind.Let or VariableDeclarationKind.Const
            })
        {
            return false;
        }

        var binding = TryResolveBinding(scope, alias.Name);
        if (binding == null
            || !ReferenceEquals(binding.DeclarationNode, declaration)
            || binding.DeclaringScope.Kind == ScopeKind.Global
            || binding.DeclaringScope.Parameters.Contains(alias.Name))
        {
            return false;
        }

        var propertyNames = new HashSet<string>(StringComparer.Ordinal);
        foreach (var (node, nodeParent) in parents)
        {
            // Dynamic name resolution and shadowing require a stronger alias proof.
            if (node is WithStatement
                || node is Identifier { Name: "eval" }
                || node is VariableDeclarator { Id: Identifier otherAlias }
                    && otherAlias.Name == alias.Name && !ReferenceEquals(node, declaration)
                || node is IFunction function
                    && (function.Id?.Name == alias.Name
                        || function.Params.Any(parameter => parameter is Identifier id && id.Name == alias.Name)))
            {
                return false;
            }

            if (node is not Identifier use
                || use.Name != alias.Name
                || IsDeclarationIdentifier(use, nodeParent))
            {
                continue;
            }

            Node receiver = use;
            var useParent = nodeParent;
            while (useParent is ParenthesizedExpression)
            {
                receiver = useParent;
                if (!parents.TryGetValue(receiver, out useParent))
                {
                    return false;
                }
            }

            if (useParent is not MemberExpression member
                || !ReferenceEquals(member.Object, receiver)
                || TryGetStaticMemberName(member) is not { } propertyName
                || propertyName is "globalThis" or "global")
            {
                return false;
            }

            Node callee = member;
            while (parents.TryGetValue(callee, out var calleeParent)
                && calleeParent is ParenthesizedExpression)
            {
                callee = calleeParent;
            }
            if (parents.TryGetValue(callee, out var callParent)
                && (callParent is CallExpression call && ReferenceEquals(call.Callee, callee)
                    || callParent is TaggedTemplateExpression tagged && ReferenceEquals(tagged.Tag, callee)))
            {
                // A method call would pass the potentially global receiver to the callee.
                return false;
            }

            propertyNames.Add(propertyName);
        }

        // Even a contained alias can replace a global or expose one of its intrinsic objects.
        // Retain per-name invalidation without treating unrelated receiver fields as an escape.
        root.WrittenGlobalObjectProperties.UnionWith(propertyNames);
        return true;
    }
}
