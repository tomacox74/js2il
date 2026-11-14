using Acornima.Ast;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private ScopeAnalysisCache? _analysisCache;

    // Only lexical structure is cached: inferred types change between fixed-point passes.
    private sealed class ScopeAnalysisCache
    {
        public Dictionary<(Scope Child, Scope Target), HashSet<string>> ParentReferences { get; } = new();
        public Dictionary<Scope, HashSet<string>> AncestorNames { get; } = new();
        public Dictionary<Scope, Dictionary<Node, Scope>> ChildScopes { get; } = new();
        public HashSet<BindingInfo>? IdentifierWriteBindings { get; set; }
        public HashSet<BindingInfo> PotentialNumericForwardingBindings { get; } = new();
    }

    private Scope? FindChildScopeByAstNode(Scope scope, Node node)
    {
        if (scope.Children.Count == 0)
        {
            return null;
        }

        if (_analysisCache == null)
        {
            return scope.Children.FirstOrDefault(child => ReferenceEquals(child.AstNode, node));
        }

        if (!_analysisCache.ChildScopes.TryGetValue(scope, out var children))
        {
            children = new Dictionary<Node, Scope>(ReferenceEqualityComparer.Instance);
            foreach (var child in scope.Children)
            {
                children.TryAdd(child.AstNode, child);
            }
            _analysisCache.ChildScopes.Add(scope, children);
        }

        return children.GetValueOrDefault(node);
    }

    private bool HasIdentifierWrites(Scope scope, BindingInfo binding)
        => GetBindingWriteIndex(scope).IdentifierWriteBindings!.Contains(binding);

    private bool MayHaveNumericForwardingWrites(Scope scope, BindingInfo binding)
        => GetBindingWriteIndex(scope).PotentialNumericForwardingBindings.Contains(binding);

    private ScopeAnalysisCache GetBindingWriteIndex(Scope scope)
    {
        if (_analysisCache == null)
        {
            throw new InvalidOperationException("Write analysis requires a completed scope tree.");
        }

        if (_analysisCache.IdentifierWriteBindings == null)
        {
            var writes = new HashSet<BindingInfo>();
            foreach (var currentScope in EnumerateScopes(FindRootScope(scope)!))
            {
                Visit(currentScope.AstNode, currentScope);
            }
            _analysisCache.IdentifierWriteBindings = writes;

            void Visit(Node node, Scope currentScope)
            {
                if (!ReferenceEquals(node, currentScope.AstNode)
                    && FindChildScopeByAstNode(currentScope, node) != null)
                {
                    return;
                }

                var name = node switch
                {
                    AssignmentExpression { Left: Identifier identifier } => identifier.Name,
                    UpdateExpression { Argument: Identifier identifier } => identifier.Name,
                    _ => null
                };
                if (name != null && TryResolveBinding(currentScope, name) is { } target)
                {
                    writes.Add(target);
                }

                var (forwardingName, value) = node switch
                {
                    VariableDeclarator { Id: Identifier identifier, Init: Expression initializer }
                        => (identifier.Name, initializer),
                    AssignmentExpression { Operator: Acornima.Operator.Assignment, Left: Identifier identifier } assignment
                        => (identifier.Name, assignment.Right),
                    _ => ((string?)null, (Expression?)null)
                };
                // These are the only expression forms that the numeric analyzer follows
                // for hoisted-read provenance. Type-dependent safety is still checked there.
                if (value is Identifier or ParenthesizedExpression or SequenceExpression
                    or ConditionalExpression or LogicalExpression or AssignmentExpression
                    && forwardingName != null
                    && TryResolveBinding(currentScope, forwardingName) is { } forwardingTarget)
                {
                    _analysisCache.PotentialNumericForwardingBindings.Add(forwardingTarget);
                }

                foreach (var child in node.ChildNodes)
                {
                    Visit(child, currentScope);
                }
            }
        }

        return _analysisCache;
    }
}
