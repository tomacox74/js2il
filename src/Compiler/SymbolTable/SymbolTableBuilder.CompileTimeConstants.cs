using Acornima.Ast;
using Jroc.Services;

namespace Jroc.SymbolTables;

public partial class SymbolTableBuilder
{
    private void AnalyzeCompileTimeConstants(Scope scope)
        => AnalyzeCompileTimeConstants(scope, BuildNodeScopeMap(scope));

    private void AnalyzeCompileTimeConstants(
        Scope scope,
        IReadOnlyDictionary<Node, Scope> nodeScopes)
    {
        foreach (var binding in scope.Bindings.Values)
        {
            if (binding.Kind != BindingKind.Const
                || !binding.IsCaptured
                || binding.HasNonInitializationWrite
                || HasNonInitializationWrite(scope, binding, nodeScopes)
                || binding.DeclarationNode is not VariableDeclarator
                {
                    Id: Identifier declaredIdentifier,
                    Init: { } initializer
                }
                || !string.Equals(declaredIdentifier.Name, binding.Name, StringComparison.Ordinal)
                || !IsDirectUnconditionalDeclaration(scope, binding.DeclarationNode)
                || !TryGetPrimitiveConstant(initializer, out var type, out var value))
            {
                continue;
            }

            if (!TryGetBindingInitializationBoundary(binding, out var initializationBoundary))
            {
                continue;
            }

            if (HasReferenceBeforeInitialization(scope, binding, initializationBoundary, nodeScopes))
            {
                binding.RequiresRuntimeTemporalDeadZoneChecks = true;
                continue;
            }

            if (!CanEliminateCapturedConstantStorage(scope, binding, initializationBoundary))
            {
                continue;
            }

            binding.IsCompileTimeConstant = true;
            binding.CompileTimeConstantType = type;
            binding.CompileTimeConstantValue = value;
        }

        foreach (var child in scope.Children)
        {
            AnalyzeCompileTimeConstants(child, nodeScopes);
        }
    }

    private bool HasReferenceBeforeInitialization(
        Scope scope,
        BindingInfo binding,
        int initializationBoundary,
        IReadOnlyDictionary<Node, Scope> nodeScopes)
    {
        var hasReference = false;
        var declaredIdentifier = (binding.DeclarationNode as VariableDeclarator)?.Id;
        var walker = new Jroc.Utilities.AstWalker();
        walker.Visit(scope.AstNode, node =>
        {
            if (hasReference
                || node is not Identifier identifier
                || identifier.Start >= initializationBoundary
                || ReferenceEquals(identifier, declaredIdentifier)
                || !string.Equals(identifier.Name, binding.Name, StringComparison.Ordinal))
            {
                return;
            }

            nodeScopes.TryGetValue(node, out var currentScope);
            currentScope ??= scope;
            hasReference = ReferenceEquals(TryResolveBinding(currentScope, identifier.Name), binding);
        });
        return hasReference;
    }

    private bool HasNonInitializationWrite(
        Scope scope,
        BindingInfo binding,
        IReadOnlyDictionary<Node, Scope> nodeScopes)
    {
        var hasWrite = false;
        var walker = new Jroc.Utilities.AstWalker();
        walker.Visit(scope.AstNode, node =>
        {
            nodeScopes.TryGetValue(node, out var currentScope);
            currentScope ??= scope;

            hasWrite |= node switch
            {
                UpdateExpression update =>
                    IsBindingWriteTarget(update.Argument, currentScope, binding),
                AssignmentExpression assignment =>
                    IsBindingWriteTarget(assignment.Left, currentScope, binding),
                ForOfStatement forOf =>
                    IsLoopBindingWriteTarget(forOf.Left, currentScope, binding),
                ForInStatement forIn =>
                    IsLoopBindingWriteTarget(forIn.Left, currentScope, binding),
                _ => false
            };
        });
        return hasWrite;
    }

    private static bool IsDirectUnconditionalDeclaration(Scope scope, Node declarationNode)
    {
        IEnumerable<Statement> statements = scope.AstNode switch
        {
            Program program => program.Body,
            BlockStatement block => block.Body,
            FunctionDeclaration { Body: BlockStatement body } => body.Body,
            FunctionExpression { Body: BlockStatement body } => body.Body,
            ArrowFunctionExpression { Body: BlockStatement body } => body.Body,
            _ => Array.Empty<Statement>()
        };

        return statements
            .OfType<VariableDeclaration>()
            .SelectMany(declaration => declaration.Declarations)
            .Any(declarator => ReferenceEquals(declarator, declarationNode));
    }

    private bool CanEliminateCapturedConstantStorage(
        Scope declaringScope,
        BindingInfo binding,
        int initializationBoundary)
    {
        foreach (var child in declaringScope.Children)
        {
            if (!DoesScopeSubtreeReferenceBinding(child, declaringScope, binding))
            {
                continue;
            }

            if (!IsCapturedConstantBoundarySafe(child, declaringScope, binding, initializationBoundary))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCapturedConstantBoundarySafe(
        Scope candidate,
        Scope declaringScope,
        BindingInfo binding,
        int initializationBoundary)
    {
        if (candidate.Kind == ScopeKind.Block)
        {
            foreach (var child in candidate.Children)
            {
                if (DoesScopeSubtreeReferenceBinding(child, declaringScope, binding)
                    && !IsCapturedConstantBoundarySafe(child, declaringScope, binding, initializationBoundary))
                {
                    return false;
                }
            }

            // Blocks execute in the declaring callable, where lowering remains flow-sensitive.
            return true;
        }

        if (candidate.AstNode.Start < initializationBoundary)
        {
            return false;
        }

        return candidate.AstNode switch
        {
            // Function declarations are hoisted and can run before their source position.
            FunctionDeclaration => false,
            FunctionExpression or ArrowFunctionExpression or ClassDeclaration or ClassExpression => true,
            _ => false
        };
    }

    private static bool TryGetPrimitiveConstant(
        Expression initializer,
        out JavascriptType type,
        out object? value)
    {
        switch (initializer)
        {
            case NumericLiteral numeric:
                type = JavascriptType.Number;
                value = numeric.Value;
                return true;
            case StringLiteral text:
                type = JavascriptType.String;
                value = text.Value;
                return true;
            case BooleanLiteral boolean:
                type = JavascriptType.Boolean;
                value = boolean.Value;
                return true;
            case NullLiteral:
                type = JavascriptType.Null;
                value = null;
                return true;
            default:
                type = JavascriptType.Unknown;
                value = null;
                return false;
        }
    }
}
