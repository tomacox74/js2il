using Acornima;
using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void InferReceiverTypeCandidates(Scope root)
    {
        var stringObjectCandidateBindings = new HashSet<BindingInfo>();

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
            var pending = new Stack<(Node Node, bool IsScopeRoot)>();
            pending.Push((scope.AstNode, true));

            while (pending.Count > 0)
            {
                var (node, isScopeRoot) = pending.Pop();
                if (!isScopeRoot
                    && scope.Children.Any(child => ReferenceEquals(child.AstNode, node)))
                {
                    continue;
                }

                switch (node)
                {
                    case VariableDeclarator
                    {
                        Id: Identifier identifier,
                        Init: { } initializer
                    }:
                        AddAssignmentCandidates(
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
                        if (assignment.Operator == Operator.Assignment)
                        {
                            AddAssignmentCandidates(
                                binding,
                                assignment.Right,
                                scope);
                        }
                        else if (assignment.Operator == Operator.AdditionAssignment
                            && binding != null
                            && (binding.ReceiverCandidateClrTypes.Contains(typeof(string))
                                || stringObjectCandidateBindings.Contains(binding)
                                || ContainsStringCandidate(assignment.Right, scope)))
                        {
                            changed |= binding.ReceiverCandidateClrTypes.Add(typeof(string));
                        }
                        break;
                    }
                }

                foreach (var child in node.ChildNodes)
                {
                    pending.Push((child, false));
                }
            }
        }

        void AddAssignmentCandidates(
            BindingInfo? binding,
            Expression expression,
            Scope scope)
        {
            AddCandidates(binding, expression, scope);
            if (binding != null
                && IsStringObjectCandidate(expression, scope))
            {
                changed |= stringObjectCandidateBindings.Add(binding);
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

            switch (expression)
            {
                case StringLiteral:
                    yield return typeof(string);
                    yield break;

                case ArrayExpression:
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case CallExpression
                    {
                        Callee: Identifier { Name: "String" }
                    }
                    when !IsIdentifierShadowed(scope, "String"):
                case CallExpression
                    {
                        Callee: MemberExpression
                        {
                            Computed: false,
                            Object: Identifier { Name: "String" },
                            Property: Identifier { Name: "fromCharCode" }
                        }
                    }
                    when !IsIdentifierShadowed(scope, "String"):
                    yield return typeof(string);
                    yield break;

                case NewExpression
                    {
                        Callee: Identifier constructor
                    }
                    when string.Equals(constructor.Name, "Array", StringComparison.Ordinal)
                         && !IsIdentifierShadowed(scope, "Array"):
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case NewExpression
                    {
                        Callee: Identifier constructor
                    }
                    when _runtimeIntrinsicCatalog.TryGetIntrinsicObject(constructor.Name, out var intrinsic)
                         && intrinsic != null:
                    if (IsReceiverCandidateType(intrinsic.Type))
                    {
                        yield return intrinsic.Type;
                    }
                    yield break;

                case CallExpression
                    {
                        Callee: MemberExpression
                        {
                            Computed: false,
                            Object: Identifier { Name: "Array" },
                            Property: Identifier { Name: "of" or "from" }
                        }
                    }
                    when !IsIdentifierShadowed(scope, "Array"):
                    yield return typeof(JavaScriptRuntime.Array);
                    yield break;

                case NonLogicalBinaryExpression
                    {
                        Operator: Operator.Addition
                    } addition
                    when ContainsStringCandidate(addition, scope):
                    yield return typeof(string);
                    yield break;
            }
        }

        bool IsStringObjectCandidate(Expression expression, Scope scope)
            => expression switch
            {
                NewExpression
                {
                    Callee: Identifier { Name: "String" }
                } => !IsIdentifierShadowed(scope, "String"),
                Identifier identifier => TryResolveBinding(scope, identifier.Name) is { } sourceBinding
                    && stringObjectCandidateBindings.Contains(sourceBinding),
                _ => false
            };

        bool ContainsStringCandidate(Expression expression, Scope scope)
        {
            var pending = new Stack<Expression>();
            pending.Push(expression);

            while (pending.Count > 0)
            {
                switch (pending.Pop())
                {
                    case StringLiteral:
                    case NewExpression
                        {
                            Callee: Identifier { Name: "String" }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                    case CallExpression
                        {
                            Callee: Identifier { Name: "String" }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                    case CallExpression
                        {
                            Callee: MemberExpression
                            {
                                Computed: false,
                                Object: Identifier { Name: "String" },
                                Property: Identifier { Name: "fromCharCode" }
                            }
                        }
                        when !IsIdentifierShadowed(scope, "String"):
                        return true;

                    case Identifier identifier
                        when TryResolveBinding(scope, identifier.Name) is { } binding
                            && (binding.ReceiverCandidateClrTypes.Contains(typeof(string))
                                || stringObjectCandidateBindings.Contains(binding)):
                        return true;

                    case NonLogicalBinaryExpression
                        {
                            Operator: Operator.Addition
                        } addition:
                        pending.Push(addition.Left);
                        pending.Push(addition.Right);
                        break;
                }
            }

            return false;
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
