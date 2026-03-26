using Acornima.Ast;

namespace Js2IL.SymbolTables;

public partial class SymbolTableBuilder
{
    private void AnalyzeRuntimeTemporalDeadZoneChecks(Scope scope)
    {
        foreach (var binding in scope.Bindings.Values)
        {
            binding.RequiresRuntimeTemporalDeadZoneChecks =
                binding.RequiresTemporalDeadZoneChecks
                && binding.IsCaptured
                && MayCapturedBindingBeObservedBeforeInitialization(scope, binding);
        }

        foreach (var child in scope.Children)
        {
            AnalyzeRuntimeTemporalDeadZoneChecks(child);
        }
    }

    private bool MayCapturedBindingBeObservedBeforeInitialization(Scope declaringScope, BindingInfo binding)
    {
        if (!binding.RequiresTemporalDeadZoneChecks || !binding.IsCaptured)
        {
            return false;
        }

        if (!TryGetBindingInitializationBoundary(binding, out var initializationBoundary))
        {
            return true;
        }

        foreach (var child in declaringScope.Children)
        {
            if (MayCapturedBindingBeObservedBeforeInitialization(
                    declaringScope,
                    child,
                    binding,
                    initializationBoundary))
            {
                return true;
            }
        }

        return false;
    }

    private bool MayCapturedBindingBeObservedBeforeInitialization(
        Scope scopeContext,
        Scope candidateScope,
        BindingInfo targetBinding,
        int initializationBoundary)
    {
        if (candidateScope.Kind == ScopeKind.Block)
        {
            if (candidateScope.AstNode.Start >= initializationBoundary)
            {
                return false;
            }

            // Entering the block predeclares its lexical names, so references inside the block
            // resolve to the shadowing binding rather than the outer one for the entire block.
            if (candidateScope.Bindings.ContainsKey(targetBinding.Name))
            {
                return false;
            }

            foreach (var child in candidateScope.Children)
            {
                if (MayCapturedBindingBeObservedBeforeInitialization(
                        candidateScope,
                        child,
                        targetBinding,
                        initializationBoundary))
                {
                    return true;
                }
            }

            return false;
        }

        if (candidateScope.UsesGlobalScopeSemantics)
        {
            return false;
        }

        if (!DoesScopeSubtreeReferenceBinding(candidateScope, scopeContext, targetBinding))
        {
            return false;
        }

        // Function declarations are hoisted, but they are inert until pre-init code actually
        // references the callable binding (for example, an initializer calling the function).
        if (candidateScope.AstNode is FunctionDeclaration
            && TryGetFunctionScopeBinding(scopeContext, candidateScope, out var functionBinding))
        {
            return IsBindingReferencedBeforeBoundary(scopeContext, functionBinding, initializationBoundary);
        }

        return candidateScope.AstNode.Start < initializationBoundary;
    }

    private bool DoesScopeSubtreeReferenceBinding(Scope candidateScope, Scope scopeContext, BindingInfo targetBinding)
    {
        if (!ReferenceEquals(TryResolveBinding(candidateScope, targetBinding.Name), targetBinding))
        {
            return false;
        }

        return CollectReferencedParentVariables(candidateScope, scopeContext).Contains(targetBinding.Name);
    }

    private bool IsBindingReferencedBeforeBoundary(Scope scope, BindingInfo targetBinding, int boundary)
    {
        bool referenced = false;
        WalkScope(scope);
        return referenced;

        void WalkScope(Scope currentScope)
        {
            if (referenced)
            {
                return;
            }

            if (!ReferenceEquals(TryResolveBinding(currentScope, targetBinding.Name), targetBinding))
            {
                return;
            }

            WalkNode(GetExecutedScopeBodyRoot(currentScope), currentScope);
        }

        void WalkNode(Node? node, Scope currentScope)
        {
            if (node == null || referenced || node.Start >= boundary)
            {
                return;
            }

            if (ReferenceEquals(node, targetBinding.DeclarationNode))
            {
                return;
            }

            if (!ReferenceEquals(node, currentScope.AstNode)
                && TryGetDirectChildScope(currentScope, node, out var childScope))
            {
                if (childScope.Kind == ScopeKind.Block
                    && childScope.AstNode.Start < boundary
                    && !childScope.Bindings.ContainsKey(targetBinding.Name))
                {
                    WalkScope(childScope);
                }

                return;
            }

            if (node is Identifier identifier
                && ReferenceEquals(TryResolveBinding(currentScope, identifier.Name), targetBinding))
            {
                referenced = true;
                return;
            }

            foreach (var childNode in node.ChildNodes)
            {
                WalkNode(childNode, currentScope);
                if (referenced)
                {
                    return;
                }
            }
        }
    }

    private static Node GetExecutedScopeBodyRoot(Scope scope)
    {
        return scope.AstNode switch
        {
            FunctionDeclaration fd when fd.Body is BlockStatement body => body,
            FunctionExpression fe when fe.Body is BlockStatement body => body,
            ArrowFunctionExpression af when af.Body is BlockStatement body => body,
            ArrowFunctionExpression af => af.Body,
            _ => scope.AstNode
        };
    }

    private static bool TryGetBindingInitializationBoundary(BindingInfo binding, out int boundary)
    {
        switch (binding.DeclarationNode)
        {
            case VariableDeclarator declarator:
                boundary = declarator.Init?.End ?? declarator.Start;
                return true;
            case ClassDeclaration classDeclaration:
                boundary = classDeclaration.End;
                return true;
            case ClassExpression classExpression:
                boundary = classExpression.End;
                return true;
            case Identifier identifier:
                boundary = identifier.Start;
                return true;
            default:
                boundary = binding.DeclarationNode.End;
                return boundary > 0;
        }
    }

    private static bool TryGetDirectChildScope(Scope scope, Node node, out Scope childScope)
    {
        foreach (var candidate in scope.Children)
        {
            if (ReferenceEquals(candidate.AstNode, node))
            {
                childScope = candidate;
                return true;
            }
        }

        childScope = null!;
        return false;
    }

    private static bool TryGetFunctionScopeBinding(Scope scope, Scope functionScope, out BindingInfo binding)
    {
        binding = null!;

        if (functionScope.AstNode is not FunctionDeclaration functionDeclaration
            || functionDeclaration.Id is not Identifier functionId)
        {
            return false;
        }

        if (!scope.Bindings.TryGetValue(functionId.Name, out var resolvedBinding)
            || resolvedBinding == null
            || !ReferenceEquals(resolvedBinding.DeclarationNode, functionDeclaration))
        {
            return false;
        }

        binding = resolvedBinding;
        return true;
    }
}
