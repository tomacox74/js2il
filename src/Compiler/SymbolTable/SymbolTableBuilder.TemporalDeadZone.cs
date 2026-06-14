using Acornima.Ast;

namespace Jroc.SymbolTables;

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
}
