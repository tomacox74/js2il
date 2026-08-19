using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferReceiverTypeCandidates(Scope root)
    {
        foreach (var scope in EnumerateScopes(root))
        {
            foreach (var binding in scope.Bindings.Values)
            {
                binding.ReceiverCandidateClrTypes.Clear();
                AddStableReceiverCandidate(binding);
            }
        }

        bool changed;
        do
        {
            changed = false;
            foreach (var scope in EnumerateScopes(root))
            {
                VisitScope(scope);
            }
        }
        while (changed);

        void VisitScope(Scope scope)
        {
            Visit(scope.AstNode, scope, isScopeRoot: true);
        }

        void Visit(Node node, Scope scope, bool isScopeRoot)
        {
            if (!isScopeRoot
                && scope.Children.Any(child => ReferenceEquals(child.AstNode, node)))
            {
                return;
            }

            switch (node)
            {
                case VariableDeclarator
                {
                    Id: Identifier identifier,
                    Init: { } initializer
                }:
                    AddCandidates(
                        TryResolveBinding(scope, identifier.Name),
                        initializer,
                        scope);
                    break;

                case AssignmentExpression
                {
                    Left: Identifier identifier
                } assignment:
                {
                    var binding = TryResolveBinding(scope, identifier.Name);
                    AddCandidates(binding, assignment.Right, scope);

                    if (assignment.Operator == Operator.AdditionAssignment
                        && binding?.ReceiverCandidateClrTypes.Contains(typeof(string)) == true)
                    {
                        changed |= binding.ReceiverCandidateClrTypes.Add(typeof(string));
                    }
                    break;
                }
            }

            foreach (var child in node.ChildNodes)
            {
                Visit(child, scope, isScopeRoot: false);
            }
        }

        void AddCandidates(BindingInfo? binding, Expression expression, Scope scope)
        {
            if (binding == null)
            {
                return;
            }

            foreach (var type in GetReceiverCandidates(expression, scope))
            {
                changed |= binding.ReceiverCandidateClrTypes.Add(type);
            }
        }

        IEnumerable<Type> GetReceiverCandidates(Expression expression, Scope scope)
        {
            if (expression is Identifier identifier
                && TryResolveBinding(scope, identifier.Name) is { } sourceBinding)
            {
                foreach (var candidate in sourceBinding.ReceiverCandidateClrTypes)
                {
                    yield return candidate;
                }

                yield break;
            }

            if (expression is NewExpression
                {
                    Callee: Identifier { Name: "String" }
                }
                && !IsIdentifierShadowed(scope, "String"))
            {
                yield return typeof(string);
                yield break;
            }

            if (expression is CallExpression
                {
                    Callee: Identifier { Name: "String" }
                }
                && !IsIdentifierShadowed(scope, "String"))
            {
                yield return typeof(string);
                yield break;
            }

            if (expression is CallExpression
                {
                    Callee: MemberExpression
                    {
                        Computed: false,
                        Object: Identifier { Name: "String" },
                        Property: Identifier { Name: "fromCharCode" }
                    }
                }
                && !IsIdentifierShadowed(scope, "String"))
            {
                yield return typeof(string);
                yield break;
            }

            if (expression is NonLogicalBinaryExpression
                {
                    Operator: Operator.Addition
                } addition)
            {
                foreach (var candidate in GetReceiverCandidates(addition.Left, scope))
                {
                    if (candidate == typeof(string))
                    {
                        yield return candidate;
                        yield break;
                    }
                }

                foreach (var candidate in GetReceiverCandidates(addition.Right, scope))
                {
                    if (candidate == typeof(string))
                    {
                        yield return candidate;
                        yield break;
                    }
                }
            }

            var inferredType = InferExpressionClrType(expression, scope);
            if (IsReceiverCandidateType(inferredType))
            {
                yield return inferredType!;
            }
        }
    }

    private static void AddStableReceiverCandidate(BindingInfo binding)
    {
        if (IsReceiverCandidateType(binding.ClrType))
        {
            binding.ReceiverCandidateClrTypes.Add(binding.ClrType!);
        }
    }

    private static bool IsReceiverCandidateType(Type? type)
        => type == typeof(string)
           || type is
           {
               IsAbstract: false,
               IsSealed: false,
               Namespace: { } namespaceName
           }
           && namespaceName.StartsWith("JavaScriptRuntime", StringComparison.Ordinal);
}
